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
    UNKNOWN = -1,
    /// <summary>
    /// Game and library logo
    /// </summary>
    LOGO = 0,
    /// <summary>
    /// Title screen
    /// </summary>
    MENU,
    /// <summary>
    /// Options screen
    /// </summary>
    OPTIONS,
    /// <summary>
    /// Gameplay screen
    /// </summary>
    GAMEPLAY,
    /// <summary>
    /// Ending screen
    /// </summary>
    ENDING
};
