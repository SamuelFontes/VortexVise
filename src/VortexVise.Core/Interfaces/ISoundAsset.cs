namespace VortexVise.Core.Interfaces
{
    public interface ISoundAsset
    {
        public void Load(string path);
        public void Unload();
        public void Play(float pan = 0.5f, float pitch = 1f, float volume = 1f, bool overrideIfPlaying = true);
        public bool IsPlaying();
    }
}
