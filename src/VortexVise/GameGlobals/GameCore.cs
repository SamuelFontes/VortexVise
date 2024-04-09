using Microsoft.AspNetCore.SignalR.Client;
using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

/// <summary>
/// Global game attributes.
/// </summary>
public static class GameCore
{
    /// <summary>
    /// Defines tickrate the game and server runs at. This tick defines the minimal amount of game simulations per second that need to occur even if the performs badly and cause low FPS. This will ensure a consistent game behaviour independently of low performance.
    /// </summary>
    public static int GameTickRate { get; private set; } = 120;
    /// <summary>
    /// Render texture that the game will be rendered at before scalling it to the display's resolution.
    /// </summary>
    public static RenderTexture GameRendering { get; set; }
    /// <summary>
    /// Defines internal game rendering resolution.
    /// </summary>
    public static int GameScreenWidth { get; private set; } = 960;
    /// <summary>
    /// Defines internal game rendering resolution.
    /// </summary>
    public static int GameScreenHeight { get; private set; } = 540;
    /// <summary>
    /// Indicates how much the game is being scalled based on the game resolution and screen resolution.
    /// </summary>
    public static float GameScreenScale { get; set; }
    /// <summary>
    /// If true the game will exit.
    /// </summary>
    public static bool GameShouldClose { get; set; } = false;
    /// <summary>
    /// This is used to indicate if the game is running in dedicated server mode.
    /// </summary>
    public static bool IsServer { get; set; } = false;
    /// <summary>
    /// Set target FPS for the game. If set to 0 it's unlimited.
    /// </summary>
    public static int TargetFPS { get; private set; } = 0;
    /// <summary>
    /// Current connected server IP. Maybe deprecated.
    /// </summary>
    public static string ServerIPAddress { get; set; } = "";
    /// <summary>
    /// Define if its a network game.
    /// </summary>
    public static bool IsNetworkGame { get; set; } = false;
    /// <summary>
    /// Global Network SignalR Hub Connection.
    /// </summary>
    public static HubConnection HubConnection { get; set; }
    public static int PlayerOneGamepad { get; set; } = -9;                       // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
    public static PlayerProfile PlayerOneProfile { get; set; } = new PlayerProfile() { Id = 0, Name = "PlayerOne", SkinLocation = "Resources/Sprites/Skins/fatso.png" }; // TODO: remove this after implementing profiles
    public static int PlayerTwoGamepad { get; set; } = -9;                       // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
    public static PlayerProfile PlayerTwoProfile { get; set; } = new PlayerProfile() { Id = 1, Name = "PlayerTwo", SkinLocation = "Resources/Sprites/Skins/fatso.png" };
    public static int PlayerThreeGamepad { get; set; } = -9;                     // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
    public static PlayerProfile PlayerThreeProfile { get; set; } = new PlayerProfile() { Id = 2, Name = "PlayerThree", SkinLocation = "Resources/Sprites/Skins/fatso.png" };
    public static int PlayerFourGamepad { get; set; } = -9;                      // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
    public static PlayerProfile PlayerFourProfile { get; set; } = new PlayerProfile() { Id = 3, Name = "PlayerFour", SkinLocation = "Resources/Sprites/Skins/fatso.png" };
    public static int MaxWeapons { get; set; } = 4;
}
