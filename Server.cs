using System;
using System.Net.Sockets;
using System.Collections.Generic;

public class Server
{
    const int PORT = 5287;
    const string HELP_MESSAGE = @"h/help - help
q - quit (shutdown server)
live - how many users are online right now

";

    static int usersOnline = 0;
    static List<SocketHandler.Controller> users = new List<SocketHandler.Controller>();
    static Dictionary<string, Action<SocketHandler.Controller, string[]>> routes = 
        new Dictionary<string, Action<SocketHandler.Controller, string[]>>()
    {
        { "queue", QueueUser },
    };

    static void Main(string[] args)
    {
        SocketHandler.Server servSocket = new SocketHandler.Server(5287);
        servSocket.onNewConnection += StartNewSession;

        bool running = true;
        servSocket.onCloseConnection += (e) =>
        {
            if (e != null)
            {
                Console.WriteLine("Server crashed:");
                Console.WriteLine(e);
                running = false;
            }
        };

        Console.WriteLine(HELP_MESSAGE);

        string message;
        while (running)
        {
            Console.Write(">>> ");
            message = Console.ReadLine();
            if (message == "q")
            {
                running = false;
                servSocket.Stop();
            }
            else if (message == "h" || message == "help")
            {
                Console.Write(HELP_MESSAGE);
            }
            else if (message == "live")
            {
                Console.WriteLine(usersOnline);
            }
        }
    }

    private static void StartNewSession(Socket socket)
    {
        SocketHandler.Controller controller = new SocketHandler.Controller(socket);
        controller.onCloseConnection += (e) => LogOut(e, controller);
        controller.onReceiveData += (s) => ProcessMessage(s, controller);
        users.Add(controller);
    }

    private static void LogOut(Exception e, SocketHandler.Controller controller)
    {
        users.Remove(controller);
    }

    private static void ProcessMessage(string message, SocketHandler.Controller controller)
    {
        string[] args = message.Split(' ');
        routes[args[0]](controller, args);
    }

    static void QueueUser(SocketHandler.Controller controller, string[] args)
    {

    }
}
