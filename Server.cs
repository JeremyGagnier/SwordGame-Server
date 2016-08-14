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
    static GameQueue gameQueue = new GameQueue();
    static SocketHandler.UDPServer udpServer;

    /// <summary>
    /// Routes is used for matching messages to functions.
    /// </summary>
    static Dictionary<string, Action<User, string[]>> routes = 
        new Dictionary<string, Action<User, string[]>>()
    {
        { "queue", QueueUser },
        { "dequeue", DequeueUser },
        { "disconnect", Disconnect },
        { "name", SetName },
        { "g", GetFrame }
    };

    static void Main(string[] args)
    {
        SocketHandler.Server servSocket = new SocketHandler.Server(PORT);
        udpServer = new SocketHandler.UDPServer(PORT);
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
            message = Console.ReadLine();
            switch(message)
            {
                case "q":
                case "quit":
                    running = false;
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

        for(int i = 0; i < users.Count; ++i)
        {
            users[i].LogOut();
        }
        servSocket.Stop();
        udpServer.Stop();
    }

    private static void StartNewSession(Socket socket)
    {
        User newUser = new User(socket, udpServer);
        users.Add(newUser);
        udpServer.ListenToEndPoint((IPEndPoint)socket.RemoteEndPoint, newUser.HandleGameMessage);
    }

    public static void LogOut(Exception e, User user)
    {
        users.Remove(user);
        gameQueue.Remove(user);
        if (user.game != null)
        {
            user.game.LeaveGame(user);
        }
        user.LogOut();
    }

    public static void ProcessMessage(string message, User user)
    {
        string[] args = message.Split(' ');
        routes[args[0]](user, args);
    }

    private static void QueueUser(User user, string[] args)
    {
        List<int> validPlayerCounts = new List<int>();
        foreach (string s in args[1].Split(','))
        {
            validPlayerCounts.Add(Convert.ToInt32(s));
        }
        user.Queue(gameQueue, validPlayerCounts);
        Game game = gameQueue.TryFormGame();
        if (game != null)
        {
            games.Add(game);
        }
    }

    private static void DequeueUser(User user, string[] args)
    {
        user.Dequeue(gameQueue);
    }

    private static void Disconnect(User user, string[] args)
    {
        LogOut(null, user);
    }

    private static void SetName(User user, string[] args)
    {
        user.name = args[1];
    }

    private static void GetFrame(User user, string[] args)
    {
        user.game.GetFrame(user, Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
    }

    public static void EndGame(Game game)
    {
        games.Remove(game);
    }
}
