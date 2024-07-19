// SysWorx Limited

using System.Diagnostics;
using System.IO.Ports;

namespace NanoCom.ConsoleApp;

public class WrappedComPort : IDisposable
{
    private readonly SerialPort comPort;

    // Constructor
    public WrappedComPort()
    {
        comPort = new SerialPort();
    }

    // Implement IDisposable
    public void Dispose()
    {
        Close();

        GC.SuppressFinalize(this);
    }

    // Ensure proper cleanup
    ~WrappedComPort()
    {
        Dispose();
    }

    // User must register function to call when a line of text terminated by \n has been received
    public event EventHandler<string>? MessageReceived;

    public void RegisterReceiveCallback(EventHandler<string> callback)
    {
        MessageReceived += callback;
    }

    public void DeregisterReceiveCallback(EventHandler<string> callback)
    {
        MessageReceived -= callback;
    }

    // Example method to trigger the event
    protected virtual void OnMessageReceived(string message)
    {
        MessageReceived?.Invoke(this, message);
    }

    private void Close()
    {
        // Implement the logic to close the SerialPort
        if (comPort.IsOpen)
        {
            comPort.Close();
        }
    }
    public void SendLine(string aString)
    {
        try
        {
            if (comPort.IsOpen)
            {
                comPort.Write(aString);
            }
        }
        catch (Exception exp)
        {
            Debug.Print(exp.Message);
        }
    }

    public string Open(string portName, string baudRate, string dataBits, string parity, string stopBits)
    {
        try
        {
            comPort.WriteBufferSize = 4096;
            comPort.ReadBufferSize = 4096;
            comPort.WriteTimeout = 500;
            comPort.ReadTimeout = 500;
            comPort.DtrEnable = true;
            comPort.Handshake = Handshake.None;
            comPort.PortName = portName.TrimEnd();
            comPort.BaudRate = Convert.ToInt32(baudRate);
            comPort.DataBits = Convert.ToInt32(dataBits);
            switch (parity)
            {
                case "N":
                    comPort.Parity = Parity.None;
                    break;
                case "E":
                    comPort.Parity = Parity.Even;
                    break;
                case "O":
                    comPort.Parity = Parity.Odd;
                    break;
            }
            switch (stopBits)
            {
                case "1":
                    comPort.StopBits = StopBits.One;
                    break;
                case "2":
                    comPort.StopBits = StopBits.Two;
                    break;
            }
            comPort.Open();
            comPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        catch (Exception error)
        {
            return error.Message + "\r\n";
        }
        return comPort.IsOpen
            ? $"{comPort.PortName} Opened | Baud {comPort.BaudRate} | Data Bits {comPort.DataBits} | Parity {comPort.Parity} | Stop Bits {comPort.StopBits}"
            : $"{comPort.PortName}  Open Failed \r\n";
    }

    public bool IsOpen()
    {
        return comPort.IsOpen;
    }

    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        if (!comPort.IsOpen)
        {
            return;
        }

        try
        {
            var indata = comPort.ReadLine();
            indata += "\n";
            OnMessageReceived(indata);
        }
        catch (Exception error)
        {
            Debug.Print(error.Message);
        }
    }
}