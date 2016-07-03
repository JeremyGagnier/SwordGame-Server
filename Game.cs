using System;
using System.Collections.Generic;

public class Game
{
    private List<User> players = null;

    public Game(List<User> players)
    {
        this.players = players;
        int seed = (new System.Random()).Next();
        List<string> playerNames = new List<string>();
        for (int i = 0; i < players.Count; ++i)
        {
            if (string.IsNullOrEmpty(players[i].name))
            {
                playerNames.Add(string.Format("Player{0}", i));
            }
            else
            {
                playerNames.Add(players[i].name);
            }
        }
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].SetUpGame(this, seed, players.Count, i + 1, playerNames);
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
