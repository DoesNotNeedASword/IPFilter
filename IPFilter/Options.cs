using CommandLine;

namespace IPFilter;

public class Options
{
    [Option('l', "file-log", Required = true, HelpText = "Path to the log file.")]
    public string FileLog { get; set; }

    [Option('o', "file-output", Required = true, HelpText = "Path to the output file.")]
    public string FileOutput { get; set; }

    [Option('s', "address-start", Required = false, Default = null, HelpText = "The lower boundary of the IP address range. Optional, by default all addresses are processed.")]
    public string? AddressStart { get; set; }

    [Option('m', "address-mask", Required = false, Default = null, HelpText = "Subnet mask defining the upper boundary of the address range as a decimal number. Optional, if not set, all addresses starting from the lower boundary are processed. Cannot be used without address-start.")]
    public string? AddressMask { get; set; }
    [Option("min-time", Required = false, HelpText = "Minimum log time to process. Optional, if not set, all times are processed.")]
    public DateTimeOffset? MinTime { get; set; }

    [Option("max-time", Required = false, HelpText = "Maximum log time to process. Optional, if not set, all times are processed.")]
    public DateTimeOffset? MaxTime { get; set; }
}