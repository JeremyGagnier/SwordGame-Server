using System;
using System.Collections.Generic;

public class Game
{
    private List<User> players = null;

    public Game(List<User> players)
    {
        this.players = players;
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].playerNum = i + 1;
        }
    }

    public void GameMessage(User user, string[] args)
    {
        foreach (User player in players)
        {
            if (player != user)
            {
                args[0] = player.playerNum.ToString();
                player.SendGameMessage(string.Join(" ", args));
            }
        }
    }
}
