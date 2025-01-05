using System.Numerics;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.Utilities;

namespace VortexVise.Desktop.Logic;

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
            var camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, GameCore.GameScreenWidth, GameCore.GameScreenHeight, 0.5f, 0.5f);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 2)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), GameCore.GameScreenHeight, 0.25f, 0.5f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), GameCore.GameScreenHeight, 0.25f, 0.5f, (int)(GameCore.GameScreenWidth * 0.5f), 0);
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 3)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,(int)(GameCore.GameScreenWidth * 0.5f), 0);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,0, (int)(GameCore.GameScreenHeight * 0.5f));
            GameMatch.PlayerCameras.Add(camera);
        }
        else if (players == 4)
        {
            // Player 1
            var camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f);
            GameMatch.PlayerCameras.Add(camera);

            // Player 2
            camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,(int)(GameCore.GameScreenWidth * 0.5f), 0);
            GameMatch.PlayerCameras.Add(camera);

            // Player 3
            camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,0, (int)(GameCore.GameScreenHeight * 0.5f));
            GameMatch.PlayerCameras.Add(camera);

            // Player 4
            camera = new PlayerCamera();
            camera.Setup(GameCore.GameScreenWidth, GameCore.GameScreenHeight, (int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f), 0.25f, 0.25f,(int)(GameCore.GameScreenWidth * 0.5f), (int)(GameCore.GameScreenHeight * 0.5f));
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

    public static void ProcessCamera(Vector2 targetPosition, IPlayerCamera playerCamera)
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
        if (GameCore.GameScreenHeight / Utils.GetNumberOfLocalPlayers() >= MapLogic.GetMapSize().Y) // Only if map is bigger than screen
            target.Y = MapLogic.GetMapSize().Y * 0.5f;

        target.X = (int)target.X;
        target.Y = (int)target.Y;
        // Make camera smooth
        playerCamera.SetTarget(target);
    }
}
