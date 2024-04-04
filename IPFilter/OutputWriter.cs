using System.Collections.Concurrent;

namespace IPFilter;

public class OutputWriter
{
    public static async Task WriteResults(string outputPath, Dictionary<string, int> results)
    {
        try
        {
            await using var file = new StreamWriter(outputPath);
            foreach (var entry in results)
            {
                await file.WriteLineAsync($"{entry.Key} {entry.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while writing to file: {ex.Message}");
        }
    }
}