using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.Models;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

/// <summary>
/// Handles all camera logic in the game.
/// </summary>
public static class CameraLogic
{
    /// <summary>
    /// Setup cameras player cameras and handles split screen multiplayer.
    /// </summary>
    public static void Init()
    {
        var players = Utils.GetNumberOfLocalPlayers();
        if (players == 1)
        {
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture(GameCore.GameScreenWidth, GameCore.GameScreenHeight)
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.5f, 0.5f);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 2)
        {
            // Player 1
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), GameCore.GameScreenHeight)
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.5f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), GameCore.GameScreenHeight)
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), 0);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.5f);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 3)
        {
            // Player 1
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth), (int)(GameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.5f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new(0, (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 4)
        {
            // Player 1
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), 0);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new(0, (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 4
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

        }
    }
    /// <summary>
    /// Start drawing to specific camera.
    /// </summary>
    /// <param name="index">Defines what camera it will be drawing to. 0 to 3.</param>
    /// <param name="targetPosition">Define the position the camera will be targeting. Usually this is the player position.</param>
    public static void BeginDrawingToCamera(int index, Vector2 targetPosition)
    {
        Raylib.EndTextureMode();
        Raylib.BeginTextureMode(GameMatch.PlayerCameras[index].RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK); // Make area outside the map be black on the camera view
        ProcessCamera(ref targetPosition, GameMatch.PlayerCameras[index], ref GameMatch.PlayerCameras[index].Camera);
        Raylib.BeginMode2D(GameMatch.PlayerCameras[index].Camera);
    }
    /// <summary>
    /// End drawing to a specific camera.
    /// </summary>
    /// <param name="index">Defines what camera it will stop drawing to. 0 to 3.</param>
    public static void EndDrawingToCamera(int index, bool isPlayerDead)
    {
        Raylib.EndMode2D();
        Raylib.EndTextureMode();
        if (isPlayerDead)
            Raylib.DrawTextureRec(GameMatch.PlayerCameras[index].RenderTexture.texture, GameMatch.PlayerCameras[index].RenderRectangle, GameMatch.PlayerCameras[index].CameraPosition, Raylib.GRAY);
        else
            Raylib.DrawTextureRec(GameMatch.PlayerCameras[index].RenderTexture.texture, GameMatch.PlayerCameras[index].RenderRectangle, GameMatch.PlayerCameras[index].CameraPosition, Raylib.WHITE);
    }

    /// <summary>
    /// Unload all camera related textures.
    /// </summary>
    public static void Unload()
    {
        foreach (var camera in GameMatch.PlayerCameras)
        {
            Raylib.UnloadRenderTexture(camera.RenderTexture);
        }
        GameMatch.PlayerCameras.Clear();
    }

    static void ProcessCamera(ref Vector2 targetPosition, PlayerCamera playerCamera, ref Camera2D camera)
    {
        Vector2 target = new((int)targetPosition.X, (int)targetPosition.Y);

        // Make it stay inside the map
        if (target.X - GameCore.GameScreenWidth * playerCamera.CameraOffset.X <= 0)
            target.X = GameCore.GameScreenWidth * playerCamera.CameraOffset.X;
        else if (target.X + GameCore.GameScreenWidth * playerCamera.CameraOffset.X >= MapLogic.GetMapSize().X)
            target.X = MapLogic.GetMapSize().X - GameCore.GameScreenWidth * playerCamera.CameraOffset.X;

        if (target.Y - GameCore.GameScreenHeight * playerCamera.CameraOffset.Y <= 0)
            target.Y = GameCore.GameScreenHeight * playerCamera.CameraOffset.Y;
        else if (target.Y + GameCore.GameScreenHeight * playerCamera.CameraOffset.Y >= MapLogic.GetMapSize().Y)
            target.Y = MapLogic.GetMapSize().Y - GameCore.GameScreenHeight * playerCamera.CameraOffset.Y;

        if (GameCore.GameScreenWidth / Utils.GetNumberOfLocalPlayers() >= MapLogic.GetMapSize().X) // Only if map is bigger than screen
            target.X = MapLogic.GetMapSize().X * 0.5f;
        if (GameCore.GameScreenHeight  /Utils.GetNumberOfLocalPlayers() >= MapLogic.GetMapSize().Y) // Only if map is bigger than screen
            target.Y = MapLogic.GetMapSize().Y * 0.5f;

        target.X = (int)target.X;
        target.Y = (int)target.Y;
        // Make camera smooth
        camera.target.X = RayMath.Lerp(camera.target.X, target.X, 1 - (float)Math.Exp(-4 * Raylib.GetFrameTime()));
        camera.target.Y = RayMath.Lerp(camera.target.Y, target.Y, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));
    }
}
