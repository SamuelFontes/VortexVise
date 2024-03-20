using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameGlobals;
using VortexVise.Models;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

public static class CameraLogic
{
    public static List<PlayerCamera> PlayerCameras { get; set; } = new List<PlayerCamera>();
    public static void Init()
    {
        var players = Utils.GetNumberOfLocalPlayers();
        if (players == 1)
        {
            var cameraOne = new PlayerCamera();
            cameraOne.RenderTexture = Raylib.LoadRenderTexture(GameCore.GameScreenWidth, GameCore.GameScreenHeight);
            cameraOne.RenderRectangle = new(0, 0, cameraOne.RenderTexture.texture.width, -cameraOne.RenderTexture.texture.height);
            var cameraView = new Vector2(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f);
            cameraOne.Camera = new Camera2D();
            cameraOne.Camera.offset = cameraView;
            cameraOne.Camera.target = cameraView;
            cameraOne.Camera.zoom = 1;
            PlayerCameras.Add(cameraOne);
        }
        else if (players == 2)
        {

        }
        else if (players == 3)
        {

        }
        else if (players == 4)
        {

        }
    }
    public static void BeginDrawingToCamera(int index, Vector2 targetPosition)
    {
        Raylib.EndTextureMode();
        Raylib.BeginTextureMode(PlayerCameras[index].RenderTexture);
        ProcessCamera(ref targetPosition, ref PlayerCameras[index].Camera);
        Raylib.BeginMode2D(PlayerCameras[index].Camera);
    }
    public static void EndDrawingToCamera(int index)
    {
        Raylib.EndMode2D();
        Raylib.EndTextureMode();
        Raylib.BeginTextureMode(GameCore.GameRendering); // Really important, otherwise will fuck everything up, this is the main game screen 
        Raylib.DrawTextureRec(PlayerCameras[index].RenderTexture.texture, PlayerCameras[index].RenderRectangle, PlayerCameras[index].CameraPosition, Raylib.WHITE);
    }
    public static void Unload()
    {
        foreach (var camera in PlayerCameras)
        {
            Raylib.UnloadRenderTexture(camera.RenderTexture);
        }
        PlayerCameras.Clear();
    }
    static void ProcessCamera(ref Vector2 targetPosition, ref Camera2D camera)
    {
        Vector2 target = new(targetPosition.X, targetPosition.Y);

        // Make it stay inside the map
        if (target.X - GameCore.GameScreenWidth * 0.5f <= 0)
            target.X = GameCore.GameScreenWidth * 0.5f;
        else if (target.X + GameCore.GameScreenWidth * 0.5f >= MapLogic.GetMapSize().X)
            target.X = MapLogic.GetMapSize().X - GameCore.GameScreenWidth * 0.5f;

        if (target.Y - GameCore.GameScreenHeight * 0.5f <= 0)
            target.Y = GameCore.GameScreenHeight * 0.5f;
        else if (target.Y + GameCore.GameScreenHeight * 0.5f >= MapLogic.GetMapSize().Y)
            target.Y = MapLogic.GetMapSize().Y - GameCore.GameScreenHeight * 0.5f;

        // Make camera smooth
        // FIXME: fix camera jerkness when almost hitting the target
        camera.target.X = RayMath.Lerp(camera.target.X, target.X, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));
        camera.target.Y = RayMath.Lerp(camera.target.Y, target.Y, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));

    }

}
