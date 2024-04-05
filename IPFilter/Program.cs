using CommandLine;
using IPFilter;

Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync(async options =>
    {
        try
        {
            var reader = new LogReader();
            var analyzer = new LogAnalyzer(options, reader);
            var writer = new OutputWriter(options.FileOutput);

            var results = analyzer.AnalyzeAsync().GetAwaiter().GetResult();
            await writer.WriteResults(results);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    });