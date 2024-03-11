using ZeroElectric.Vinculum;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.States;

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
    public static GameClient Client = new GameClient();
    public static Guid playerId = Guid.NewGuid();
    static public void InitGameplayScene()
    {

        MapLogic.LoadMap("SkyArchipelago", false);

        LastTimeAccumulator = CurrentTime;
        DeltaTime = 1d / GameCore.GameTickRate;
        LastTime = CurrentTime - DeltaTime;


        LastState.CurrentTime = CurrentTime;
        LastState.Gravity = Gravity;
        PlayerLogic.Init(false);
        LastState.PlayerStates.Add(new(playerId));

    }

    static public void UpdateGameplayScene()
    {
        bool isSlowerThanTickRate = false;

        CurrentTime = Raylib.GetTime();
        double simulationTime = CurrentTime - LastTime;

        while (simulationTime >= DeltaTime) // perform one update for every interval passed
        {
            isSlowerThanTickRate = true;

            if (Client.IsConnected)
            {
                // Do all the network magic
                Client.SendInput(PlayerLogic.GetInput(), playerId, CurrentTime);

                // This should not stop the game, so make it run in another task
                GameState receivedState = Client.LastServerState;
                if (receivedState.CurrentTime != Client.LastSimulatedTime)
                {
                    receivedState.ApproximateState(LastState, playerId);
                    State = GameLogic.SimulateState(receivedState, CurrentTime, playerId, (float)(DeltaTime - Accumulator), true);
                    Client.LastSimulatedTime = receivedState.CurrentTime;
                }
                else
                {
                    // Client-Side Prediction
                    State = GameLogic.SimulateState(LastState, CurrentTime, playerId, (float)(DeltaTime - Accumulator), true);
                }
            }
            else
            {
                State = GameLogic.SimulateState(LastState, CurrentTime, playerId, (float)(DeltaTime - Accumulator), true);
            }
            simulationTime -= DeltaTime;
            LastTime += DeltaTime;
            Accumulator = 0;
            LastTimeAccumulator = CurrentTime;

        }
        if (!isSlowerThanTickRate)
        {
            // This is if the player has more fps than tickrate, it will always be processed on the client side this should be the same as client-side prediction
            double accumulatorSimulationTime = CurrentTime - LastTimeAccumulator;
            Accumulator += accumulatorSimulationTime;
            State = GameLogic.SimulateState(LastState, CurrentTime, playerId, (float)accumulatorSimulationTime, false);
            LastTimeAccumulator = CurrentTime;
        }
        //gameStates.Add(state);
        LastState = State;

    }

    static public void DrawGameplayScene()
    {
        var player = State.PlayerStates.FirstOrDefault(p => p.Id == playerId);
        if (player == null) return;
        Raylib.ClearBackground(BLACK);

        PlayerLogic.ProcessCamera(player.Position);
        GameLogic.DrawState(State);
        Raylib.EndMode2D();
    }

    static public void UnloadGameplayScene()
    {
    }
    static public int FinishGameplayScene()
    {
        return 0;
    }

}
