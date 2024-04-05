# IP Filter Program

This program analyzes an access log file containing IPv4 addresses with their request timestamps and outputs a list of IP addresses that fall within a specified range along with the count of their occurrences within a given time interval.

## Prerequisites

Before running this program, ensure you have [.NET](https://dotnet.microsoft.com/download) installed on your machine.

## How to Run

To execute the program, use the following command in the terminal or command prompt from the project's root directory:

.\IPFilter.exe --file-log "......\logfile.log" --file-output "......\outputfile.txt" --address-start "192.168.1.0" --address-mask "24" --min-time "2024-01-01T00:00:00Z" --max-time "2024-04-05T00:00:00Z"

### Optional Parameters

- `--address-start`: The lower boundary of the IP address range (default is any address).
- `--address-mask`: The subnet mask defining the upper boundary of the range as a decimal number (default is all addresses starting from the lower boundary).
- `--min-time`: The minimum timestamp from which logs will be processed.
- `--max-time`: The maximum timestamp until which logs will be processed.

## Configuration for IDEs

### Visual Studio

1. Right-click the project in Solution Explorer.
2. Select 'Properties'.
3. Navigate to 'Debug'.
4. In 'Application arguments', input your desired command line arguments.

### JetBrains Rider

1. Open the 'Run' menu and select 'Edit Configurations'.
2. Add a new .NET run configuration.
3. Under 'Program arguments', input your desired command line arguments.

### Visual Studio Code

1. Go to the 'Run and Debug' sidebar.
2. Select 'create a launch.json file'.
3. In the configurations array, add an `args` array with your desired command line arguments.

## Input File Format

The log file should contain lines with IPv4 addresses followed by the timestamp of the request, separated by a space. Each line should follow the pattern: `IP_ADDRESS TIMESTAMP`. For example:

192.168.1.100 2024-04-01T08:15:30+00:00 
192.168.1.101 2024-04-01T08:17:22+00:00

