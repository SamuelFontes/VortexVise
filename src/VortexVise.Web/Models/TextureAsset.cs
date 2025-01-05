using VortexVise.Core.Interfaces;

namespace VortexVise.Web.Models
{
    public class TextureAsset : ITextureAsset
    {
        public string AssetPath { get; set; } = string.Empty;
        public bool IsLoaded { get; set; } = false;
        public int Height { get; set; }
        public int Width { get; set; }
        public Raylib_cs.Texture2D Texture { get; set; }
        public TextureAsset(string assetPath)
        {
            AssetPath = assetPath;
        }
        public TextureAsset()
        {
        }


        public void Load()
        {
            Texture = Raylib_cs.Raylib.LoadTexture(AssetPath);
            Height = Texture.Height;
            Width = Texture.Width;
            IsLoaded = true;
        }


        public void Unload()
        {
            Raylib_cs.Raylib.UnloadTexture(Texture);
            Height = 0;
            Width = 0;
            IsLoaded = false;
        }


        public void Load(string assetPath)
        {
            AssetPath = assetPath;
            Load();
        }
    }
}
