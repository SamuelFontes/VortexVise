namespace VortexVise.GameGlobals;

/// <summary>
/// Configuration for game settings. This should be maintained after the user exits the game.
/// </summary>
public static class GameSettings
{
    /// <summary>
    /// This will define if the scalling will be done by integer scalling or bilienear scalling. Integer scalling should only be done when game rendering resolution is multiple of the screen resolution, otherwise it will look like shit.
    /// </summary>
    public static bool IntegerScalling { get; set; } = true;
    /// <summary>
    /// Put black bars on the sides or top and bottom for the nitpicking weirdos. I like it streched.
    /// </summary>
    public static bool MaintainAspectRatio { get; set; } = true;
    /// <summary>
    /// Control the volume of game sounds.
    /// </summary>
    public static float VolumeSounds { get; set; } = 1;
    /// <summary>
    /// Control the volume of game music.
    /// </summary>
    public static float VolumeBGM { get; set; } = 1;
    /// <summary>
    /// Control the volume of game ambience sounds.
    /// </summary>
    public static float VolumeBGS { get; set; } = 1;
    /// <summary>
    /// Define to use borderless full screen, if false it will use true full screen.
    /// </summary>
    public static bool BorderlessFullScreen { get; set; } = true;
    /// <summary>
    /// Define if game should run in fullscreen.
    /// </summary>
    public static bool FullScreen = false;
}
