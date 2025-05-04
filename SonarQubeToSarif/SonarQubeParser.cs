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
    private const string SarifSchema = "https://raw.githubusercontent.com/oasis-tcs/sarif-spec/master/Schemata/sarif-schema-2.1.0.json";
    private const string SarifVersion = "2.1.0";

    internal static async Task ParseAsync(
        string host,
        string project,
        string token,
        string outputFileName,
        CancellationToken cancellationToken = default)
    {
        using var httpClient = CreateHttpClient(host, token);
        var queryParam = $"?project={project}";
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        var hotspots = await httpClient.GetFromJsonAsync<HotspotsDto>(HotspotsUrl + queryParam, options, cancellationToken);
        ArgumentNullException.ThrowIfNull(hotspots, nameof(HotspotsDto));
        var issues = await httpClient.GetFromJsonAsync<IssuesDto>(IssuesUrl + queryParam, options, cancellationToken);
        ArgumentNullException.ThrowIfNull(issues, nameof(IssuesDto));
        
        var hotspotsRules = hotspots.Hotspots.Select(h => h.RuleKey).ToList();
        var issuesRules = issues.Issues.Select(i => i.Rule).ToList();
        var rules = hotspotsRules.Union(issuesRules).Distinct().ToList();

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
        var ruleDetailsDictionary = ruleDetails.ToDictionary(x => x.Id);

        var issuesComponents = issues.Components.ToDictionary(x => x.Key);
        var issuesList = issues.Issues.Select(i => new SarifDto.ResultDto
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
                            StartLine = i.TextRange.StartLine,
                            EndLine = i.TextRange.EndLine,
                            StartColumn = i.TextRange.StartOffset,
                            EndColumn = i.TextRange.EndOffset
                        }
                    }
                }
            ]
        }).ToList();

        var hotspotsComponents = hotspots.Components.ToDictionary(x => x.Key);
        var hotspotsList = hotspots.Hotspots.Select(h => new SarifDto.ResultDto
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
        }).ToList();

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
                    Results = [..issuesList.Concat(hotspotsList)],
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