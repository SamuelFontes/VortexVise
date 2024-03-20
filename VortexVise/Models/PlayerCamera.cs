using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameGlobals;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

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
