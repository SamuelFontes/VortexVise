using VortexVise.Core.Interfaces;
using VortexVise.Core.GameGlobals;

namespace VortexVise.Web.Models
{
    public class SoundAsset : ISoundAsset
    {
        Raylib_cs.Sound sound;
        public bool IsPlaying()
        {
            return Raylib_cs.Raylib.IsSoundPlaying(sound);
        }

        public void Load(string path)
        {
            sound = Raylib_cs.Raylib.LoadSound(path);
        }

        public void Play(float pan = 0.5F, float pitch = 1, float volume = 1, bool overrideIfPlaying = true)
        {
            if (!overrideIfPlaying && Raylib_cs.Raylib.IsSoundPlaying(sound)) return;

            volume *= GameSettings.VolumeSounds;
            Raylib_cs.Raylib.SetSoundPan(sound, pan);
            Raylib_cs.Raylib.SetSoundPitch(sound, pitch);
            Raylib_cs.Raylib.SetSoundVolume(sound, volume);
            Raylib_cs.Raylib.PlaySound(sound);
        }

        public void Unload()
        {
            Raylib_cs.Raylib.UnloadSound(sound);
        }
    }
}
