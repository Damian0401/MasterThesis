using System.Diagnostics;
using SonarQubeToSarif;

var clock = Stopwatch.StartNew();
Console.WriteLine("Starting SonarQube to SARIF conversion...");
if (ConsoleHelper.TryParseArgs(args, out var parsedArgs))
{
    var host = parsedArgs[ConsoleHelper.Arg.Host];
    var project = parsedArgs[ConsoleHelper.Arg.Project];
    var token = parsedArgs[ConsoleHelper.Arg.Token];
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Output, out var outputFileName);
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        Console.WriteLine("Exiting...");
        e.Cancel = true;
        cts.Cancel();
    };
    await SonarQubeParser.Parse(
        host,
        project,
        token,
        outputFileName ?? ConsoleHelper.DefaultOutputFileName,
        cts.Token);
}
Console.WriteLine($"Conversion completed in {clock.Elapsed.TotalSeconds:0.####} seconds.");