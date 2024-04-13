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
    /// Current connected server IP. Maybe deprecated.
    /// </summary>
    public static string ServerIPAddress { get; set; } = "";
    /// <summary>
    /// Define if its a network game.
    /// </summary>
    public static bool IsNetworkGame { get; set; } = false;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Global Network SignalR Hub Connection.
    /// </summary>
    public static HubConnection HubConnection { get; set; }
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerOneProfile { get; set; } = new() { Id = 0, Name = "PlayerOne" };
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerTwoProfile { get; set; } = new() { Id = 0, Name = "PlayerTwo" };
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerThreeProfile { get; set; } = new() { Id = 0, Name = "PlayerThree" };
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerFourProfile { get; set; } = new() { Id = 0, Name = "PlayerFour" };
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Max amount of weapons a player can carry
    /// </summary>
    public static int MaxWeapons { get; set; } = 4;
}
