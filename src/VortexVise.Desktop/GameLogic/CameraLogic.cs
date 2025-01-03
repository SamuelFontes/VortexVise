using System.Numerics;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Logic;

/// <summary>
/// Handles all camera logic in the game.
/// </summary>
public static class CameraLogic
{
    /// <summary>
    /// Setup cameras player cameras and handles split screen multiplayer.
    /// </summary>
    public static void Init(GameCore gameCore)
    {
        var players = Utils.GetNumberOfLocalPlayers(gameCore);
        if (players == 1)
        {
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture(gameCore.GameScreenWidth, gameCore.GameScreenHeight)
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.5f, 0.5f,gameCore);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 2)
        {
            // Player 1
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), gameCore.GameScreenHeight)
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.5f,gameCore);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), gameCore.GameScreenHeight)
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(gameCore.GameScreenWidth * 0.5f), 0);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.5f,gameCore);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 3)
        {
            // Player 1
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth), (int)(gameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.5f, 0.25f,gameCore);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new(0, (int)(gameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f,gameCore);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f,gameCore);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 4)
        {
            // Player 1
            var camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f,gameCore);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(gameCore.GameScreenWidth * 0.5f), 0);
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f,gameCore);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new(0, (int)(gameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f,gameCore);
            GameMatch.PlayerCameras.Add(camera);

            // Player 4
            camera = new PlayerCamera
            {
                RenderTexture = Raylib.LoadRenderTexture((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f))
            };
            camera.RenderRectangle = new(0, 0, camera.RenderTexture.texture.width, -camera.RenderTexture.texture.height);
            camera.CameraPosition = new((int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f));
            camera.Camera = new Camera2D();
            camera.SetupCamera(0.25f, 0.25f,gameCore);
            GameMatch.PlayerCameras.Add(camera);

        }
    }
    /// <summary>
    /// Start drawing to specific camera.
    /// </summary>
    /// <param name="index">Defines what camera it will be drawing to. 0 to 3.</param>
    /// <param name="targetPosition">Define the position the camera will be targeting. Usually this is the player position.</param>
    public static void BeginDrawingToCamera(int index, Vector2 targetPosition, GameCore gameCore)
    {
        Raylib.EndTextureMode();
        Raylib.BeginTextureMode(GameMatch.PlayerCameras[index].RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK); // Make area outside the map be black on the camera view
        ProcessCamera(ref targetPosition, GameMatch.PlayerCameras[index], ref GameMatch.PlayerCameras[index].Camera,gameCore);
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
        //Raylib.BeginTextureMode(gameCore.GameRendering); // Really important, otherwise will fuck everything up, this is the main game screen 
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

    static void ProcessCamera(ref Vector2 targetPosition, PlayerCamera playerCamera, ref Camera2D camera, GameCore gameCore)
    {
        Vector2 target = new((int)targetPosition.X, (int)targetPosition.Y);

        // Make it stay inside the map
        if (target.X - gameCore.GameScreenWidth * playerCamera.CameraOffset.X <= 0)
            target.X = gameCore.GameScreenWidth * playerCamera.CameraOffset.X;
        else if (target.X + gameCore.GameScreenWidth * playerCamera.CameraOffset.X >= MapLogic.GetMapSize().X)
            target.X = MapLogic.GetMapSize().X - gameCore.GameScreenWidth * playerCamera.CameraOffset.X;

        if (target.Y - gameCore.GameScreenHeight * playerCamera.CameraOffset.Y <= 0)
            target.Y = gameCore.GameScreenHeight * playerCamera.CameraOffset.Y;
        else if (target.Y + gameCore.GameScreenHeight * playerCamera.CameraOffset.Y >= MapLogic.GetMapSize().Y)
            target.Y = MapLogic.GetMapSize().Y - gameCore.GameScreenHeight * playerCamera.CameraOffset.Y;

        if (gameCore.GameScreenWidth / Utils.GetNumberOfLocalPlayers(gameCore) >= MapLogic.GetMapSize().X) // Only if map is bigger than screen
            target.X = MapLogic.GetMapSize().X * 0.5f;
        if (gameCore.GameScreenHeight  /Utils.GetNumberOfLocalPlayers(gameCore) >= MapLogic.GetMapSize().Y) // Only if map is bigger than screen
            target.Y = MapLogic.GetMapSize().Y * 0.5f;

        target.X = (int)target.X;
        target.Y = (int)target.Y;
        // Make camera smooth
        camera.target.X = RayMath.Lerp(camera.target.X, target.X, 1 - (float)Math.Exp(-4 * Raylib.GetFrameTime()));
        camera.target.Y = RayMath.Lerp(camera.target.Y, target.Y, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));
    }
}
