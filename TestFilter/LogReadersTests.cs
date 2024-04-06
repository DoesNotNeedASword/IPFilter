using IPFilter;

namespace TestFilter;

public class LogReaderTests
{
    [Fact]
    public async Task ReadLinesAsync_WithRealFile_ReadsCorrectly()
    {
        const string testFilePath = @"..\..\..\logfile1.log";

        File.WriteAllLines(testFilePath, [
            "192.168.1.100 2024-01-01T08:15:30+00:00",
            "192.168.1.101 2024-01-02T08:15:30+00:00"
        ]);

        var logReader = new LogReader(); 

        var lines = new List<string>();
        await foreach (var line in logReader.ReadLinesAsync(testFilePath))
        {
            lines.Add(line);
        }

        Assert.Equal(2, lines.Count); 
        Assert.Equal("192.168.1.100 2024-01-01T08:15:30+00:00", lines[0]);
        Assert.Equal("192.168.1.101 2024-01-02T08:15:30+00:00", lines[1]);

        File.Delete(testFilePath);
    }
}