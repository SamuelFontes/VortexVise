using System.Text.Json;
using VortexVise.Core.Enums;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Logic;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.Networking;
using VortexVise.Desktop.States;
using VortexVise.Desktop.Utilities;

namespace VortexVise.Desktop.Scenes;

/// <summary>
/// GameplayScene
/// All gameplay related logic should start here.
/// </summary>
public class GameplayScene
{
    int finishScreen = 0;
    public double LastTimeAccumulator { get; set; }
    public double CurrentTime { get; set; } = 0;
    public float Gravity { get; set; } = 1000;
    public double DeltaTime { get; set; }
    public double LastTime { get; set; }
    public GameState LastState = new();
    public double Accumulator = 0;
    public GameState State = new();
    public List<GameState> GameStates = new();
    public readonly IInputService _inputService;

    public GameplayScene(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void InitGameplayScene(GameCore gameCore, IAssetService assetService, IRendererService rendererService)
    {
        GameUserInterface.DisableCursor = true;
        CurrentTime = rendererService.GetTime();

        LastTimeAccumulator = CurrentTime;
        DeltaTime = 1d / gameCore.GameTickRate;
        LastTime = CurrentTime - DeltaTime;


        LastState.CurrentTime = CurrentTime;
        LastState.Gravity = Gravity;
        PlayerLogic.Init(assetService);
        CameraLogic.Init(gameCore);
        if (gameCore.PlayerOneProfile.Gamepad != GamepadSlot.Disconnected) LastState.PlayerStates.Add(new(gameCore.PlayerOneProfile.Id, gameCore.PlayerOneProfile.Skin));
        if (gameCore.PlayerTwoProfile.Gamepad != GamepadSlot.Disconnected) LastState.PlayerStates.Add(new(gameCore.PlayerTwoProfile.Id, gameCore.PlayerTwoProfile.Skin));
        if (gameCore.PlayerThreeProfile.Gamepad != GamepadSlot.Disconnected) LastState.PlayerStates.Add(new(gameCore.PlayerThreeProfile.Id, gameCore.PlayerThreeProfile.Skin));
        if (gameCore.PlayerFourProfile.Gamepad != GamepadSlot.Disconnected) LastState.PlayerStates.Add(new(gameCore.PlayerFourProfile.Id, gameCore.PlayerFourProfile.Skin));

        BotLogic.Init(LastState);

        LastState.ResetGameState();
        finishScreen = 0;
    }

    public void UpdateGameplayScene(SceneManager sceneManager, ICollisionService collisionService, GameCore gameCore, IRendererService rendererService)
    {
        //if (Raylib.IsKeyPressed(KeyboardKey.KEY_F2)) MapLogic.LoadNextMap();
        //if (Raylib.IsKeyPressed(KeyboardKey.KEY_F3))
        //{
        //    var bot = new PlayerState(Guid.NewGuid(), GameAssets.Gameplay.Skins.OrderBy(x => Guid.NewGuid()).First());
        //    bot.IsBot = true;
        //    var b = new Bot();
        //    b.Id = bot.Id;
        //    GameMatch.Bots.Add(b);
        //    LastState.PlayerStates.Add(bot); // add bot
        //}
        //if (Raylib.IsKeyPressed(KeyboardKey.KEY_F5))
        //{
        //    var bot = new PlayerState(Guid.NewGuid(), GameAssets.Gameplay.Skins.OrderBy(x => Guid.NewGuid()).First());
        //    bot.Id = Guid.NewGuid();
        //    bot.Position = State.PlayerStates.Select(x => x.Position).First();
        //    LastState.PlayerStates.Add(bot); // Add testing dummy
        //}
        //if (Raylib.IsKeyPressed(KeyboardKey.KEY_F4))
        //{
        //    LastState.PlayerStates.RemoveAll(x => x.IsBot);
        //}
        bool isSlowerThanTickRate = false;

        CurrentTime = rendererService.GetTime();
        double simulationTime = CurrentTime - LastTime;

        while (simulationTime >= DeltaTime) // perform one update for every interval passed
        {
            CurrentTime = rendererService.GetTime();
            isSlowerThanTickRate = true;

            if (GameClient.IsConnected)
            {
                // Do all the network magic
                // TODO: The input should be send together instead of one for each player
                if (gameCore.PlayerOneProfile.Gamepad != GamepadSlot.Disconnected) GameClient.SendInput(_inputService.ReadPlayerInput(gameCore.PlayerOneProfile.Gamepad), gameCore.PlayerOneProfile.Id, State.Tick);
                if (gameCore.PlayerTwoProfile.Gamepad != GamepadSlot.Disconnected) GameClient.SendInput(_inputService.ReadPlayerInput(gameCore.PlayerTwoProfile.Gamepad), gameCore.PlayerTwoProfile.Id, State.Tick);
                if (gameCore.PlayerThreeProfile.Gamepad != GamepadSlot.Disconnected) GameClient.SendInput(_inputService.ReadPlayerInput(gameCore.PlayerThreeProfile.Gamepad), gameCore.PlayerThreeProfile.Id, State.Tick);
                if (gameCore.PlayerFourProfile.Gamepad != GamepadSlot.Disconnected) GameClient.SendInput(_inputService.ReadPlayerInput(gameCore.PlayerFourProfile.Gamepad), gameCore.PlayerFourProfile.Id, State.Tick);

                if (GameClient.IsHost)
                {
                    State = GameLogic.SimulateState(collisionService, LastState, CurrentTime, (float)(DeltaTime - Accumulator), true, _inputService, sceneManager, gameCore);
                    State.OwnerId = gameCore.PlayerOneProfile.Id;
                    GameClient.SendState(State);
                }
                else
                {
                    // This should not stop the game, so make it run in another task
                    GameState receivedState = GameClient.LastServerState;
                    if (receivedState.Tick != GameClient.LastTickSimluated)
                    {
                        //if (gameCore.PlayerOneProfile.Gamepad != -9) receivedState.ApproximateState(LastState, gameCore.PlayerOneProfile.Id);
                        //if (gameCore.PlayerTwoProfile.Gamepad != -9) receivedState.ApproximateState(LastState, gameCore.PlayerTwoProfile.Id);
                        //if (gameCore.PlayerThreeProfile.Gamepad != -9) receivedState.ApproximateState(LastState, gameCore.PlayerThreeProfile.Id);
                        //if (gameCore.PlayerFourProfile.Gamepad != -9) receivedState.ApproximateState(LastState, gameCore.PlayerFourProfile.Id);
                        // TODO: Simulate all ticks in the past up to current one, apply approximations on the state that matches the tick received 

                        State = GameLogic.SimulateState(collisionService, receivedState, CurrentTime, (float)(DeltaTime - Accumulator), true, _inputService, sceneManager, gameCore);
                        GameClient.LastTickSimluated = receivedState.Tick;
                    }
                    else
                    {
                        // Client-Side Prediction
                        State = GameLogic.SimulateState(collisionService, LastState, CurrentTime, (float)(DeltaTime - Accumulator), true, _inputService, sceneManager, gameCore);
                    }
                }
            }
            else
            {
                State = GameLogic.SimulateState(collisionService, LastState, CurrentTime, (float)(DeltaTime - Accumulator), true, _inputService, sceneManager, gameCore);
            }
            simulationTime -= DeltaTime;
            LastTime += DeltaTime;
            Accumulator = 0;
            LastTimeAccumulator = CurrentTime;
            LastState = State;
        }

        if (!isSlowerThanTickRate)
        {
            // This is if the player has more fps than tickrate, it will always be processed on the client side this should be the same as client-side prediction
            double accumulatorSimulationTime = CurrentTime - LastTimeAccumulator;
            Accumulator += accumulatorSimulationTime;
            State = GameLogic.SimulateState(collisionService, LastState, CurrentTime, (float)accumulatorSimulationTime, false, _inputService, sceneManager, gameCore);
            LastTimeAccumulator = CurrentTime;
            LastState = State;
        }
        GameStates.Add(State);
        GameMatch.GameState = State;

        //if (Raylib.IsKeyPressed(KeyboardKey.KEY_F8))
        //{
        //    var json = JsonSerializer.Serialize(GameStates, SourceGenerationContext.Default.ListGameState);
        //    System.IO.File.WriteAllText(@"replay.json", json);
        //}
        //if (Raylib.IsKeyPressed(KeyboardKey.KEY_F9))
        //{
        //    foreach (var map in GameAssets.Gameplay.Maps)
        //    {
        //        var json = JsonSerializer.Serialize(map, SourceGenerationContext.Default.Map);
        //        System.IO.File.WriteAllText($"Resources/Maps/{map.Name.Replace(" ", "")}.json", json);

        //    }
        //}

        if (!State.IsRunning) finishScreen = 1;
    }

    public void DrawGameplayScene(IRendererService rendererService, GameCore gameCore, ICollisionService collisionService)
    {
        rendererService.ClearBackground(System.Drawing.Color.Black);
        if (State.PlayerStates.Count == 0) return;

        for (int i = 0; i < Utils.GetNumberOfLocalPlayers(gameCore); i++)
        {
            PlayerState player;
            if (i == 0) player = State.PlayerStates.First(p => p.Id == gameCore.PlayerOneProfile.Id);
            else if (i == 1) player = State.PlayerStates.First(p => p.Id == gameCore.PlayerTwoProfile.Id);
            else if (i == 2) player = State.PlayerStates.First(p => p.Id == gameCore.PlayerThreeProfile.Id);
            else if (i == 3) player = State.PlayerStates.First(p => p.Id == gameCore.PlayerFourProfile.Id);
            else return;

            CameraLogic.BeginDrawingToCamera(i, player.Position, gameCore);
            GameRenderer.DrawGameState(rendererService, State, player, collisionService);
            CameraLogic.EndDrawingToCamera(i, player.IsDead);
        }

    }

    public void UnloadGameplayScene(IAssetService assetService)
    {
        CameraLogic.Unload();
        PlayerLogic.Unload(assetService);
        LastState = new();
        State = new();
        BotLogic.Unload();
        GameMatch.GameState = null;
        GC.Collect();
    }

    public int FinishGameplayScene()
    {
        return finishScreen;
    }

}
