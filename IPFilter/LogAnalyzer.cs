using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;

namespace IPFilter;

public class LogAnalyzer(Options options, ILogReader reader)
{
    public async Task<Dictionary<string, int>> AnalyzeAsync()
    {
        var results = new Dictionary<string, int>();
        try
        {
            await foreach (var line in reader.ReadLinesAsync(options.FileLog).ConfigureAwait(false))
            {
                if (!TryParseLine(line, out var ipAddress, out var logTime))
                    continue;

                if (!InRange(ipAddress, logTime))
                    continue;

                if (!results.TryAdd(ipAddress.ToString(), 1))
                    results[ipAddress.ToString()]++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return results;
    }

    private bool TryParseLine(string line, out IPAddress ipAddress, out DateTime logTime)
    {
        var lineSpan = line.AsSpan();
        var spaceIndex = lineSpan.IndexOf(' ');
        if (spaceIndex == -1)
        {
            ipAddress = null;
            logTime = default;
            return false;
        }

        var ipSpan = lineSpan[..spaceIndex];
        var dateSpan = lineSpan[(spaceIndex + 1)..];

        if (IPAddress.TryParse(ipSpan.ToString(), out ipAddress) &&
            DateTime.TryParseExact(dateSpan.ToString(), "yyyy-MM-ddTHH:mm:ssK",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out logTime)) return true;
        logTime = default;
        return false;

    }
    private bool InRange(IPAddress ipAddress, DateTime logTime)
    {
        if (string.IsNullOrEmpty(options.AddressStart))
            return true;

        if (!IPAddress.TryParse(options.AddressStart, out var startAddress))
        {
            Console.WriteLine($"Invalid start address: {options.AddressStart}");
            return false;
        }

        var maskLength = 32; 
        if (!string.IsNullOrEmpty(options.AddressMask))
        {
            if (!int.TryParse(options.AddressMask, out maskLength) || maskLength < 0 || maskLength > 32)
            {
                Console.WriteLine($"Invalid mask: {options.AddressMask}");
                return false;
            }
        }

        var mask = maskLength == 0 ? 0 : ~((uint)1 << (32 - maskLength)) + 1;

        var ipAsUint = BinaryPrimitives.ReadUInt32BigEndian(ipAddress.GetAddressBytes());
        var startAsUint = BinaryPrimitives.ReadUInt32BigEndian(startAddress.GetAddressBytes());

        var isInIpRange = (ipAsUint & mask) >= (startAsUint & mask);
    
        var isInTimeRange = (!options.MinTime.HasValue || logTime >= options.MinTime.Value) &&
                            (!options.MaxTime.HasValue || logTime <= options.MaxTime.Value);

        return isInIpRange && isInTimeRange;
    }
}