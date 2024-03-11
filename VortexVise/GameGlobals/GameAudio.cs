using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.GameGlobals;

public static class GameAudio
{
    public static Sound HookShoot {  get; private set; }    
    public static Sound HookHit { get; private set; }
    public static Sound Jump { get; private set; }
    public static Sound Dash { get; private set; }

    public static void InitAudio()
    {
        HookShoot = Raylib.LoadSound("Resources/Audio/FX/hook_fire.wav");
        HookHit = Raylib.LoadSound("Resources/Audio/FX/hook_hit.wav");
        Jump = Raylib.LoadSound("Resources/Audio/FX/jump.wav");
        Dash = Raylib.LoadSound("Resources/Audio/FX/dash.wav");
    }

    public static void UnloadAudio()
    {
        Raylib.UnloadSound(HookShoot);
        Raylib.UnloadSound(HookHit);
        Raylib.UnloadSound(Jump);
        Raylib.UnloadSound(Dash);
    }

    public static void PlaySound(Sound sound, float pan = 0.5f, float pitch = 1f, float volume = 1f)
    {
        // This is probably not woking properly, tried using the pitch it sounds the same
        if (GameCore.IsServer) return; // Audio don't play on the server

        Raylib.SetSoundPan(sound, pan);
        Raylib.SetSoundPitch(sound, pitch);
        Raylib.SetSoundVolume(sound, volume);
        Raylib.PlaySound(sound);
        Raylib.SetSoundPan(sound, 0.5f);
        Raylib.SetSoundPitch(sound, 1f);
        Raylib.SetSoundVolume(sound, 1f);
    }
}
