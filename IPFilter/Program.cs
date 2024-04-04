using CommandLine;
using IPFilter;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(async opts =>
    {
        try
        {
            var reader = new LogReader(opts.FileLog);
            var analyzer = new LogAnalyzer(opts);
            var results = await analyzer.AnalyzeAsync(reader.ReadLinesAsync());
            await OutputWriter.WriteResults(opts.FileOutput, results);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    });