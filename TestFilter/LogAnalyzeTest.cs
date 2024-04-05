using System.Globalization;
using IPFilter;

namespace TestFilter;

public class LogAnalyzeTests
{
    [Fact]
    public void Analyze_WithinTimeRangeAndIPRange_ShouldCountOccurrences()
    {
        var options = new Options
        {
            AddressStart = "192.168.1.0",
            AddressMask = "24",
            MinTime = DateTime.ParseExact("2024-01-01T00:00:00+00:00", "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture),
            MaxTime = DateTime.ParseExact("2024-01-02T00:00:00+00:00", "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture)
        };
        var logAnalyzer = new LogAnalyzer();
        var lines = new List<string>
        {
            "192.168.1.100 2024-01-01T08:15:30+00:00",
            "192.168.1.100 2024-01-01T09:00:00+00:00",
            "192.168.1.101 2024-01-01T10:00:00+00:00"
        };

        var result = logAnalyzer.Analyze(lines, options);

        Assert.Equal(2, result["192.168.1.100"]);
        Assert.Equal(1, result["192.168.1.101"]);
    }

    [Fact]
    public void Analyze_OutsideTimeRange_ShouldIgnore()
    {
        // Arrange
        var options = new Options
        {
            AddressStart = "192.168.1.0",
            AddressMask = "24",
            MinTime = DateTime.ParseExact("2024-01-01T00:00:00+00:00", "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture),
            MaxTime = DateTime.ParseExact("2024-01-02T00:00:00+00:00", "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture)
        };
        var logAnalyzer = new LogAnalyzer();
        var lines = new List<string>
        {
            "192.168.1.100 2024-01-03T08:15:30+00:00", 
            "192.168.1.101 2024-01-03T09:00:00+00:00"  
        };
        var result = logAnalyzer.Analyze(lines, options);

        Assert.Empty(result);
    }

    [Fact]
    public void Analyze_SpecificTime_ShouldInclude()
    {
        var options = new Options
        {
            AddressStart = "192.168.1.0",
            AddressMask = "24",
            MinTime = DateTime.ParseExact("2024-01-01T08:00:00+00:00", "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture),
            MaxTime = DateTime.ParseExact("2024-01-01T09:00:00+00:00", "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture)
        };
        var logAnalyzer = new LogAnalyzer();
        var lines = new List<string>
        {
            "192.168.1.100 2024-01-01T08:15:30+00:00", 
            "192.168.1.101 2024-01-01T08:45:00+00:00"  
        };

        var result = logAnalyzer.Analyze(lines, options);

        Assert.Equal(1, result["192.168.1.100"]);
        Assert.Equal(1, result["192.168.1.101"]);
    }
}