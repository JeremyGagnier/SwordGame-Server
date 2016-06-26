using System;
using System.Collections.Generic;

public class Game
{
    private List<User> players = null;

    public Game(List<User> players)
    {
        this.players = players;
        List<string> playerNames = new List<string>();
        foreach (User player in players)
        {
            playerNames.Add(player.name);
        }
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].SetUpGame(this, players.Count, i + 1, playerNames);
        }
    }

    public void GameMessage(User user, string[] args)
    {
        foreach (User player in players)
        {
            if (player != user)
            {
                args[0] = player.playerNum.ToString();
                player.SendGameMessage("g " + string.Join(" ", args));
            }
        }
    }
}
