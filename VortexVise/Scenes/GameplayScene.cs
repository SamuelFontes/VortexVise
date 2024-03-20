using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.States;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;
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
    static public void InitGameplayScene()
    {
        GameUserInterface.DisableCursor = true;
        CurrentTime = Raylib.GetTime();

        MapLogic.LoadRandomMap();

        LastTimeAccumulator = CurrentTime;
        DeltaTime = 1d / GameCore.GameTickRate;
        LastTime = CurrentTime - DeltaTime;


        LastState.CurrentTime = CurrentTime;
        LastState.Gravity = Gravity;
        PlayerLogic.Init(false);
        if(GameCore.PlayerOneGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerOneProfile.Id));
        if(GameCore.PlayerTwoGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerTwoProfile.Id));
        if(GameCore.PlayerThreeGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerThreeProfile.Id));
        if(GameCore.PlayerFourGamepad != -9) LastState.PlayerStates.Add(new(GameCore.PlayerFourProfile.Id));

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
                if (GameCore.PlayerOneGamepad != -9) GameClient.SendInput(PlayerLogic.GetInput(GameCore.PlayerOneGamepad), GameCore.PlayerOneProfile.Id, CurrentTime);
                if (GameCore.PlayerTwoGamepad != -9) GameClient.SendInput(PlayerLogic.GetInput(GameCore.PlayerTwoGamepad), GameCore.PlayerTwoProfile.Id, CurrentTime);
                if (GameCore.PlayerThreeGamepad != -9) GameClient.SendInput(PlayerLogic.GetInput(GameCore.PlayerThreeGamepad), GameCore.PlayerThreeProfile.Id, CurrentTime);
                if (GameCore.PlayerFourGamepad != -9) GameClient.SendInput(PlayerLogic.GetInput(GameCore.PlayerFourGamepad), GameCore.PlayerFourProfile.Id, CurrentTime);

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
        var player = State.PlayerStates.FirstOrDefault(p => p.Id == GameCore.PlayerOneProfile.Id);
        if (player == null) return;
        Raylib.ClearBackground(Raylib.BLACK);

        PlayerLogic.ProcessCamera(player.Position);
        GameLogic.DrawState(State);
        Raylib.EndMode2D();

        if (GameClient.IsConnected) Raylib.DrawText(GameClient.Ping.ToString(), 0, 32, 32, Raylib.RAYWHITE);
    }

    static public void UnloadGameplayScene()
    {
    }
    static public int FinishGameplayScene()
    {
        return 0;
    }

}
