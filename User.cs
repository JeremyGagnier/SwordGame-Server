using System;
using System.Net.Sockets;
using System.Collections.Generic;

public class User
{
    private SocketHandler.Controller controller = null;

    public Game game = null;
    public int playerNum = 0;

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
}
