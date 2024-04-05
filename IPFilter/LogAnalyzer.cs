using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;

namespace IPFilter;

public class LogAnalyzer(Options options, ILogReader reader)
{
    public async Task<ConcurrentDictionary<string, int>> AnalyzeAsync()
    {
        var results = new ConcurrentDictionary<string, int>();
        await foreach (var line in reader.ReadLinesAsync(options.FileLog))
        {
            var spaceIndex = line.IndexOf(' ');
            if (spaceIndex == -1) continue;

            var ipString = line[..spaceIndex];
            var dateString = line[(spaceIndex + 1)..];

            if (!IPAddress.TryParse(ipString, out var ipAddress)) continue;

            if (!DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ssK",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var logTime)) continue;

            if (!InRange(ipAddress, logTime, options)) continue;

            results.AddOrUpdate(ipString, 1, (key, oldValue) => oldValue + 1);
        }

        return results;
    }
    private bool InRange(IPAddress ipAddress, DateTime logTime, Options options)
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