using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Models;

namespace VortexVise.Core.Interfaces
{
    public interface IRendererService
    {
        public void DrawTexturePro(ITextureAsset texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color tint);
        public void DrawTextureRec(ITextureAsset texture, Rectangle source, Vector2 position, Color tint);
        public void DrawRectangleRec(Rectangle rec, Color color);

    }
}
