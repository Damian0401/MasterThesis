using System.Text;
using System.Text.Json;
using Azure;
using Azure.AI.Inference;
using ProjectAnalizer.Dtos;

namespace ProjectAnalizer;

internal class ProjectAnalyzer
{
    internal static async Task AnalyzeAsync(
        IReadOnlyCollection<string> models,
        string token,
        string endpoint,
        string outputFileName,
        IReadOnlyCollection<string> extensionsToAnalyze,
        IReadOnlyCollection<string> directoriesToskip,
        CancellationToken cancellationToken = default)
    {
        var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories)
            .Where(file => extensionsToAnalyze.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .Where(file => !directoriesToskip.Any(dir => file.Contains(@$"{dir}\", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var fileContents = new Dictionary<string, string>();
        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), file);
            var fileContent = await File.ReadAllTextAsync(file, cancellationToken);
            fileContents[relativePath] = fileContent;
        }

        var promptBuilder = new StringBuilder(Constants.SystemPrompt);
        foreach (var (key, value) in fileContents)
        {
            promptBuilder.AppendLine($"----- {key}");
            promptBuilder.AppendLine(value);
        }
        promptBuilder.AppendLine("-----");
        var userPrompt = promptBuilder.ToString();
        await File.WriteAllTextAsync("prompt.txt", userPrompt, cancellationToken); // TODO Remove

        var client = CreateChatClient(token, endpoint);

        var sarif = new SarifDto
        {
            Schema = Constants.SarifSchema,
            Version = Constants.SarifVersion,
            Runs = []
        };

        foreach (var model in models)
        {
            var dto = await GetLlmResponseOrNullAsync(model, userPrompt, client, cancellationToken);
            if (dto is null)
            {
                continue;
            }
            var rules = dto.Results
                .GroupBy(x => x.RuleId)
                .Select(MapToRule())
                .ToList();
            var results = dto.Results
                .Select(MapToResult(rules))
                .ToList();
            sarif.Runs.Add(new SarifDto.RunDto
            {
                Tool = new SarifDto.ToolDto
                {
                    Driver = new SarifDto.DriverDto
                    {
                        Name = model,
                        SemanticVersion = "1.0.0",
                        Version = "1.0.0",
                        InformationUri = endpoint,
                        Rules = rules
                    }
                },
                Results = results,
            });
        }

        var content = JsonSerializer.Serialize(sarif, Constants.SarifJsonOptions);
        await File.WriteAllTextAsync(outputFileName, content, cancellationToken);
    }

    private static Func<IGrouping<string, LlmResponseDto.ResultDto>, SarifDto.RuleDto> MapToRule()
    {
        return x => new SarifDto.RuleDto
        {
            Id = x.Key,
            DefaultConfiguration = new()
            {
                Level = x.GroupBy(r => r.Level)
                    .OrderByDescending(r => r.Count())
                    .First()
                    .First()
                    .Level
            },
            Help = new()
            {
                Text = string.Empty
            },
            Properties = new()
            {
                Categories = [],
                Tags = []
            },
            ShortDescription = new()
            {
                Text = x.First().RuleDescription
            }
        };
    }

    private static Func<LlmResponseDto.ResultDto, SarifDto.ResultDto> MapToResult(List<SarifDto.RuleDto> rules)
    {
        return x => new SarifDto.ResultDto
        {
            Level = x.Level,
            RuleId = x.RuleId,
            Message = new()
            {
                Text = x.Message
            },
            RuleIndex = rules.IndexOf(rules.First(r => r.Id == x.RuleId)),
            Locations = [
                new()
                {
                    PhysicalLocation = new()
                    {
                        ArtifactLocation = new()
                        {
                            Uri = x.Path,
                            UriBaseId = "solutionDir"
                        },
                        Region = new()
                        {
                            StartLine = x.StartLine,
                            EndLine = x.EndLine,
                            StartColumn = x.StartColumn,
                            EndColumn = x.EndColumn
                        }
                    }
                }
            ]
        };
    }

    private static async Task<LlmResponseDto?> GetLlmResponseOrNullAsync(
        string model, 
        string userPrompt, 
        ChatCompletionsClient client,
        CancellationToken cancellationToken)
    {
        var options = new ChatCompletionsOptions
        {
            Model = model,
            Messages = [
                new ChatRequestUserMessage(userPrompt),
            ]
        };

        var chatResponse = await client.CompleteAsync(options, cancellationToken);
        var match = Constants.JsonRegex().Match(chatResponse.Value.Content);
        if (!match.Success)
        {
            await LogAsync($"{model} failed - json tag is missing.", cancellationToken); // TODO Remove
            return null;
        }
        var jsonContent = match.Groups[1].Value;
        try
        {
            var responseDto = JsonSerializer.Deserialize<IEnumerable<LlmResponseDto.ResultDto>>(jsonContent, Constants.ChatJsonOptions);
            await LogAsync($"{model} successed.", cancellationToken); // TODO Remove
            return new LlmResponseDto
            {
                Results = responseDto!
            };
        }
        catch (Exception e)
        {
            await LogAsync($"{model} failed - {e.Message}", cancellationToken); // TODO Remove
            return null;
        }
    }

    private static ChatCompletionsClient CreateChatClient(string token, string endpoint)
    {
        var credential = new AzureKeyCredential(token);
        return new ChatCompletionsClient(
            new Uri(endpoint),
            credential);
    }

    private static async Task LogAsync(string message, CancellationToken cancellationToken)
    {
        await File.AppendAllTextAsync("logs.txt", $"{DateTime.Now}: {message}\n", cancellationToken);
    }
}