using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Interfaces
{
    public interface IRendererService
    {
        public void DrawTexturePro(ITextureAsset texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color tint);
        public void DrawTextureRec(ITextureAsset texture, Rectangle source, Vector2 position, Color tint);
        public void DrawRectangleRec(Rectangle rec, Color color);
        public void DrawTexture(ITextureAsset? texture, int x, int y, Color color);
        public void DrawTextureEx(ITextureAsset? texture, Vector2 position, int v1, int v2, Color white);
        public Vector2 MeasureTextEx(IFontAsset font, string text, float fontSize, float spacing);
        public void DrawTextEx(IFontAsset font, string text, Vector2 position, float fontSize, float spacing, Color tint);
        public void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color);
    }
}
