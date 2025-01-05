using System;
using VortexVise.Core.Interfaces;
using VortexVise.Web.Models;

namespace VortexVise.Web.Services
{
    public class AssetService : IAssetService
    {

        public ITextureAsset LoadTexture(string fileName)
        {
            var texture = new TextureAsset(fileName);
            texture.Load();
            return texture;
        }


        public void UnloadTexture(ITextureAsset texture)
        {
            try
            {
                if (texture != null)
                    texture.Unload();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Well I don't care
            }
        }
    }
}
