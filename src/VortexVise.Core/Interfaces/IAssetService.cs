namespace VortexVise.Core.Interfaces
{
    public interface IAssetService
    {
        public ITextureAsset LoadTexture(string fileName);
        void UnloadTexture(ITextureAsset texture);
    }
}
