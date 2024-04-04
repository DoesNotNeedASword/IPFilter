namespace IPFilter;

public class LogReader(string filePath)
{
    public async IAsyncEnumerable<string> ReadLinesAsync()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Log file not found.");
            yield break;
        }

        await foreach (var line in File.ReadLinesAsync(filePath))
        {
            yield return line;
        }
    }
}