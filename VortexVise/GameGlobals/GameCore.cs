using Raylib_cs;
using VortexVise.Scenes;

namespace VortexVise.GameGlobals;


public static class GameCore
{
    public static int GameTickRate { get; private set; } = 64;                  // Defines tickrate the game and server runs at
    public static int GameScreenWidth { get; private set; } = 960;              // Defines internal game resolution
    public static int GameScreenHeight { get; private set; } = 540;             // Defines internal game resolution
    public static float GameScreenScale { get; set; }                           // How much the screen is scalling 
    public static bool GameShouldClose { get; set; } = false;                   // If true the game will close 


}
