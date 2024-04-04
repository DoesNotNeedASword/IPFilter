using CommandLine;
using IPFilter;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        try
        {
            var reader = new LogReader(options.FileLog);
            var analyzer = new LogAnalyzer();

            var lines = reader.ReadLines();
            var results = analyzer.Analyze(lines, options.AddressStart, options.AddressMask);
            OutputWriter.WriteResults(options.FileOutput, results);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    });