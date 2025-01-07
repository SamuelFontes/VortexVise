using VortexVise.Core.Interfaces;
using VortexVise.Core.Models;

namespace VortexVise.Editor
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
