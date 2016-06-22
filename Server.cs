using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class Server
{
    const int PORT = 5287;
    const string DNS = "progressiongames.servegame.org";
    const string HELP_MESSAGE = @"
h/help - help
q/quit - quit (shutdown server)
live - how many users are online right now
dnstest - check the current IP of " + DNS + @"
";

    static List<User> users = new List<User>();
    static List<Game> games = new List<Game>();
    static List<User> gameQueue = new List<User>();
    
    /// <summary>
    /// Routes is used for matching messages to functions based on the first word.
    /// </summary>
    static Dictionary<string, Action<User, string[]>> routes = 
        new Dictionary<string, Action<User, string[]>>()
    {
        { "queue", QueueUser },
        { "disconnect", Disconnect },
        { "g", GameMessage }
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

        Console.WriteLine();
        Console.WriteLine(HELP_MESSAGE);

        string message;
        while (running)
        {
            Console.Write(">>> ");
            message = Console.ReadLine();
            switch(message)
            {
                case "q":
                case "quit":
                    running = false;
                    servSocket.Stop();
                    break;
                case "h":
                case "help":
                    Console.WriteLine(HELP_MESSAGE);
                    break;
                case "live":
                    Console.WriteLine(users.Count);
                    break;
                case "dnstest":
                    Console.WriteLine("" + Dns.GetHostAddresses(DNS)[0]);
                    break;
                default:
                    Console.WriteLine(message + " is not a valid command");
                    break;
            }
        }
        foreach (User user in users)
        {
            user.LogOut();
        }
    }

    private static void StartNewSession(Socket socket)
    {
        users.Add(new User(socket));
    }

    public static void LogOut(Exception e, User user)
    {
        users.Remove(user);
        user.LogOut();
        gameQueue.Remove(user);
    }

    public static void ProcessMessage(string message, User user)
    {
        string[] args = message.Split(' ');
        routes[args[0]](user, args);
    }

    static void QueueUser(User user, string[] args)
    {
        if (gameQueue.Count < 4) {
            gameQueue.Add(user);
        } else {
            List<User> players = gameQueue;
            gameQueue = new List<User>();
            players.Add(user);
            games.Add(new Game(players));
        }
    }

    static void Disconnect(User user, string[] args)
    {
        LogOut(null, user);
    }

    static void GameMessage(User user, string[] args)
    {
        user.game.GameMessage(user, args);
    }
}
