namespace Utils;

internal static class Consts
{
    public class Environments
    {
        public const string GithubToken = "GITHUB_TOKEN";
    }

    public class GitHubModels
    {
        public const string Phi3Mini4kInstruct = "Phi-3-mini-4k-instruct";
        public const string Phi3Small8kInstruct = "Phi-3-small-8k-instruct";
        public const string GPT4o = "gpt-4o";
        public const string MetaLlama3_70BInstruct = "Meta-Llama-3-70B-Instruct";
    }

    public const string SystemMessage = """
    Act as static code analysis tool. Search for different types of vulnerabilities in the code.
    Give your findings severity level and at least one tag. Provide line number and short description of the issue.
    Tags:
    #bug - logic errors, null pointer dereference, incorrect memory management, concurrency issues, potential crashes.
    #vulnerability - injection vulnerabilities, sensitive data exposure, XSS, CSRF, insecure deserialization, use of weak encryption algorithms.
    #code-smell - duplicated code, complex or lengthy methods, poor naming conventions, unused variables or methods.
    #security-hotspot - sensitive API usage, dynamic SQL queries, potential privilege escalation paths.
    #general - style violations, deprecated API usage, incomplete implementation, unnecessary dependencies, compatibility issues.
    Severity levels:
    [BLOCKER] - Must-fix issues with highest impact
    [CRITICAL] - critical issue that must be fixed as soon as possible.
    [MAJOR] - major issue that should be fixed.
    [MINOR] - minor issue that could be fixed.
    [INFO] - informational message that does not require immediate action.
    """;
}