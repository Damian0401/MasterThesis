namespace SonarQubeToSarif;

internal class ConsoleHelper
{
    private static readonly IDictionary<string, Arg> ArgsMap = new Dictionary<string, Arg>(StringComparer.OrdinalIgnoreCase)
    {
        ["--host"] = Arg.Host,
        ["-h"] = Arg.Host,
        ["--project"] = Arg.Project,
        ["-p"] = Arg.Project,
        ["--token"] = Arg.Token,
        ["-t"] = Arg.Token,
        ["--output"] = Arg.Output,
        ["-o"] = Arg.Output,
    };
    private static readonly Arg[] RequiredArgs = [Arg.Host, Arg.Project, Arg.Token];
    private const string HelpArg = "--help";
    private const string HelpText = """
    Tool used to transform SonarQube report to SARIF format.
    Usage:
      SonarQubeToSarif [--help] --host <host> --project <project> [--output <output_file>]

    Arguments:
      --help           Display help message.
      --host, -h       Specify the host to connect to.
      --project, -p    Specify the project to use.
      --token, -t      Specify the token to use for authentication.
      --output, -o     Specify the output file name (optional, default: output.sarif).
    """;
    internal const string DefaultOutputFileName = "output.sarif";
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
        Help,
        Host,
        Project,
        Token,
        Output,
    }
}