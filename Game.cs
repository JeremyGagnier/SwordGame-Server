using System;
using System.Collections.Generic;

public class Game
{
    private List<User> players = null;
    private List<List<byte[]>> playerFrames = new List<List<byte[]>>();

    public Game(List<User> players)
    {
        this.players = players;
        for (int i = 0; i < players.Count; ++i)
        {
            playerFrames.Add(new List<byte[]>());
        }

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
            players[i].SetUpGame(this, seed, players.Count, i, playerNames);
        }
    }

    public void GameMessage(User user, byte[] message)
    {
        // Read the Game Message Specification for more information
        // about byte accessing
        int frame = message[0] * 65536 + message[1] * 256 + message[2];
        int pnum = (int)message[3] % 4;
        if (playerFrames[pnum] == null) return; // This user has already left
        if (frame >= playerFrames[pnum].Count)
        {
            // Fill with null until we reach our frame count
            for (int i = playerFrames[pnum].Count; i < frame; ++i)
            {
                playerFrames[pnum].Add(null);
            }
            playerFrames[pnum].Add(message);
        }
        else
        {
            if (playerFrames[pnum][frame] == null)
            {
                playerFrames[pnum][frame] = message;
            }
            else
            {
                // Already received this message so just move on
                return;
            }
        }
        foreach (User player in players)
        {
            if (player != null && player != user)
            {
                player.SendGameMessage(message);
            }
        }
    }

    public void GetFrame(User user, int pnum, int frame)
    {
        if (playerFrames[pnum].Count > frame && playerFrames[pnum][frame] != null)
        {
            user.SendGameMessage(playerFrames[pnum][frame]);
        }
        else
        {
            players[pnum].AskForFrame(frame);
        }
    }

    public void LeaveGame(User user)
    {
        players[user.playerNum] = null;
        playerFrames[user.playerNum] = null;
        if (players.Count == 0)
        {
            Server.EndGame(this);
        }
        user.game = null;
    }
}
