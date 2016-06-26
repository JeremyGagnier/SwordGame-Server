using System;
using System.Collections.Generic;

public class GameQueue
{
    private Dictionary<int, List<User>> gameQueue =
        new Dictionary<int, List<User>>();

    public void Add(User user, List<int> modes)
    {
        foreach(int mode in modes) {
            if (!gameQueue.ContainsKey(mode)) {
                gameQueue.Add(mode, new List<User>());
            }
            gameQueue[mode].Add(user);
        }
    }

    public void Remove(User user)
    {
        foreach (List<User> userList in gameQueue.Values)
        {
            if (userList.Contains(user))
            {
                userList.Remove(user);
            }
        }
    }

    public Game TryFormGame()
    {
        for (int i = 4; i >= 2; --i)
        {
            if (gameQueue.ContainsKey(i) && gameQueue[i].Count == i)
            {
                List<User> players = gameQueue[i];
                gameQueue.Remove(i);
                foreach (User player in players)
                {
                    Remove(player);
                }
                return new Game(players);
            }
            else if (gameQueue.ContainsKey(i) && gameQueue[i].Count > i)
            {
                Console.WriteLine("Found a list with too many players!");
            }
        }
        return null;
    }
}
