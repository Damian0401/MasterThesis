using System.Diagnostics;
using SonarQubeToSarif;

if (ConsoleHelper.TryParseArgs(args, out var parsedArgs))
{
    var clock = Stopwatch.StartNew();
    Console.WriteLine("Starting SonarQube to SARIF conversion...");
    var host = parsedArgs[ConsoleHelper.Arg.Host];
    var project = parsedArgs[ConsoleHelper.Arg.Project];
    var token = parsedArgs[ConsoleHelper.Arg.Token];
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Output, out var outputFileName);
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Issues, out var issues);
    parsedArgs.TryGetValue(ConsoleHelper.Arg.Hotspots, out var hotspots);
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        Console.WriteLine("Exiting...");
        e.Cancel = true;
        cts.Cancel();
    };
    await SonarQubeParser.ParseAsync(
        host,
        project,
        token,
        outputFileName ?? ConsoleHelper.DefaultOutputFileName,
        issues is null ? true : bool.Parse(issues),
        hotspots is null ? true : bool.Parse(hotspots),
        cts.Token);
    Console.WriteLine($"Conversion completed in {clock.Elapsed.TotalSeconds:0.####} seconds.");
}