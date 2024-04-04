using CommandLine;
using IPFilter;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(opts =>
    {
        try
        {
            var analyzer = new LogAnalyzer(opts);
            var results = analyzer.Analyze();
            OutputWriter.WriteResults(opts.FileOutput, results);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    });