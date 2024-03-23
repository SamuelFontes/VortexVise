//----------------------------------------------------------------------------------
// Enumerators Definition
//----------------------------------------------------------------------------------
namespace VortexVise.Enums;

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
    MENU,                       // Title screen
    OPTIONS,                    // Options screen
    GAMEPLAY,                   // Gameplay screen
    ENDING                      // Ending screen
};

// Weapon type
public enum WeaponType
{
    Pistol,
    SMG,
    Shotgun,
    MeleeBlunt,
    MeleeCut,
    Granade,
    Mine,
    Bazoka
};

// StatusEffects
public enum StatusEffects
{
    Cold,                       // Cause slow, if accumulated it will cause freeze, fires remove this
    Wet,                        // If it is wet, it can get stunned longer with eleticity, can remove fire, fire removes this but don't burn, increase cold effect 
    Fire,                       // Cause constant damage, removes with water, cold or freeze, can spread to others if stay too close 
    Eletricity,                 // Stuns, if target was wet it doubles stun time
    Freezed,                    // Covered in ice, is stunned, any hit causes double damage but remove this effect, this is only caused by stacking a certain amout of cold
    Confusion,                  // Invert player controls
    Dizzy,                      // Simple stun
    GetRotatedIdiot,            // Inverts map horizontally and invert gravity for this entity, dude will fall upwards
    Bleeding,                   // Deals damage over time, fast
    Poison,                     // Deals damage over time, slow
    Heal,                       // Heal target
};
