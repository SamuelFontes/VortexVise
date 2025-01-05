using System;
using System.Drawing;
using System.Numerics;
using System.Text;
using VortexVise.Core.Interfaces;
using VortexVise.Web.Extensions;
using VortexVise.Web.Models;

namespace VortexVise.Web.Services
{
    public class RendererService : IRendererService
    {

        public void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color)
        {
            Raylib_cs.Raylib.DrawLineEx(startPos, endPos, thick, color.ToRaylibColor());
        }


        public void DrawRectangleRec(Rectangle rec, Color color)
        {
            Raylib_cs.Raylib.DrawRectangleRec(rec.ToRaylibRectangle(), color.ToRaylibColor());
        }


        public void DrawTextEx(IFontAsset font, string text, Vector2 position, float fontSize, float spacing, Color tint)
        {
            if (font is FontAsset raylibFont)
            {
                Raylib_cs.Raylib.DrawTextEx(raylibFont.Font, text, position, fontSize, spacing, tint.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid asset type.");
            }
        }


        public void DrawTexture(ITextureAsset? texture, int x, int y, Color color)
        {
            if (texture is TextureAsset raylibTexture)
            {
                Raylib_cs.Raylib.DrawTexture(raylibTexture.Texture, x, y, color.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid texture asset type.");
            }
        }


        public void DrawTextureEx(ITextureAsset? texture, Vector2 position, int rotation, int scale, Color color)
        {
            if (texture is TextureAsset raylibTexture)
            {
                Raylib_cs.Raylib.DrawTextureEx(raylibTexture.Texture, position, rotation, scale, color.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid texture asset type.");
            }
        }


        public void DrawTexturePro(ITextureAsset texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color tint)
        {
            if (texture is TextureAsset raylibTexture)
            {
                Raylib_cs.Raylib.DrawTexturePro(raylibTexture.Texture, source.ToRaylibRectangle(), dest.ToRaylibRectangle(), origin, rotation, tint.ToRaylibColor());
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
                Raylib_cs.Raylib.DrawTextureRec(raylibTexture.Texture, source.ToRaylibRectangle(), position, tint.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid texture asset type.");
            }
        }


        public int GetScreenHeight()
        {
            return Raylib_cs.Raylib.GetScreenHeight();
        }


        public int GetScreenWidth()
        {
            return Raylib_cs.Raylib.GetScreenWidth();
        }


        public Vector2 GetMousePosition()
        {
            return Raylib_cs.Raylib.GetMousePosition();
        }


        public float GetFrameTime()
        {
            return Raylib_cs.Raylib.GetFrameTime();
        }


        public Vector2 MeasureTextEx(IFontAsset font, string text, float fontSize, float spacing)
        {
            if (font is FontAsset raylibFont)
            {
                return Raylib_cs.Raylib.MeasureTextEx(raylibFont.Font, text, fontSize, spacing);
            }
            else
            {
                throw new ArgumentException("Invalid asset type.");
            }
        }


        public void DrawText(string text, Vector2 position, float fontSize, Color tint)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            unsafe
            {
                fixed (byte* p = bytes)
                {
                    sbyte* sp = (sbyte*)p;
                    //SP is now what you want
                    Raylib_cs.Raylib.DrawText(sp, (int)position.X, (int)position.Y, (int)fontSize, tint.ToRaylibColor());
                }
            }
        }


        public void DrawTextCentered(IFontAsset font, string text, Vector2 textPosition, int textSize, Color color)
        {
            if (font is FontAsset raylibFont)
            {
                var textBoxSize = MeasureTextEx(raylibFont, text, textSize, 0);
                var pos = new Vector2(textPosition.X - textBoxSize.X * 0.5f, textPosition.Y - textBoxSize.Y * 0.5f); // Centers text
                DrawTextEx(raylibFont, text, pos, textSize, 0, color);
            }
            else
            {
                throw new ArgumentException("Invalid asset type.");
            }
        }


        public void ClearBackground(Color color)
        {
            Raylib_cs.Raylib.ClearBackground(color.ToRaylibColor());
        }

        public double GetTime()
        {
            return Raylib_cs.Raylib.GetTime();
        }


        public void BeginDrawingToCamera(IPlayerCamera camera)
        {
            if (camera is PlayerCamera raylibCamera)
            {
                Raylib_cs.Raylib.EndTextureMode();
                Raylib_cs.Raylib.BeginTextureMode(raylibCamera.RenderTexture);
                Raylib_cs.Raylib.ClearBackground(Raylib_cs.Color.Black); // Make area outside the map be black on the camera view
                Raylib_cs.Raylib.BeginMode2D(raylibCamera.Camera);
            }
            else
            {
                throw new ArgumentException("Invalid camera type.");
            }
        }


        public void EndDrawingToCamera(IPlayerCamera camera, Color color)
        {
            if (camera is PlayerCamera raylibCamera)
            {
                Raylib_cs.Raylib.EndMode2D();
                Raylib_cs.Raylib.EndTextureMode();
                //Raylib_cs.Raylib.BeginTextureMode(GameCore.GameRendering); // Enable this in case of rendering to render texture
                Raylib_cs.Raylib.DrawTextureRec(raylibCamera.RenderTexture.Texture, raylibCamera.RenderRectangle, raylibCamera.CameraPosition, color.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid camera type.");
            }
        }


        public void BeginDrawing()
        {
            Raylib_cs.Raylib.BeginDrawing();
        }

        public void EndDrawing()
        {
            Raylib_cs.Raylib.EndDrawing();
        }
    }
}
