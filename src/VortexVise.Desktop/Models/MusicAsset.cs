using VortexVise.Core.Interfaces;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Models
{
    public class MusicAsset : IMusicAsset
    {
        public bool IsPlaying { get; set; }
        public bool IsLoaded { get; set; }
        public float Volume { get; set; }
        public ZeroElectric.Vinculum.Music InternalMusic { get; set; }

        public void Load(string path)
        {
            InternalMusic = Raylib.LoadMusicStream(path);
        }

        public void Play()
        {
            IsPlaying = true;
            Raylib.PlayMusicStream(InternalMusic);
        }

        public void Update()
        {
            Raylib.UpdateMusicStream(InternalMusic);
        }

        public void SetVolume(float volume)
        {
            Raylib.SetMusicVolume(InternalMusic, volume);
        }

        public void Stop()
        {
            Raylib.StopMusicStream(InternalMusic);
            IsPlaying = false;
        }

        public void Unload()
        {
            Raylib.UnloadMusicStream(InternalMusic);
        }
    }
}
