using VortexVise.GameGlobals;
using VortexVise.Logic;
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
    public static GameState State = new GameState();
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
        PlayerLogic.Init(false);
        CameraLogic.Init();
        WeaponLogic.Init();
        if (GameCore.PlayerOneGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerOneProfile.Id));
        if (GameCore.PlayerTwoGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerTwoProfile.Id));
        if (GameCore.PlayerThreeGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerThreeProfile.Id));
        if (GameCore.PlayerFourGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerFourProfile.Id));

        // Play gameplay music
        GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetNotGonnaLeoThis);

    }

    static public void UpdateGameplayScene()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F2)) MapLogic.LoadRandomMap();
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
                if (GameCore.PlayerOneGamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerOneGamepad), GameCore.PlayerOneProfile.Id, CurrentTime);
                if (GameCore.PlayerTwoGamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerTwoGamepad), GameCore.PlayerTwoProfile.Id, CurrentTime);
                if (GameCore.PlayerThreeGamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerThreeGamepad), GameCore.PlayerThreeProfile.Id, CurrentTime);
                if (GameCore.PlayerFourGamepad != -9) GameClient.SendInput(GameInput.GetInput(GameCore.PlayerFourGamepad), GameCore.PlayerFourProfile.Id, CurrentTime);

                // This should not stop the game, so make it run in another task
                GameState receivedState = GameClient.LastServerState;
                if (receivedState.CurrentTime != GameClient.LastSimulatedTime)
                {
                    if (GameCore.PlayerOneGamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerOneProfile.Id);
                    if (GameCore.PlayerTwoGamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerTwoProfile.Id);
                    if (GameCore.PlayerThreeGamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerThreeProfile.Id);
                    if (GameCore.PlayerFourGamepad != -9) receivedState.ApproximateState(LastState, GameCore.PlayerFourProfile.Id);

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
            GameRenderer.DrawGameState(State);
            // TODO: draw hud here
            CameraLogic.EndDrawingToCamera(i);
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
