using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using SocketHandler;

public class Tests
{
    const int PORT = 9998;

    static UDPController udpClient = null;

    static void Main(string[] args)
    {
        SocketHandler.Server server = new SocketHandler.Server(PORT);
        server.onNewConnection += OnConnected;
        
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = null;
        foreach (IPAddress addr in ipHostInfo.AddressList)
        {
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                ipAddress = addr;
                break;
            }
        }
        Client client = new Client(ipAddress, PORT);
        UDPController udpServer = new UDPController(client.socket);
        udpServer.onReceiveData += OnServerReceived;

        udpServer.SendData(Encoding.Unicode.GetBytes("hello world"));

        string message;
        bool running = true;
        while (running)
        {
            message = Console.ReadLine();
            switch (message)
            {
                case "q":
                    running = false;
                    break;
            }
        }

        server.Stop();
        client.Stop();
        udpServer.Stop();
        udpClient.Stop();
    }

    static void OnConnected(Socket socket)
    {
        udpClient = new UDPController(socket);
        udpClient.onReceiveData += (message) => OnClientReceived(message, udpClient);
    }

    static void OnServerReceived(byte[] message)
    {
        Console.WriteLine("Test Server received message: " + Encoding.Unicode.GetString(message));
    }

    static void OnClientReceived(byte[] message, UDPController controller)
    {
        Console.WriteLine("Test Client received message: " + Encoding.Unicode.GetString(message));
        controller.SendData(message);
    }
}
