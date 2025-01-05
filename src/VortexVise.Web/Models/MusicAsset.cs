using VortexVise.Core.Interfaces;

namespace VortexVise.Web.Models
{
    public class MusicAsset : IMusicAsset
    {
        public bool IsPlaying { get; set; }
        public bool IsLoaded { get; set; }
        public float Volume { get; set; }
        public Raylib_cs.Music InternalMusic { get; set; }


        public void Load(string path)
        {
            InternalMusic = Raylib_cs.Raylib.LoadMusicStream(path);
        }


        public void Play()
        {
            IsPlaying = true;
            Raylib_cs.Raylib.PlayMusicStream(InternalMusic);
        }


        public void Update()
        {
            Raylib_cs.Raylib.UpdateMusicStream(InternalMusic);
        }


        public void SetVolume(float volume)
        {
            Raylib_cs.Raylib.SetMusicVolume(InternalMusic, volume);
        }


        public void Stop()
        {
            Raylib_cs.Raylib.StopMusicStream(InternalMusic);
            IsPlaying = false;
        }


        public void Unload()
        {
            Raylib_cs.Raylib.UnloadMusicStream(InternalMusic);
        }
    }
}
