namespace VortexVise.Core.Interfaces
{
    public interface ITextureAsset
    {
        public string AssetPath { get; set; }
        public bool IsLoaded { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public void Load(); // TODO: remove this
        public void Load(string assetPath);
        public void Unload();
    }
}
