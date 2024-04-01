using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

public static class GameSounds
{
    public static Sound HookShoot;
    public static Sound HookHit;
    public static Sound Jump;
    public static Sound Dash;
    public static Sound Click;
    public static Sound Selection;
    public static Sound WeaponDrop;

    public static void InitAudio()
    {
        HookShoot = Raylib.LoadSound("Resources/Audio/FX/hook_fire.wav");
        HookHit = Raylib.LoadSound("Resources/Audio/FX/hook_hit.wav");
        Jump = Raylib.LoadSound("Resources/Audio/FX/jump.wav");
        Dash = Raylib.LoadSound("Resources/Audio/FX/dash.wav");
        Click = Raylib.LoadSound("Resources/Audio/FX/click.wav");
        Selection = Raylib.LoadSound("Resources/Audio/FX/selection.wav");
        WeaponDrop = Raylib.LoadSound("Resources/Audio/FX/metal_drop.wav");
    }

    public static void UnloadAudio()
    {
        Raylib.UnloadSound(HookShoot);
        Raylib.UnloadSound(HookHit);
        Raylib.UnloadSound(Jump);
        Raylib.UnloadSound(Dash);
        Raylib.UnloadSound(Click);
        Raylib.UnloadSound(Selection);
        Raylib.UnloadSound(WeaponDrop);
    }

    public static void PlaySound(Sound sound, float pan = 0.5f, float pitch = 1f, float volume = 1f)
    {
        if (GameCore.IsServer) return; // Audio don't play on the server

        volume *= GameSettings.VolumeSounds;
        Raylib.SetSoundPan(sound, pan);
        Raylib.SetSoundPitch(sound, pitch);
        Raylib.SetSoundVolume(sound, volume);
        Raylib.PlaySound(sound);
    }
}
