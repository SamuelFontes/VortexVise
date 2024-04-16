using VortexVise.Enums;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.Models;
using VortexVise.Networking;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;
/// <summary>
/// GameplayScene
/// All gameplay related logic should start here.
/// </summary>
static internal class GameplayScene
{
    public static double LastTimeAccumulator { get; set; }
    public static double CurrentTime { get; set; } = 0;
    public static float Gravity { get; set; } = 1000;
    public static double DeltaTime { get; set; }
    public static double LastTime { get; set; }
    public static GameState LastState = new();
    public static double Accumulator = 0;
    public static GameState State = new();
    public static int NumberOfLocalPlayers = 0;
    static public void InitGameplayScene()
    {
        NumberOfLocalPlayers = Utils.GetNumberOfLocalPlayers();
        GameUserInterface.DisableCursor = true;
        CurrentTime = Raylib.GetTime();

        MapLogic.LoadRandomMap();

        LastTimeAccumulator = CurrentTime;
        DeltaTime = 1d / GameCore.GameTickRate;
        LastTime = CurrentTime - DeltaTime;


        LastState.CurrentTime = CurrentTime;
        LastState.Gravity = Gravity;
        PlayerLogic.Init();
        CameraLogic.Init();
        WeaponLogic.Init();
        if (GameCore.PlayerOneProfile.Gamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerOneProfile.Id, GameCore.PlayerOneProfile.Skin));
        if (GameCore.PlayerTwoProfile.Gamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerTwoProfile.Id, GameCore.PlayerTwoProfile.Skin));
        if (GameCore.PlayerThreeProfile.Gamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerThreeProfile.Id, GameCore.PlayerThreeProfile.Skin));
        if (GameCore.PlayerFourProfile.Gamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerFourProfile.Id, GameCore.PlayerFourProfile.Skin));

        LastState.ResetGameState();
    }

    static public void UpdateGameplayScene()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F2) || Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_MIDDLE_LEFT)) MapLogic.LoadNextMap();
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F3) || Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT))
        {
            var bot = new PlayerState(State.PlayerStates.Count, GameAssets.Gameplay.Skins.OrderBy(x => Guid.NewGuid()).First());
            bot.IsBot = true;
            var b = new Bot();
            b.Id = bot.Id;
            GameMatch.Bots.Add(b);
            LastState.PlayerStates.Add(bot); // Add testing dummy
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F5) || Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_THUMB))
        {
            var bot = new PlayerState(State.PlayerStates.Count, GameAssets.Gameplay.Skins.OrderBy(x => Guid.NewGuid()).First());
            bot.Id = -99;
            LastState.PlayerStates.Add(bot); // Add testing dummy
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F4) || Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_RIGHT_THUMB))
        {
            LastState.PlayerStates.RemoveAll(x => x.IsBot);
            LastState.PlayerStates.RemoveAll(x => x.Id == -99);
        }
        bool isSlowerThanTickRate = false;

        CurrentTime = Raylib.GetTime();
        double simulationTime = CurrentTime - LastTime;

        while (simulationTime >= DeltaTime) // perform one update for every interval passed
        {
            CurrentTime = Raylib.GetTime();
            isSlowerThanTickRate = true;

            if (GameClient.IsConnected)
            {
                // Do all the network magic
                if (GameCore.PlayerOneProfile.Gamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerOneProfile.Gamepad), GameCore.PlayerOneProfile.Id, CurrentTime);
                if (GameCore.PlayerTwoProfile.Gamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerTwoProfile.Gamepad), GameCore.PlayerTwoProfile.Id, CurrentTime);
                if (GameCore.PlayerThreeProfile.Gamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerThreeProfile.Gamepad), GameCore.PlayerThreeProfile.Id, CurrentTime);
                if (GameCore.PlayerFourProfile.Gamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerFourProfile.Gamepad), GameCore.PlayerFourProfile.Id, CurrentTime);

                // This should not stop the game, so make it run in another task
                GameState receivedState = GameClient.LastServerState;
                if (receivedState.CurrentTime != GameClient.LastSimulatedTime)
                {
                    if (GameCore.PlayerOneProfile.Gamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerOneProfile.Id);
                    if (GameCore.PlayerTwoProfile.Gamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerTwoProfile.Id);
                    if (GameCore.PlayerThreeProfile.Gamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerThreeProfile.Id);
                    if (GameCore.PlayerFourProfile.Gamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerFourProfile.Id);

                    State = GameLogic.SimulateState(receivedState, CurrentTime, (float)(DeltaTime - Accumulator), true);
                    GameClient.LastSimulatedTime = receivedState.CurrentTime;
                }
                else
                {
                    // Client-Side Prediction
                    State = GameLogic.SimulateState(LastState, CurrentTime, (float)(DeltaTime - Accumulator), true);
                }
            }
            else
            {
                State = GameLogic.SimulateState(LastState, CurrentTime, (float)(DeltaTime - Accumulator), true);
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
            State = GameLogic.SimulateState(LastState, CurrentTime, (float)accumulatorSimulationTime, false);
            LastTimeAccumulator = CurrentTime;
            LastState = State;
        }
        //gameStates.Add(state);
        GameMatch.GameState = State;

    }

    static public void DrawGameplayScene()
    {
        Raylib.ClearBackground(Raylib.BLACK);
        if (State.PlayerStates.Count == 0) return;

        for (int i = 0; i < NumberOfLocalPlayers; i++)
        {
            PlayerState player;
            if (i == 0) player = State.PlayerStates.First(p => p.Id == GameCore.PlayerOneProfile.Id);
            else if (i == 1) player = State.PlayerStates.First(p => p.Id == GameCore.PlayerTwoProfile.Id);
            else if (i == 2) player = State.PlayerStates.First(p => p.Id == GameCore.PlayerThreeProfile.Id);
            else if (i == 3) player = State.PlayerStates.First(p => p.Id == GameCore.PlayerFourProfile.Id);
            else return;

            CameraLogic.BeginDrawingToCamera(i, player.Position);
            GameRenderer.DrawGameState(State, player);
            // TODO: draw hud here
            CameraLogic.EndDrawingToCamera(i);
        }

        // Global HUD
        if (State.MatchState == MatchStates.Warmup)
        {
            Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, new(0, 0, 0, 100));
            Utils.DrawTextCentered($"STARTING IN {(int)State.MatchTimer + 1}", new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f), 32, Raylib.WHITE);
        }
        else if (State.MatchState == MatchStates.Playing)
        {
            var t = TimeSpan.FromSeconds((int)State.MatchTimer);
            Utils.DrawTextCentered($"{t.ToString(@"mm\:ss")}", new(GameCore.GameScreenWidth * 0.5f, 32), 32, Raylib.WHITE);
        }
        else if (State.MatchState == MatchStates.EndScreen)
        {
            Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, new(0, 0, 0, 100));
            var t = TimeSpan.FromSeconds((int)State.MatchTimer);
            Utils.DrawTextCentered($"RESULTS - {t.ToString(@"mm\:ss")}", new(GameCore.GameScreenWidth * 0.5f, 32), 32, Raylib.WHITE);
            var y = 64;
            var players = State.PlayerStates.OrderByDescending(x => x.Stats.Kills).ToList();
            Utils.DrawTextCentered($"PLAYER {players[0].Id + 1} WON!", new(GameCore.GameScreenWidth * 0.5f, y), 32, Raylib.WHITE);
            y += 32;
            foreach (var player in players)
            {
                Utils.DrawTextCentered($"Player {player.Id + 1} - {player.Stats.Kills}/{player.Stats.Deaths}", new(GameCore.GameScreenWidth * 0.5f, y), 16, Raylib.WHITE);
                y += 16;
            }
        }
        else if (State.MatchState == MatchStates.Voting)
        {
            Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, new(0, 0, 0, 100));
            var t = TimeSpan.FromSeconds((int)State.MatchTimer);
            Utils.DrawTextCentered($"MAP VOTING - {t.ToString(@"mm\:ss")}", new(GameCore.GameScreenWidth * 0.5f, 32), 32, Raylib.WHITE);
        }


        if (GameClient.IsConnected) Raylib.DrawText(GameClient.Ping.ToString(), 0, 32, 32, Raylib.RAYWHITE);
    }

    static public void UnloadGameplayScene()
    {
        CameraLogic.Unload();
        //PlayerLogic.Unload();
        WeaponLogic.Unload();
    }
    static public int FinishGameplayScene()
    {
        return 0;
    }

}
