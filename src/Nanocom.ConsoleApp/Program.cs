// SysWorx Limited

using System.CommandLine;
using System.IO.Ports;
namespace NanoCom.ConsoleApp;

internal static class Program
{
    private static async Task Main(string[] args)
    {

        var rootCommand = SetupCommands();

        _ = await rootCommand.InvokeAsync(args);
    }

    private static RootCommand SetupCommands()
    {
        RootCommand rootCommand = new("NanoCom - A simple serial port communication tool (c) SysWorx Limited 2024\r\n");

        Command portCommand = new("open", "Open a port for communication");
        Option<string> portOption = new(name: "--port", description: "The port name") { IsRequired = true };
        portOption.AddAlias("-p");
        Option<string> baudRateOption = new(name: "--baudrate", description: "The baud rate", getDefaultValue: () => "9600");
        baudRateOption.AddAlias("-b");
        Option<string> dataBitsOption = new(name: "--databits", description: "The data bits", getDefaultValue: () => "8");
        var parityOption = new Option<string>(name: "--parity", description: "The parity", getDefaultValue: () => "N").FromAmong("N", "E", "O");
        var stopBitsOption = new Option<string>(name: "--stopbits", description: "The stop bits", getDefaultValue: () => "1").FromAmong("1", "2");
        portCommand.Add(portOption);
        portCommand.Add(baudRateOption);
        portCommand.Add(dataBitsOption);
        portCommand.Add(parityOption);
        portCommand.Add(stopBitsOption);

        portCommand.SetHandler(PortCommandHandler(), portOption, baudRateOption, dataBitsOption, parityOption, stopBitsOption);

        Command listPortsCommand = new("list", "List all available ports");
        listPortsCommand.SetHandler(ListPortsCommandHandler());

        rootCommand.Add(portCommand);
        rootCommand.Add(listPortsCommand);

        return rootCommand;
    }

    private static Action ListPortsCommandHandler()
    {
        return () =>
        {

            var ports = SerialPort.GetPortNames();
            Console.WriteLine("Available ports:");
            foreach (var port in ports)
            {
                Console.WriteLine(port);
            }
        };
    }

    private static Action<string, string, string, string, string> PortCommandHandler()
    {
        return (portOptionValue, baudRateOptionValue, dataBitsOptionValue, parityOptionValue, stopBitsOptionValue) =>
        {
            WrappedComPort serialComPort = new();
            serialComPort.RegisterReceiveCallback(OnReceive);
            var portName = portOptionValue;
            var baudRate = baudRateOptionValue;
            var dataBits = dataBitsOptionValue;
            var parity = parityOptionValue;
            var stopBits = stopBitsOptionValue;
            var result = serialComPort.Open(portName, baudRate, dataBits, parity, stopBits);
            Console.WriteLine(result);
            if (serialComPort.IsOpen())
            {
                while (true)
                {
                    var input = Console.ReadLine();
                    if (input == "exit")
                    {
                        break;
                    }

                    if (input != null)
                    {
                        serialComPort.SendLine(input);
                    }
                }
            }
        };
    }

    private static void OnReceive(object? sender, string e)
    {
        Console.Write(e);
    }
}
