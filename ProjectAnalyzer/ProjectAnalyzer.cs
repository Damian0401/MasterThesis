using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectAnalizer.Dtos;

namespace ProjectAnalizer;

internal class ProjectAnalyzer
{
    private const string BasePrompt = """
    You are a static code analysis engine. Your task is to review the provided source code and identify:
    - *Security vulnerabilities* such as injection risks, improper authentication, insecure data handling, outdated dependencies, etc.
    - *Code smells* including poor coding practices, anti-patterns, and maintainability issues.
    - *Bugs or risky logic* such as null pointer dereferencing, race conditions, unhandled edge cases, or logical errors.

    Your output should formated and structured in rules and results.

    Input files:

    
    """;

    private const string SarifSchema = "https://raw.githubusercontent.com/oasis-tcs/sarif-spec/master/Schemata/sarif-schema-2.1.0.json";
    private const string SarifVersion = "2.1.0";

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

        var promptBuilder = new StringBuilder(BasePrompt);
        foreach (var (key, value) in fileContents)
        {
            promptBuilder.AppendLine($"----- {key}");
            promptBuilder.AppendLine(value);
        }
        promptBuilder.AppendLine("-----");
        var prompt = promptBuilder.ToString();
        await File.WriteAllTextAsync("prompt.txt", prompt, cancellationToken); // TODO: remove

        var sarif = new SarifDto
        {
            Schema = SarifSchema,
            Version = SarifVersion,
            Runs = []
        };

        foreach (var model in models)
        {
            var response = new LlmResponseDto
            {
                Rules = [],
                Results = []
            };
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
                        Rules = response.Rules
                    }
                },
                Results = response.Results,
            });
        }

        var content = JsonSerializer.Serialize(sarif, new JsonSerializerOptions() 
            { 
                WriteIndented = true, 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });
        await File.WriteAllTextAsync(outputFileName, content, cancellationToken);
    }
}