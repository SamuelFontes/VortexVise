using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

/// <summary>
/// Global game attributes.
/// </summary>
public static class GameCore
{
    /// <summary>
    /// Defines tickrate the game and server runs at. This tick defines the minimal amount of game simulations per second that need to occur even if the performs badly and cause low FPS. This will ensure a consistent game behavior independently of low performance.
    /// </summary>
    public static int GameTickRate { get; private set; } = 60;
    /// <summary>
    /// Render texture that the game will be rendered at before scaling it to the display's resolution.
    /// </summary>
    /// <summary>
    /// Defines internal game rendering resolution.
    /// </summary>
    public static int GameScreenWidth { get { return Raylib.GetScreenWidth(); }  }
    /// <summary>
    /// Defines internal game rendering resolution.
    /// </summary>
    public static int GameScreenHeight { get { return Raylib.GetScreenHeight(); }  }
    public static int MenuFontSize { get; set; } = 32;
    /// <summary>
    /// If true the game will exit.
    /// </summary>
    public static bool GameShouldClose { get; set; } = false;
    /// <summary>
    /// This is used to indicate if the game is running in dedicated server mode.
    /// </summary>
    public static bool IsServer { get; set; } = false;
    /// <summary>
    /// Define if its a network game.
    /// </summary>
    public static bool IsNetworkGame { get; set; } = false;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerOneProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerOne", Gamepad = -9 };
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerTwoProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerTwo", Gamepad = -9 };
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerThreeProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerThree", Gamepad = -9 };
    /// <summary>
    /// Player profile
    /// </summary>
    public static PlayerProfile PlayerFourProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerFour", Gamepad = -9 };
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Max amount of weapons a player can carry
    /// </summary>
    public static int MaxWeapons { get; set; } = 4;
    /// <summary>
    /// Master servers.
    /// </summary>
    public static List<MasterServer> MasterServers { get; set; } = [];
}
