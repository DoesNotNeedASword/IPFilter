using System.Collections.Concurrent;
using System.Net;

namespace IPFilter;

public class LogAnalyzer(Options options)
{
    public async Task<Dictionary<string, int>> AnalyzeAsync(IAsyncEnumerable<string> lines)
    {
        var results = new Dictionary<string, int>();

        await foreach(var line in lines)
        {
            var parts = line.Split(' ');
            if (parts.Length < 2) continue;

            var ipString = parts[0];
            if (!IPAddress.TryParse(ipString, out var ipAddress)) continue;

            if (!InRange(ipAddress, options.AddressStart, options.AddressMask)) continue;

            lock (results)
            {
                if (results.TryGetValue(ipString, out var count))
                    results[ipString] = count + 1;
                else
                    results.TryAdd(ipString, 1);
            }
        }

        return results;
    }


    private bool InRange(IPAddress ipAddress, string addressStart, string addressMask)
    {
        if (string.IsNullOrEmpty(addressStart))
            return true;

        if (!IPAddress.TryParse(addressStart, out var startAddress))
        {
            Console.WriteLine($"Invalid start address: {addressStart}");
            return false;
        }

        var maskLength = 32; 
        if (!string.IsNullOrEmpty(addressMask))
        {
            if (!int.TryParse(addressMask, out maskLength) || maskLength < 0 || maskLength > 32)
            {
                Console.WriteLine($"Invalid mask: {addressMask}");
                return false;
            }
        }

        var mask = maskLength == 0 ? 0 : ~((uint)1 << (32 - maskLength)) + 1;

        var ipAsUint = BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);
        var startAsUint = BitConverter.ToUInt32(startAddress.GetAddressBytes(), 0);

        return (ipAsUint & mask) >= (startAsUint & mask);
    }
}