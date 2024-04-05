namespace IPFilter;

public class LogReader : ILogReader
{
    public async IAsyncEnumerable<string> ReadLinesAsync(string path)
    {
        await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
        using var streamReader = new StreamReader(fileStream);

        while (await streamReader.ReadLineAsync() is { } line)
        {
            yield return line;
        }
    }
}