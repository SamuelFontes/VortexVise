using VortexVise.Core.Models;

namespace VortexVise.Core.GameContext;

/// <summary>
/// Game attributes.
/// </summary>
public class GameCore
{
    /// <summary>
    /// Defines tickrate the game and server runs at. This tick defines the minimal amount of game simulations per second that need to occur even if the performs badly and cause low FPS. This will ensure a consistent game behavior independently of low performance.
    /// </summary>
    public int GameTickRate { get; private set; } = 60;
    /// <summary>
    /// Render texture that the game will be rendered at before scaling it to the display's resolution.
    /// </summary>
    /// <summary>
    /// Defines internal game rendering resolution.
    /// </summary>
    public int GameScreenWidth { get; set; }
    /// <summary>
    /// Defines internal game rendering resolution.
    /// </summary>
    public int GameScreenHeight { get; set; }
    public int MenuFontSize { get; set; } = 32;
    /// <summary>
    /// If true the game will exit.
    /// </summary>
    public bool GameShouldClose { get; set; } = false;
    /// <summary>
    /// This is used to indicate if the game is running in dedicated server mode.
    /// </summary>
    public bool IsServer { get; set; } = false;
    /// <summary>
    /// Define if its a network game.
    /// </summary>
    public bool IsNetworkGame { get; set; } = false;
    /// <summary>
    /// Player profile
    /// </summary>
    public PlayerProfile PlayerOneProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerOne", Gamepad = -9 };
    /// <summary>
    /// Player profile
    /// </summary>
    public PlayerProfile PlayerTwoProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerTwo", Gamepad = -9 };
    /// <summary>
    /// Player profile
    /// </summary>
    public PlayerProfile PlayerThreeProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerThree", Gamepad = -9 };
    /// <summary>
    /// Player profile
    /// </summary>
    public PlayerProfile PlayerFourProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerFour", Gamepad = -9 };
    /// <summary>
    /// Max amount of weapons a player can carry
    /// </summary>
    public int MaxWeapons { get; set; } = 4;
}
