using CommandLine;
using IPFilter;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        try
        {
            var reader = new LogReader();
            var analyzer = new LogAnalyzer(options, reader);

            var results = analyzer.AnalyzeAsync().Result;
            OutputWriter.WriteResults(options.FileOutput, results);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    });