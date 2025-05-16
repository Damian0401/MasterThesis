using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using SonarQubeToSarif.Dtos;

namespace SonarQubeToSarif;

internal static class SonarQubeParser
{
    private const string IssuesUrl = "/api/issues/search";
    private const string HotspotsUrl = "/api/hotspots/search";
    private const string RulesUrl = "/api/rules/show";
    private const string SarifSchema = "https://json.schemastore.org/sarif-2.1.0.json";
    private const string SarifVersion = "2.1.0";

    internal static async Task ParseAsync(
        string host,
        string project,
        string token,
        string outputFileName,
        bool includeIssues,
        bool includeHotspots,
        CancellationToken cancellationToken = default)
    {
        using var httpClient = CreateHttpClient(host, token);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        List<SarifDto.RuleDto> ruleDetails = [];
        List<SarifDto.ResultDto> results = [];

        if (includeIssues)
        {
            var issuesResults = await GetIssuesAsync(project, httpClient, options, ruleDetails, cancellationToken);
            results.AddRange(issuesResults);
        }

        if (includeHotspots)
        {
            var hotspotsResults = await GetHotspotsAsync(project, httpClient, options, ruleDetails, cancellationToken);
            results.AddRange(hotspotsResults);
        }

        var sarif = new SarifDto
        {
            Schema = SarifSchema,
            Version = SarifVersion,
            Runs =
            [
                new()
                {
                    Tool = new()
                    {
                        Driver = new()
                        {
                            Name = "SonarQube",
                            SemanticVersion = "1.0.0",
                            Version = "1.0.0",
                            InformationUri = $"{host}/coding_rules",
                            Rules = ruleDetails
                        }
                    },
                    Results = results,
                }
            ]
        };
        var content = JsonSerializer.Serialize(sarif, new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        });
        await File.WriteAllTextAsync(outputFileName, content, cancellationToken);
    }

    private static async Task<IEnumerable<SarifDto.ResultDto>> GetIssuesAsync(
        string project,
        HttpClient httpClient,
        JsonSerializerOptions options,
        List<SarifDto.RuleDto> ruleDetails,
        CancellationToken cancellationToken)
    {
        var issuesQueryParam = $"?componentKeys={project}";
        var issues = await httpClient.GetFromJsonAsync<IssuesDto>(IssuesUrl + issuesQueryParam, options, cancellationToken);
        ArgumentNullException.ThrowIfNull(issues, nameof(IssuesDto));
        var issuesRules = issues.Issues.Select(i => i.Rule).Distinct().ToList();
        var issueRuleDetails = await GetRuleDetailsAsync(httpClient, issuesRules, cancellationToken);
        ruleDetails.AddRange(issueRuleDetails);
        var ruleDetailsDictionary = issueRuleDetails.ToDictionary(x => x.Id);
        var issuesComponents = issues.Components.ToDictionary(x => x.Key);
        var issuesResults = issues.Issues.Select(i => new SarifDto.ResultDto
        {
            RuleId = i.Rule,
            RuleIndex = ruleDetails.IndexOf(ruleDetailsDictionary[i.Rule]),
            Level = GetRuleSeverity(i.Severity),
            Message = new()
            {
                Text = i.Message
            },
            Locations = [
                new()
                    {
                        PhysicalLocation = new()
                        {
                            ArtifactLocation = new()
                            {
                                Uri = issuesComponents[i.Component].Path ?? issuesComponents[i.Component].Key,
                                UriBaseId = "solutionDir"
                            },
                            Region = new()
                            {
                                StartLine = i.TextRange.StartLine + 1,
                                EndLine = i.TextRange.EndLine + 1,
                                StartColumn = i.TextRange.StartOffset + 1,
                                EndColumn = i.TextRange.EndOffset + 1
                            }
                        }
                    }
            ]
        });
        return issuesResults;
    }

    private static async Task<IEnumerable<SarifDto.ResultDto>> GetHotspotsAsync(
        string project,
        HttpClient httpClient,
        JsonSerializerOptions options,
        List<SarifDto.RuleDto> ruleDetails,
        CancellationToken cancellationToken)
    {
        var hotspotsQueryParam = $"?project={project}";
        var hotspots = await httpClient.GetFromJsonAsync<HotspotsDto>(HotspotsUrl + hotspotsQueryParam, options, cancellationToken);
        ArgumentNullException.ThrowIfNull(hotspots, nameof(HotspotsDto));
        var hotspotsRules = hotspots.Hotspots.Select(h => h.RuleKey).Distinct().ToList();
        var hotspotsRuleDetails = await GetRuleDetailsAsync(httpClient, hotspotsRules, cancellationToken);
        ruleDetails.AddRange(hotspotsRuleDetails);
        var ruleDetailsDictionary = hotspotsRuleDetails.ToDictionary(x => x.Id);
        var hotspotsComponents = hotspots.Components.ToDictionary(x => x.Key);
        var hotspotsResults = hotspots.Hotspots.Select(h => new SarifDto.ResultDto
        {
            RuleId = h.RuleKey,
            RuleIndex = ruleDetails.IndexOf(ruleDetailsDictionary[h.RuleKey]),
            Level = GetRuleSeverity(h.VulnerabilityProbability),
            Message = new()
            {
                Text = h.Message
            },
            Locations = [
                new()
                    {
                        PhysicalLocation = new()
                        {
                            ArtifactLocation = new()
                            {
                                Uri = hotspotsComponents[h.Component].Path ?? hotspotsComponents[h.Component].Key,
                                UriBaseId = "solutionDir"
                            },
                            Region = new()
                            {
                                StartLine = h.TextRange.StartLine,
                                EndLine = h.TextRange.EndLine,
                                StartColumn = h.TextRange.StartOffset,
                                EndColumn = h.TextRange.EndOffset
                            }
                        }
                    }
            ]
        });
        return hotspotsResults;
    }

    private static async Task<List<SarifDto.RuleDto>> GetRuleDetailsAsync(
        HttpClient httpClient,
        ICollection<string> rules,
        CancellationToken cancellationToken)
    {
        var ruleDetails = new List<SarifDto.RuleDto>();
        foreach (var rule in rules)
        {
            var ruleInfo = await httpClient.GetFromJsonAsync<RulesDto>(RulesUrl + $"?key={rule}", cancellationToken);
            ArgumentNullException.ThrowIfNull(ruleInfo, nameof(RulesDto));
            var ruleDto = new SarifDto.RuleDto
            {
                Id = ruleInfo.Rule.Key,
                ShortDescription = new()
                {
                    Text = ruleInfo.Rule.Name
                },
                Help = new()
                {
                    Text = ruleInfo.Rule.DescriptionSections
                        .Where(x => x.Key == "how_to_fix")
                        .FirstOrDefault()?.Content ?? string.Empty
                },
                Properties = new()
                {
                    Tags = ruleInfo.Rule.SysTags,
                    Categories = [ruleInfo.Rule.Type]
                },
                DefaultConfiguration = new()
                {
                    Level = GetRuleSeverity(ruleInfo.Rule.Severity)
                },
            };
            ruleDetails.Add(ruleDto);
        }

        return ruleDetails;
    }

    private static HttpClient CreateHttpClient(string host, string token)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        httpClient.BaseAddress = new Uri(host);
        return httpClient;
    }

    private static SarifDto.SeverityLevel GetRuleSeverity(string severity)
    {
        return severity switch
        {
            // Issue severity
            "BLOCKER" => SarifDto.SeverityLevel.Error,
            "CRITICAL" => SarifDto.SeverityLevel.Error,
            "MAJOR" => SarifDto.SeverityLevel.Warning,
            "MINOR" => SarifDto.SeverityLevel.Note,
            "INFO" => SarifDto.SeverityLevel.None,
            // Hotspot severity
            "HIGH" => SarifDto.SeverityLevel.Error,
            "MEDIUM" => SarifDto.SeverityLevel.Warning,
            "LOW" => SarifDto.SeverityLevel.Note,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }
}