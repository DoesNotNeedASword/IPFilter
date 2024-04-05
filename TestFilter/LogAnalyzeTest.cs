using System.Globalization;
using IPFilter;
using Moq;

namespace TestFilter;

public class LogAnalyzeTests
{
    [Fact]
    public async Task AnalyzeAsync_ValidData_ReturnsCorrectCounts()
    {
        var mockLogReader = new Mock<ILogReader>();
        var options = CreateTestOptions();
        mockLogReader.Setup(reader => reader.ReadLinesAsync(It.IsAny<string>()))
            .Returns(GetTestLines());

        var analyzer = new LogAnalyzer(options, mockLogReader.Object);
        
        var results = await analyzer.AnalyzeAsync();
        
        Assert.Equal(2, results["192.168.1.100"]);
    }

   [Fact]
    public async Task AnalyzeAsync_InvalidIPAddress_Ignored()
    {
        var mockLogReader = CreateMockLogReader(new[] { "invalid-ip 2024-01-01T08:15:30+00:00" });
        var analyzer = new LogAnalyzer(CreateTestOptions(), mockLogReader.Object);

        var results = await analyzer.AnalyzeAsync();

        Assert.Empty(results);
    }

    [Fact]
    public async Task AnalyzeAsync_OutOfTimeRange_Ignored()
    {
        var mockLogReader = CreateMockLogReader(new[] { "192.168.1.100 2023-12-31T08:15:30+00:00" });
        var analyzer = new LogAnalyzer(CreateTestOptions(), mockLogReader.Object);

        var results = await analyzer.AnalyzeAsync();

        Assert.Empty(results);
    }

    [Fact]
    public async Task AnalyzeAsync_EmptyLines_Ignored()
    {
        var mockLogReader = CreateMockLogReader(new[] { "", " " });
        var analyzer = new LogAnalyzer(CreateTestOptions(), mockLogReader.Object);

        var results = await analyzer.AnalyzeAsync();

        Assert.Empty(results);
    }

    private static Options CreateTestOptions()
    {
        return new Options
        {
            FileLog = @"..\..\..\logfile.log",
            FileOutput = @"..\..\..\outputfile.txt",
            AddressStart = "192.168.1.0",
            AddressMask = "24",
            MinTime = DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
            MaxTime = DateTime.Parse("2024-04-05T00:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
        };
    }

    private static Mock<ILogReader> CreateMockLogReader(IEnumerable<string> lines)
    {
        var mockLogReader = new Mock<ILogReader>();
        mockLogReader.Setup(reader => reader.ReadLinesAsync(It.IsAny<string>()))
            .Returns(GetTestLines(lines));

        return mockLogReader;
    }

    private static async IAsyncEnumerable<string> GetTestLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            yield return line;
        }
    }
    private static async IAsyncEnumerable<string> GetTestLines()
    {
        yield return "192.168.1.100 2024-01-01T08:15:30+00:00";
        yield return "192.168.1.100 2024-01-01T09:00:00+00:00";
    }
}



