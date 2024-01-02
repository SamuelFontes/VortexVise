using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GameState
{
    public static Gamemode gamemode = Gamemode.MainMenu;
    public static List<Player> localPlayers = new List<Player>(); 
    public static List<Team> teams = new List<Team>();

    public static int GetNumberOfLocalPlayers()
    {
        return localPlayers.Count;
    }

}
