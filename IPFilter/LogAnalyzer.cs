using System.Collections.Concurrent;
using System.Globalization;
using System.Net;

namespace IPFilter;

public class LogAnalyzer
{
    public ConcurrentDictionary<string, int> Analyze(IEnumerable<string> lines, Options options)
    {
        var results = new ConcurrentDictionary<string, int>();

        Parallel.ForEach(lines, (line) =>
        {
            var parts = line.Split(new[] { ' ' }, 2); // Используйте перегрузку Split для разделения только на две части
            if (parts.Length < 2) return; // Проверка, что строка содержит и IP, и дату

            var ipString = parts[0];
            if (!IPAddress.TryParse(ipString, out var ipAddress)) return;

            // dateString содержит полную строку даты и времени в формате ISO 8601
            var dateString = parts[1];
            if (!DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ssK",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var logTime)) return;

            if (!InRange(ipAddress, logTime, options)) return;

            results.AddOrUpdate(ipString, 1, (key, oldValue) => oldValue + 1);
        });

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

        var ipAsUint = IpAddressToUInt32(ipAddress);
        var startAsUint = IpAddressToUInt32(startAddress);

        bool isInIpRange = (ipAsUint & mask) >= (startAsUint & mask);
    
        bool isInTimeRange = (!options.MinTime.HasValue || logTime >= options.MinTime.Value) &&
                             (!options.MaxTime.HasValue || logTime <= options.MaxTime.Value);

        return isInIpRange && isInTimeRange;
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