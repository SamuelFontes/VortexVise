using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise;


public static class GameCore
{
    public static int GameTickRate { get; private set; } = 64;                  // Defines tickrate the game and server runs at
    public static int GameScreenWidth { get; private set; } = 960;              // Defines internal game resolution
    public static int GameScreenHeight { get; private set; } = 540;             // Defines internal game resolution
    public static GameScreen CurrentScreen { get; set; } = GameScreen.LOGO;     // Defines what is the current screen
    public static bool GameShouldClose { get; set; } = false;                   // If true the game will close 
}
