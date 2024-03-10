//----------------------------------------------------------------------------------
// Enumerators Definition
//----------------------------------------------------------------------------------
namespace VortexVise;

// GameMode, defines the rules for the current match
public enum GameMode
{
    DeathMatch = 0,             // Free for all
    TeamDeathMatch,             // Team based death match
    Survival                    // Rogue like survival mode
};

// GameScreen, defines what scene the game is in 
public enum GameScene
{
    UNKNOWN = -1,               // Used for transitioning screens
    LOGO = 0,                   // Game and library logo
    TITLE,                      // Title screen
    OPTIONS,                    // Options screen
    GAMEPLAY,                   // Gameplay screen
    ENDING                      // Ending screen
};
