using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GameState
{
    public static Gamemode Gamemode = Gamemode.MainMenu;
    public static List<Player> LocalPlayers = new List<Player>(); 
    public static List<Team> Teams = new List<Team>();
    public static Map CurrentMap; 

    public static int GetNumberOfLocalPlayers()
    {
        return LocalPlayers.Count;
    }

}
