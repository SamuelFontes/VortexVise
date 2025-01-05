using System.Numerics;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
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
            var camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, gameCore.GameScreenWidth, gameCore.GameScreenHeight, 0.5f, 0.5f);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 2)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), gameCore.GameScreenHeight, 0.25f, 0.5f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), gameCore.GameScreenHeight, 0.25f, 0.5f, (int)(gameCore.GameScreenWidth * 0.5f), 0);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 3)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,(int)(gameCore.GameScreenWidth * 0.5f), 0);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,0, (int)(gameCore.GameScreenHeight * 0.5f));
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 4)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,(int)(gameCore.GameScreenWidth * 0.5f), 0);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,0, (int)(gameCore.GameScreenHeight * 0.5f));
            GameMatch.PlayerCameras.Add(camera);

            // Player 4
            camera = new PlayerCamera();
            camera.Setup(gameCore.GameScreenWidth, gameCore.GameScreenHeight, (int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,(int)(gameCore.GameScreenWidth * 0.5f), (int)(gameCore.GameScreenHeight * 0.5f));
            GameMatch.PlayerCameras.Add(camera);
        }
    }

    /// <summary>
    /// Unload all camera related textures.
    /// </summary>
    public static void Unload()
    {
        foreach (var camera in GameMatch.PlayerCameras)
        {
            camera.Unload();
        }
        GameMatch.PlayerCameras.Clear();
    }

    public static void ProcessCamera(Vector2 targetPosition, IPlayerCamera playerCamera, GameCore gameCore)
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
        if (gameCore.GameScreenHeight / Utils.GetNumberOfLocalPlayers(gameCore) >= MapLogic.GetMapSize().Y) // Only if map is bigger than screen
            target.Y = MapLogic.GetMapSize().Y * 0.5f;

        target.X = (int)target.X;
        target.Y = (int)target.Y;
        // Make camera smooth
        playerCamera.SetTarget(target);
    }
}
