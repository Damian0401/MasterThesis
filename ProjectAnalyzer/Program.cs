using System.Diagnostics;
using ProjectAnalizer;

if (ConsoleHelper.TryParseArgs(args, out var parsedArgs))
{
    var clock = Stopwatch.StartNew();
    Console.WriteLine("Starting project analyze...");
    var models = parsedArgs[ConsoleHelper.Arg.Models];
    var token = parsedArgs[ConsoleHelper.Arg.Token];
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Url, out var endpoint);
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Output, out var outputFileName);
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Extensions, out var extensions);
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Skip, out var skip);
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        Console.WriteLine("Exiting...");
        e.Cancel = true;
        cts.Cancel();
    };
    await ProjectAnalyzer.AnalyzeAsync(
        models.Split(","),
        token,
        endpoint ?? ConsoleHelper.DefaultEndpoint,
        outputFileName ?? ConsoleHelper.DefaultOutputFileName,
        extensions?.Split(",") ?? ConsoleHelper.DefaultExtensions,
        skip?.Split(",") ?? ConsoleHelper.DefaultSkip,
        cts.Token);
    Console.WriteLine($"SARIF raport generated in {clock.Elapsed.TotalSeconds:0.####} seconds.");
}