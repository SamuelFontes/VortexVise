//----------------------------------------------------------------------------------
// Enumerators Definition
//----------------------------------------------------------------------------------
namespace VortexVise.Core.Enums;

/// <summary>
/// GameMode, defines the rules for the current match
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Free for all
    /// </summary>
    DeathMatch = 0,
    /// <summary>
    /// Team based death match
    /// </summary>
    TeamDeathMatch,
    /// <summary>
    /// Rogue like survival mode
    /// </summary>
    Survival
};
