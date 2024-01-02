using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameState
{
    public Gamemode Gamemode = Gamemode.MainMenu;
    public List<Player> LocalPlayers = new List<Player>(); 
    public List<Team> Teams = new List<Team>();

    public int GetNumberOfLocalPlayers()
    {
        return LocalPlayers.Count;
    }

}
