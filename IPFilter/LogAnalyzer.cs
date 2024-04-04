using System.Collections.Concurrent;
using System.Net;

namespace IPFilter;

public class LogAnalyzer(Options options)
{
    public ConcurrentDictionary<string, int> Analyze()
    {
        var results = new ConcurrentDictionary<string, int>();

        try
        {
            if (!File.Exists(options.FileLog))
            {
                Console.WriteLine("Log file not found.");
                return results;
            }

            Parallel.ForEach(File.ReadLines(options.FileLog), (line) =>
            {
                var parts = line.Split(' ');
                if (parts.Length < 2) return;

                var ipString = parts[0];
                if (!IPAddress.TryParse(ipString, out var ipAddress)) return;
            
                if (!InRange(ipAddress, options.AddressStart, options.AddressMask)) return;
            
                results.AddOrUpdate(ipString, 1, (key, oldValue) => oldValue + 1);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the log file: {ex.Message}");
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