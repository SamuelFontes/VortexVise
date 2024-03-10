/*******************************************************************************************
*
*   Vortex Vise
*
*   A nice game about killing things
*
********************************************************************************************/

using Raylib_cs;
using System.Numerics;
using VortexVise;
using VortexVise.GameObjects;
using VortexVise.Logic;
using VortexVise.States;
using VortexVise.Utilities;

// Initialization
//---------------------------------------------------------
Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
Raylib.InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, "Vortex Vise");
Raylib.SetWindowMinSize(GameCore.GameScreenWidth, GameCore.GameScreenHeight);
float gravity = 1000;

MapLogic.LoadMap("SkyArchipelago", false);

RenderTexture2D target = Raylib.LoadRenderTexture(512, 128);

double currentTime = Raylib.GetTime();
var lastTimeAccumulator = currentTime;
double deltaTime = 1d / GameCore.GameTickRate;
var lastTime = currentTime - deltaTime;

double accumulator = 0;

GameState lastState = new();
lastState.CurrentTime = currentTime;
lastState.Gravity = gravity;
PlayerLogic.Init(false);
Guid playerId = Guid.NewGuid();
lastState.PlayerStates.Add(new(playerId));

//List<GameState> gameStates = new List<GameState>();
//gameStates.Add(lastState);
GameState state = new GameState();
var client = new GameClient();
//Raylib.ToggleFullscreen();
int targetFPS = Utils.GetFPS();
while (!(Raylib.WindowShouldClose() || GameCore.GameShouldClose))
{
    bool isSlowerThanTickRate = false;
    //Raylib.SetTargetFPS(targetFPS);
    if (Raylib.IsKeyPressed(KeyboardKey.F11))
    {
        Raylib.ToggleFullscreen();

    }

    currentTime = Raylib.GetTime();
    double simulationTime = currentTime - lastTime;

    while (simulationTime >= deltaTime) // perform one update for every interval passed
    {
        isSlowerThanTickRate = true;

        if (client.IsConnected)
        {
            // Do all the network magic
            client.SendInput(PlayerLogic.GetInput(), playerId, currentTime);

            // This should not stop the game, so make it run in another task
            GameState receivedState = client.LastServerState;
            if (receivedState.CurrentTime != client.LastSimulatedTime)
            {
                receivedState.ApproximateState(lastState, playerId);
                state = GameLogic.SimulateState(receivedState, currentTime, playerId, (float)(deltaTime - accumulator), true);
                client.LastSimulatedTime = receivedState.CurrentTime;
            }
            else
            {
                // Client-Side Prediction
                state = GameLogic.SimulateState(lastState, currentTime, playerId, (float)(deltaTime - accumulator), true);
            }
        }
        else
        {
            state = GameLogic.SimulateState(lastState, currentTime, playerId, (float)(deltaTime - accumulator), true);
        }
        simulationTime -= deltaTime;
        lastTime += deltaTime;
        accumulator = 0;
        lastTimeAccumulator = currentTime;

    }
    if (!isSlowerThanTickRate)
    {
        // This is if the player has more fps than tickrate, it will always be processed on the client side this should be the same as client-side prediction
        double accumulatorSimulationTime = currentTime - lastTimeAccumulator;
        accumulator += accumulatorSimulationTime;
        state = GameLogic.SimulateState(lastState, currentTime, playerId, (float)(accumulatorSimulationTime), false);
        lastTimeAccumulator = currentTime;
    }
    //gameStates.Add(state);
    lastState = state;

    var player = state.PlayerStates.FirstOrDefault(p => p.Id == playerId);
    if (player == null) continue;
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    PlayerLogic.ProcessCamera(player.Position);
    GameLogic.DrawState(state);

    Raylib.BeginTextureMode(target);
    #region Debug
    // DEBUG
    Raylib.ClearBackground(new(0, 0, 0, 100));
    /*    Raylib.DrawFPS(128, 12);
        Raylib.DrawText("dt: " + (int)(1 / deltaTime), 12, 12, 20, Color.Black);
        Raylib.DrawText("player gravityForce: " + player.Velocity.Y, 12, 32, 20, Color.Black);
        Raylib.DrawText($"player position: {(int)player.Position.X} {(int)player.Velocity.Y}", 12, 64, 20, Color.Black);
        Raylib.DrawText($"collision velocity:{player.Velocity.X}", 12, 129, 20, Color.Black);
    */
    if (!client.IsConnected)
        Raylib.DrawText("PRESS F9 TO CONNECT", 12, 12, 32, Color.White);
    else
        Raylib.DrawText($"CONNECTED - {client.Ping}ms", 12, 12, 32, Color.White);
    Raylib.DrawFPS(12, 46);
    Raylib.DrawText(Utils.GetDebugString(), 12, 64, 16, Color.White);
    Raylib.EndTextureMode();
    var rec = new Rectangle() { X = 0, Y = 0, Width = (float)target.Texture.Width, Height = (float)target.Texture.Height };
    Raylib.DrawTexturePro(target.Texture, new Rectangle(0, 0, (float)target.Texture.Width, (float)target.Texture.Height * -1), rec, new Vector2(0, 0), 0, Color.White);


    #endregion

    Raylib.EndDrawing();
    if (Raylib.IsKeyPressed(KeyboardKey.F7))
    {

        Utils.SwitchDebug();
    }

    if (Raylib.IsKeyPressed(KeyboardKey.F8))
    {
        Utils.UnlockFPS();
        targetFPS = Utils.GetFPS();
    }

    if (Raylib.IsKeyPressed(KeyboardKey.F9))
    {
        client.Connect();

        Thread myThread = new Thread(new ThreadStart(client.GetState));
        myThread.Start();
    }

}

Raylib.CloseWindow();

