using System.Collections.Concurrent;
using System.Net;

namespace IPFilter;

public class LogAnalyzer()
{
    public ConcurrentDictionary<string, int> Analyze(IEnumerable<string>? lines, string addressStart, string addressMask)
    {
        var results = new ConcurrentDictionary<string, int>();

        Parallel.ForEach(lines, (line) =>
        {
            var parts = line.Split(' ');
            if (parts.Length < 2) return;

            var ipString = parts[0];
            if (!IPAddress.TryParse(ipString, out var ipAddress)) return;

            if (!InRange(ipAddress, addressStart, addressMask)) return;

            results.AddOrUpdate(ipString, 1, (key, oldValue) => oldValue + 1);
        });

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

        var ipAsUint = IpAddressToUInt32(ipAddress);
        var startAsUint = IpAddressToUInt32(startAddress);

        return (ipAsUint & mask) >= (startAsUint & mask);
    }
    
    private uint IpAddressToUInt32(IPAddress ipAddress)
    {
        var addressBytes = ipAddress.GetAddressBytes();

        // Перевернуть порядок байтов для little-endian систем
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(addressBytes);
        }

        return BitConverter.ToUInt32(addressBytes, 0);
    }
}