using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Models;
using VortexVise.Desktop.Models;

namespace VortexVise.Desktop.Services
{
    internal class AssetService : IAssetService
    {
        public ITextureAsset LoadTexture(string fileName)
        {
            var texture = new TextureAsset(fileName);
            texture.Load();
            return texture;
        }

        public void UnloadTexture(ITextureAsset texture)
        {
            texture.Unload();
        }
    }
}
