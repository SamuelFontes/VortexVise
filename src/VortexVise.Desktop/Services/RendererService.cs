﻿using System.Drawing;
using System.Numerics;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Extensions;
using VortexVise.Core.Models;
using VortexVise.Desktop.Models;

namespace VortexVise.Desktop.Services
{
    internal class RendererService : IRendererService
    {
        public void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color)
        {
            ZeroElectric.Vinculum.Raylib.DrawLineEx(startPos, endPos, thick, color.ToRaylibColor());
        }

        public void DrawRectangle(Rectangle rec, Color color)
        {
            ZeroElectric.Vinculum.Raylib.DrawRectangleRec(rec.ToRaylibRectangle(), color.ToRaylibColor());
        }

        public void DrawTextEx(IFontAsset font, string text, Vector2 position, float fontSize, float spacing, Color tint)
        {
            if (font is FontAsset raylibFont)
            {
                ZeroElectric.Vinculum.Raylib.DrawTextEx(raylibFont.Font, text, position, fontSize, spacing, tint.ToRaylibColor());
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
                ZeroElectric.Vinculum.Raylib.DrawTexture(raylibTexture.Texture, x, y, color.ToRaylibColor());
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
                ZeroElectric.Vinculum.Raylib.DrawTextureEx(raylibTexture.Texture, position, rotation, scale, color.ToRaylibColor());
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

        public float GetFrameTime()
        {
            return ZeroElectric.Vinculum.Raylib.GetFrameTime();
        }

        public Vector2 MeasureTextEx(IFontAsset font, string text, float fontSize, float spacing)
        {
            if (font is FontAsset raylibFont)
            {
                return ZeroElectric.Vinculum.Raylib.MeasureTextEx(raylibFont.Font, text, fontSize, spacing);
            }
            else
            {
                throw new ArgumentException("Invalid asset type.");
            }
        }

        public void DrawText(string text, Vector2 position, float fontSize, Color tint)
        {
            ZeroElectric.Vinculum.Raylib.DrawText(text, position.X, position.Y, fontSize, tint.ToRaylibColor());
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
            ZeroElectric.Vinculum.Raylib.ClearBackground(color.ToRaylibColor());
        }
        public double GetTime()
        {
            return ZeroElectric.Vinculum.Raylib.GetTime();
        }

        public void BeginDrawingToCamera(IPlayerCamera camera)
        {
            if (camera is PlayerCamera raylibCamera)
            {
                ZeroElectric.Vinculum.Raylib.EndTextureMode();
                ZeroElectric.Vinculum.Raylib.BeginTextureMode(raylibCamera.RenderTexture);
                ZeroElectric.Vinculum.Raylib.ClearBackground(ZeroElectric.Vinculum.Raylib.BLACK); // Make area outside the map be black on the camera view
                ZeroElectric.Vinculum.Raylib.BeginMode2D(raylibCamera.Camera);
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
                ZeroElectric.Vinculum.Raylib.EndMode2D();
                ZeroElectric.Vinculum.Raylib.EndTextureMode();
                //ZeroElectric.Vinculum.Raylib.BeginTextureMode(GameCore.GameRendering); // Enable this in case of rendering to render texture
                ZeroElectric.Vinculum.Raylib.DrawTextureRec(raylibCamera.RenderTexture.texture, raylibCamera.RenderRectangle, raylibCamera.CameraPosition, color.ToRaylibColor());
            }
            else
            {
                throw new ArgumentException("Invalid camera type.");
            }
        }

        public void BeginDrawing()
        {
            ZeroElectric.Vinculum.Raylib.BeginDrawing();
        }
        public void EndDrawing()
        {
            ZeroElectric.Vinculum.Raylib.EndDrawing();
        }
    }
}
