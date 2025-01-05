using System.Drawing;
using System.Numerics;

namespace VortexVise.Core.Interfaces
{
    public interface IRendererService
    {
        public void DrawTexturePro(ITextureAsset texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color tint);
        public void DrawTextureRec(ITextureAsset texture, Rectangle source, Vector2 position, Color tint);
        public void DrawRectangleRec(Rectangle rec, Color color);
        public void DrawTexture(ITextureAsset? texture, int x, int y, Color color);
        public void DrawTextureEx(ITextureAsset? texture, Vector2 position, int rotation, int scale, Color white);
        public Vector2 MeasureTextEx(IFontAsset font, string text, float fontSize, float spacing);
        public void DrawText(string text, Vector2 position, float fontSize, Color tint);
        public void DrawTextCentered(IFontAsset font, string text, Vector2 textPosition, int textSize, Color color);
        public void DrawTextEx(IFontAsset font, string text, Vector2 position, float fontSize, float spacing, Color tint);
        public void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color);
        public int GetScreenWidth();
        public int GetScreenHeight();
        public Vector2 GetMousePosition();
        public float GetFrameTime();
        public double GetTime();
        public void ClearBackground(Color color);
        public void BeginDrawingToCamera(IPlayerCamera camera);
        public void EndDrawingToCamera(IPlayerCamera camera, Color color);
    }
}
