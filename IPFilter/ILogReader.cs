namespace IPFilter;


public interface ILogReader
{
    IEnumerable<string> ReadLines(string path);
    IAsyncEnumerable<string> ReadLinesAsync(string path);
}