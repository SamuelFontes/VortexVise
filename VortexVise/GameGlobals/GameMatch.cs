using VortexVise.Enums;

namespace VortexVise.GameGlobals;
public static class GameMatch
{
    public static GameMode CurrentGameMode { get; set; } = GameMode.DeathMatch;
    public static int CurrentGravity = 1000;
}
