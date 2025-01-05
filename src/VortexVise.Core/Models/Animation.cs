using System.Drawing;
using VortexVise.Core.Interfaces;

namespace VortexVise.Core.Models
{
    public class Animation
    {
        public Animation(IAssetService assetService, string textureLocation, int size, int stateAmount, int scale, Color color, float frameTime)
        {
            Texture = assetService.LoadTexture(textureLocation);
            Size = size;
            StateAmount = stateAmount;
            Scale = scale;
            Color = color;
            FrameTime = frameTime;
        }
        public ITextureAsset Texture;
        public int Size;
        public int StateAmount;
        public int Scale;
        public Color Color;
        public float FrameTime;
    }
}
