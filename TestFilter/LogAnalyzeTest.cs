using IPFilter;

namespace TestFilter;

public class LogAnalyzeTests
{
    [Fact]
    public void Analyze_ValidData_ReturnsCorrectCount()
    {
        var logAnalyzer = new LogAnalyzer();
        var lines = new List<string>
        {
            "192.168.1.100 2024-04-01 08:15:30",
            "192.168.1.100 2024-04-01 08:17:22",
            "192.168.1.101 2024-04-01 08:18:45"
        };
        var addressStart = "192.168.1.0";
        var addressMask = "24";

        var result = logAnalyzer.Analyze(lines,  new Options 
        {
            AddressStart = addressStart,
            AddressMask = addressMask
        });

        Assert.Equal(2, result["192.168.1.100"]);
        Assert.Equal(1, result["192.168.1.101"]);
    }

    [Fact]
    public void Analyze_InvalidIPAddress_Ignored()
    {
        var logAnalyzer = new LogAnalyzer();
        var lines = new List<string> { "InvalidIPAddress 2024-04-01 08:15:30" };
        var addressStart = "192.168.1.0";
        var addressMask = "24";

        var result = logAnalyzer.Analyze(lines,  new Options 
        {
            AddressStart = addressStart,
            AddressMask = addressMask
        });

        Assert.Empty(result);
    }

    [Fact]
    public void Analyze_OutOfRangeIPAddress_Ignored()
    {
        var logAnalyzer = new LogAnalyzer();
        var lines = new List<string> { "192.168.1.30 2024-04-01 08:15:30" };
        var addressStart = "192.168.2.0";
        var addressMask = "24";

        var result = logAnalyzer.Analyze(lines, 
            new Options 
            {
                AddressStart = addressStart,
                AddressMask = addressMask
            });

        Assert.Empty(result);
    }

}