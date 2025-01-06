//----------------------------------------------------------------------------------
// Enumerators Definition
//----------------------------------------------------------------------------------
namespace VortexVise.Core.Enums;

/// <summary>
/// GameScreen, defines what scene the game is in 
/// </summary>
public enum GameScene
{
    /// <summary>
    /// Used for transitioning screens
    /// </summary>
    Unknown = -1,
    /// <summary>
    /// Game and library logo
    /// </summary>
    Logo = 0,
    /// <summary>
    /// Title screen
    /// </summary>
    Menu,
    /// <summary>
    /// Options screen
    /// </summary>
    Options,
    /// <summary>
    /// Gameplay screen
    /// </summary>
    Gameplay,
    /// <summary>
    /// Ending screen
    /// </summary>
    Ending
};
