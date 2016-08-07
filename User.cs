using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class User
{
    // Send several UDP packets and hope that one gets through!
    public const int UDP_SEND_COUNT = 5;

    private SocketHandler.UDPServer udpServer = null;
    private SocketHandler.Controller controller = null;
    private IPEndPoint endpoint;

    public Game game = null;
    public int playerNum = 0;
    public string name = "";

    private bool isInQueue = false;

    public User(Socket socket, SocketHandler.UDPServer udpServer)
    {
        this.udpServer = udpServer;
        endpoint = (IPEndPoint)socket.RemoteEndPoint;
        controller = new SocketHandler.Controller(socket);
        controller.onCloseConnection += (e) => Server.LogOut(e, this);
        controller.onReceiveData += (s) => Server.ProcessMessage(s, this);
    }

    public void LogOut()
    {
        controller.Stop();
    }

    public void HandleGameMessage(byte[] message)
    {
        game.GameMessage(this, message);
    }

    public void SendGameMessage(byte[] message)
    {
        for (int i = 0; i < UDP_SEND_COUNT; ++i)
        {
            udpServer.SendData(endpoint, message);
        }
    }

    public void Queue(GameQueue queue, List<int> modes)
    {
        if (isInQueue) return;

        isInQueue = true;
        queue.Add(this, modes);
    }

    public void Dequeue(GameQueue queue)
    {
        if (!isInQueue) return;

        isInQueue = false;
        queue.Remove(this);
    }

    public void SetUpGame(
        Game game,
        int seed,
        int numPlayers,
        int playerNum,
        List<string> playerNames)
    {
        this.game = game;
        this.playerNum = playerNum;
        controller.SendData(
            string.Format(
                "ng {0} {1} {2} {3}",
                seed.ToString(),
                numPlayers.ToString(),
                playerNum.ToString(),
                string.Join(" ", playerNames)));
    }
}
