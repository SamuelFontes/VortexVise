using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Models
{
    internal class TextureAsset : ITextureAsset
    {
        public string AssetPath { get; set; } = string.Empty;
        public bool IsLoaded { get; set; } = false;
        public int Height { get; set; }
        public int Width { get; set; }
        public Texture Texture { get; set; }
        public TextureAsset(string assetPath)
        {
            AssetPath = assetPath;
        }

        public void Load()
        {
            Texture = Raylib.LoadTexture(AssetPath);
            Height = Texture.height;
            Width = Texture.width;
            IsLoaded = true;
        }

        public void Unload()
        {
            Raylib.UnloadTexture(Texture);
            Height = 0;
            Width = 0;
            IsLoaded = false;
        }
    }
}
