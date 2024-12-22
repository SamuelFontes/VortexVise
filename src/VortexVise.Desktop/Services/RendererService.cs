using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Models;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.Models;

namespace VortexVise.Desktop.Services
{
    internal class RendererService : IRendererService
    {
        public void DrawRectangleRec(Rectangle rec, Color color)
        {
            ZeroElectric.Vinculum.Raylib.DrawRectangleRec(rec.ToRaylibRectangle(), color.ToRaylibColor());
        }

        public void DrawTexturePro(ITextureAsset texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color tint)
        {
            if (texture is TextureAsset raylibTexture)
            {
                ZeroElectric.Vinculum.Raylib.DrawTexturePro(raylibTexture.Texture, source.ToRaylibRectangle(), dest.ToRaylibRectangle(), origin, rotation, tint.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid texture asset type.");
            }
        }

        public void DrawTextureRec(ITextureAsset texture, Rectangle source, Vector2 position, Color tint)
        {
            if (texture is TextureAsset raylibTexture)
            {
                ZeroElectric.Vinculum.Raylib.DrawTextureRec(raylibTexture.Texture, source.ToRaylibRectangle(), position, tint.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid texture asset type.");
            }
        }
    }
}
