using System.Numerics;
using VortexVise.GameGlobals;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

/// <summary>
/// Used to create camera logic in CameraLogic.
/// </summary>
public class PlayerCamera
{
    public Camera2D Camera;
    public RenderTexture RenderTexture;
    public Rectangle RenderRectangle;
    public Vector2 CameraPosition;
    public Vector2 CameraOffset;

    public void SetupCamera(float offsetX, float offsetY)
    {
        CameraOffset.X = offsetX;
        CameraOffset.Y = offsetY;
        var cameraView = new Vector2(GameCore.GameScreenWidth * offsetX, GameCore.GameScreenHeight * offsetY);
        Camera.offset = cameraView;
        Camera.target = cameraView;
        Camera.zoom = 1;
    }
}
