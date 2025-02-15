﻿using VortexVise.Core.Interfaces;
using ZeroElectric.Vinculum;
using VortexVise.Core.GameGlobals;

namespace VortexVise.Desktop.Models
{
    public class SoundAsset : ISoundAsset
    {
        Sound sound;
        public bool IsPlaying()
        {
            return Raylib.IsSoundPlaying(sound);
        }

        public void Load(string path)
        {
            sound = Raylib.LoadSound(path);
        }

        public void Play(float pan = 0.5F, float pitch = 1, float volume = 1, bool overrideIfPlaying = true)
        {
            if (!overrideIfPlaying && Raylib.IsSoundPlaying(sound)) return;

            volume *= GameSettings.VolumeSounds;
            Raylib.SetSoundPan(sound, pan);
            Raylib.SetSoundPitch(sound, pitch);
            Raylib.SetSoundVolume(sound, volume);
            Raylib.PlaySound(sound);
        }

        public void Unload()
        {
            Raylib.UnloadSound(sound);
        }
    }
}
