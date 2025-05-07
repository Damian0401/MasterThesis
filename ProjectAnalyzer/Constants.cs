using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ProjectAnalizer;

internal static partial class Constants
{
    internal const string SystemPrompt = """
    You are a static code analysis engine. Your task is to review the provided source code and identify:
    - *Security vulnerabilities* such as injection risks, improper authentication, insecure data handling, outdated dependencies, etc.
    - *Code smells* including poor coding practices, anti-patterns, and maintainability issues.
    - *Bugs or risky logic* such as null pointer dereferencing, race conditions, unhandled edge cases, or logical errors.
    Your output must be a JSON array, enclosed between triple backticks (```json and ```), with each finding represented as a JSON object in the following format:
    [{"RuleId":"string","RuleDescription":"string","Level":"Error"|"Warning"|"Note"|"None","Message":"string","Path":"string","Category":"string","StartLine":integer,"EndLine":integer,"StartColumn":integer,"EndColumn":integer}]

    Field description:
    - RuleId: A short identifier for the rule or issue.
    - RuleDescription: A brief description of the rule being violated.
    - Level: Severity of the issue (Error, Warning, Note, or None).
    - Message: A concise explanation of the specific issue found.
    - Path: The file path where the issue occurs.
    - Category: The general category.
    - StartLine, EndLine: Line range of the issue.
    - StartColumn, EndColumn: Column range of the issue.

    Ensure the JSON is well-formed and strictly adheres to this json structure. All fields are required.
    """;

    internal const string SarifSchema = "https://raw.githubusercontent.com/oasis-tcs/sarif-spec/master/Schemata/sarif-schema-2.1.0.json";

    internal const string SarifVersion = "2.1.0";

    internal static readonly JsonSerializerOptions ChatJsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    internal static readonly JsonSerializerOptions SarifJsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    [GeneratedRegex(@"```json\s*(.*?)\s*```", RegexOptions.Singleline)]
    internal static partial Regex JsonRegex();
}