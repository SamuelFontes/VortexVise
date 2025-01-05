using System.Numerics;
using VortexVise.Core.Interfaces;

namespace VortexVise.Web.Models;

/// <summary>
/// Used to create camera logic in CameraLogic.
/// </summary>
public class PlayerCamera : IPlayerCamera
{
    public Raylib_cs.Camera2D Camera;
    public Raylib_cs.RenderTexture2D RenderTexture;
    public Raylib_cs.Rectangle RenderRectangle;
    public Vector2 CameraPosition { get; set; }
    public Vector2 CameraOffset { get; set; }


    public void SetTarget(Vector2 target)
    {
        // TODO: Make camera smooth
        Camera.Target = target;
    }


    public void Setup(int screenWidth, int screenHeight, int cameraWidth, int cameraHeight, float offsetX, float offsetY, int cameraPositionX = 0, int cameraPositionY = 0)
    {
        RenderTexture = Raylib_cs.Raylib.LoadRenderTexture(cameraWidth, cameraHeight);
        RenderRectangle = new(0, 0, RenderTexture.Texture.Width, -RenderTexture.Texture.Height);
        Camera = new Raylib_cs.Camera2D();
        CameraOffset = new(offsetX, offsetY);
        var cameraView = new Vector2(screenWidth * offsetX, screenHeight * offsetY);
        Camera.Offset = cameraView;
        Camera.Target = cameraView;
        Camera.Zoom = 1;
        CameraPosition = new(cameraPositionX, cameraPositionY);
    }


    public void Unload()
    {
        Raylib_cs.Raylib.UnloadRenderTexture(RenderTexture);
    }
}
