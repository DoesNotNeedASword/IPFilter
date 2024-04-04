namespace IPFilter;

public class LogReader(string filePath)
{
    public IEnumerable<string> ReadLines()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Log file not found.");
            yield break;
        }

        foreach (var line in File.ReadLines(filePath))
        {
            yield return line;
        }
    }
}