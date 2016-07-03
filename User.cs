using System;
using System.Net.Sockets;
using System.Collections.Generic;

public class User
{
    private SocketHandler.Controller controller = null;

    public Game game = null;
    public int playerNum = 0;
    public string name = "";

    public User(Socket socket)
    {
        controller = new SocketHandler.Controller(socket);
        controller.onCloseConnection += (e) => Server.LogOut(e, this);
        controller.onReceiveData += (s) => Server.ProcessMessage(s, this);
    }

    public void LogOut()
    {
        controller.Stop();
    }

    public void SendGameMessage(string message)
    {
        controller.SendData(message);
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
