using System.Numerics;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.GameContext;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Models;

/// <summary>
/// Used to create camera logic in CameraLogic.
/// </summary>
public class PlayerCamera : IPlayerCamera
{
    public Camera2D Camera;
    public RenderTexture RenderTexture;
    public Rectangle RenderRectangle;
    public Vector2 CameraPosition { get; set; }
    public Vector2 CameraOffset { get; set; }

    public void SetTarget(Vector2 target)
    {
        // TODO: move this to some other core
        // Make camera smooth
        Camera.target.X = RayMath.Lerp(Camera.target.X, target.X, 1 - (float)Math.Exp(-4 * Raylib.GetFrameTime()));
        Camera.target.Y = RayMath.Lerp(Camera.target.Y, target.Y, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));
    }

    public void Setup(int screenWidth, int screenHeight, int cameraWidth, int cameraHeight,float offsetX, float offsetY, int cameraPositionX = 0, int cameraPositionY = 0)
    {
        RenderTexture = Raylib.LoadRenderTexture(cameraWidth, cameraHeight);
        RenderRectangle = new(0, 0, RenderTexture.texture.width, -RenderTexture.texture.height);
        Camera = new Camera2D();
        CameraOffset = new(offsetX,offsetY);
        var cameraView = new Vector2(screenWidth * offsetX, screenHeight * offsetY);
        Camera.offset = cameraView;
        Camera.target = cameraView;
        Camera.zoom = 1;
        CameraPosition = new(cameraPositionX,cameraPositionY);
    }

    public void Unload()
    {
        Raylib.UnloadRenderTexture(RenderTexture);
    }
}
