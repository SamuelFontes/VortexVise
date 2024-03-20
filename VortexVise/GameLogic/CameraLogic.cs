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
        // Setup cameras for split screen, this is kind of a mess but since it's self contained it shouldn't be a problem
        var players = Utils.GetNumberOfLocalPlayers();
        if (players == 1)
        {
            var camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture(GameCore.GameScreenWidth, GameCore.GameScreenHeight);
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.5f, 0.5f);
            PlayerCameras.Add(camera);
        }
        else if (players == 2)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), GameCore.GameScreenHeight);
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.5f);
            PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), GameCore.GameScreenHeight);
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), 0);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.5f);
            PlayerCameras.Add(camera);
        }
        else if (players == 3)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.5f, 0.25f);
            PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new(0, (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            PlayerCameras.Add(camera);
        }
        else if (players == 4)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), 0);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new(0, (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            PlayerCameras.Add(camera);

            // Player 4
            camera = new PlayerCamera();
            camera.RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            PlayerCameras.Add(camera);

        }
    }
    public static void BeginDrawingToCamera(int index, Vector2 targetPosition)
    {
        Raylib.EndTextureMode();
        Raylib.BeginTextureMode(PlayerCameras[index].RenderTexture);
        ProcessCamera(ref targetPosition, PlayerCameras[index], ref PlayerCameras[index].Camera);
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
    static void ProcessCamera(ref Vector2 targetPosition, PlayerCamera playerCamera, ref Camera2D camera)
    {
        Vector2 target = new(targetPosition.X, targetPosition.Y);

        // Make it stay inside the map
        if (target.X - GameCore.GameScreenWidth * playerCamera.CameraOffset.X <= 0)
            target.X = GameCore.GameScreenWidth * playerCamera.CameraOffset.X;
        else if (target.X + GameCore.GameScreenWidth * playerCamera.CameraOffset.X >= MapLogic.GetMapSize().X)
            target.X = MapLogic.GetMapSize().X - GameCore.GameScreenWidth * playerCamera.CameraOffset.X;

        if (target.Y - GameCore.GameScreenHeight * playerCamera.CameraOffset.Y <= 0)
            target.Y = GameCore.GameScreenHeight * playerCamera.CameraOffset.Y;
        else if (target.Y + GameCore.GameScreenHeight * playerCamera.CameraOffset.Y >= MapLogic.GetMapSize().Y)
            target.Y = MapLogic.GetMapSize().Y - GameCore.GameScreenHeight * playerCamera.CameraOffset.Y;

        // Make camera smooth
        // FIXME: fix camera jerkness when almost hitting the target
        camera.target.X = RayMath.Lerp(camera.target.X, target.X, 1 - (float)Math.Exp(-6 * Raylib.GetFrameTime()));
        camera.target.Y = RayMath.Lerp(camera.target.Y, target.Y, 1 - (float)Math.Exp(-6 * Raylib.GetFrameTime()));

    }

}
