namespace IPFilter;


public interface ILogReader
{
    IAsyncEnumerable<string> ReadLinesAsync(string path);
}