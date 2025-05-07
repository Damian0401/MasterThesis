namespace ProjectAnalyzer;

internal class ConsoleHelper
{
    private static readonly IDictionary<string, Arg> ArgsMap = new Dictionary<string, Arg>(StringComparer.OrdinalIgnoreCase)
    {
        ["--url"] = Arg.Url,
        ["-u"] = Arg.Url,
        ["--models"] = Arg.Models,
        ["-m"] = Arg.Models,
        ["--token"] = Arg.Token,
        ["-t"] = Arg.Token,
        ["--output"] = Arg.Output,
        ["-o"] = Arg.Output,
        ["--extensions"] = Arg.Extensions,
        ["-e"] = Arg.Extensions,
        ["--skip"] = Arg.Skip,
        ["-s"] = Arg.Skip,
    };
    private static readonly Arg[] RequiredArgs = [Arg.Models, Arg.Token];
    private const string HelpArg = "--help";
    private const string HelpText = """
    Tool used to analyze a project using selected model and generate SARIF report.
    Usage:
      ProjectAnalizer [--help] --model <model> --token <token> [--endpoint <endpoint>] [--output <output_file>]

    Arguments:
      --help            Display help message.
      --models, -m      Specify the list of models to use for analysis (required).
      --token, -t       Specify the token to use for authentication.
      --url, -u         Specify the endpoint URL (optional, default: https://models.github.ai/inference).
      --output, -o      Specify the output file name (optional, default: output.sarif).
      --extensions, -e  Specify the file extensions to analyze (optional, default: cs,sln,slnx,csproj,json).
      --skip, -s        Specify the directories to skip (optional, default: bin,obj).
    """;
    internal const string DefaultOutputFileName = "output.sarif";
    internal const string DefaultEndpoint = "https://models.github.ai/inference";
    internal static readonly IReadOnlyCollection<string> DefaultExtensions = ["cs", "sln", "slnx", "csproj", "json"];
    internal static readonly IReadOnlyCollection<string> DefaultSkip = ["bin", "obj"];
    internal static bool TryParseArgs(string[] args, out IDictionary<Arg, string> parsedArgs)
    {
        parsedArgs = new Dictionary<Arg, string>();
        if (args.Contains(HelpArg))
        {
            Console.WriteLine(HelpText);
            return false;
        }

        if (args.Length == 0 || args.Length % 2 != 0)
        {
            Console.Error.WriteLine("Invalid number of arguments.");
            return false;
        }

        foreach (var arg in args.Chunk(2))
        {
            if (ArgsMap.TryGetValue(arg[0], out var argKey))
            {
                parsedArgs[argKey] = arg[1];
                continue;
            };
            Console.Error.WriteLine($"Invalid argument: {arg[0]}.");
            return false;
        }

        foreach (var requiredArg in RequiredArgs)
        {
            if (!parsedArgs.ContainsKey(requiredArg))
            {
                Console.Error.WriteLine($"Missing required argument: {requiredArg}");
                return false;
            }
        }

        return true;
    }

    internal enum Arg
    {
        Url,
        Models,
        Token,
        Output,
        Extensions,
        Skip,
    }
}