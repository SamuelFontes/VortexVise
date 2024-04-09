using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

/// <summary>
/// The configuration for the current match settings.
/// </summary>
public static class GameMatch
{
    public static int PlayerSpawnDelay { get; set; } = 3;
    public static int WeaponSpawnDelay { get; set; } = 8;
    public static Map CurrentMap { get; set; } = new();
    public static List<Rectangle> MapCollisions = [];
    public static int MapMirrored = 1;
    public static List<PlayerCamera> PlayerCameras { get; set; } = [];
}
