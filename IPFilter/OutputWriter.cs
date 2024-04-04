using System.Collections.Concurrent;

namespace IPFilter;

public class OutputWriter
{
    public static void WriteResults(string outputPath, ConcurrentDictionary<string, int> results)
    {
        try
        {
            using var file = new StreamWriter(outputPath);
            foreach (var entry in results)
            {
                file.WriteLine($"{entry.Key} {entry.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while writing to file: {ex.Message}");
        }
    }
}