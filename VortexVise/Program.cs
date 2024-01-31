using Raylib_cs;
using System.Numerics;
using VortexVise.GameObjects;
using VortexVise.Logic;
using VortexVise.States;
using VortexVise.Utilities;

float gravity = 1800;
int tickrate = 64;
int screenWidth = 1920;
int screenHeight = 1080;
Raylib.InitWindow(screenWidth, screenHeight, "Vortex Vise");
//Raylib.ToggleFullscreen();
//Raylib.DisableCursor();

MapLogic.LoadMap("SkyArchipelago");

RenderTexture2D target = Raylib.LoadRenderTexture(300, 300);

double currentTime = Raylib.GetTime();
var lastTimeAccumulator = currentTime;
double deltaTime = 1d / tickrate;
var lastTime = currentTime - deltaTime;

double accumulator = 0;

GameState lastState = new();
lastState.CurrentTime = currentTime;
lastState.Gravity = gravity;
PlayerLogic.Init();
Guid playerId = Guid.NewGuid();
lastState.PlayerStates.Add(new(playerId));

List<GameState> gameStates = new List<GameState>();
gameStates.Add(lastState);
GameState state = new GameState();
var client = new GameClient();
while (!Raylib.WindowShouldClose())
{
    bool isSlowerThanTickRate = false;
    int targetFPS = Utils.GetFPS();
    if (targetFPS != 0)
    {
        double time = 1d / targetFPS;
        Raylib.WaitTime(time);
    }

    currentTime = Raylib.GetTime();
    double simulationTime = currentTime - lastTime;

    while (simulationTime >= deltaTime) // perform one update for every interval passed
    {
        isSlowerThanTickRate = true;
        // TODO: Here we should send the state to the server
        // ON THE SERVER
        /*
        void processInput( double time, Input input )
        {
            if ( time < currentTime )// this is important
                return;

            float deltaTime = currentTime - time;

            updatePhysics( currentTime, deltaTime, input );
        }
        */
        // THIS SHOULD HAPPEN ON THE SERVER

        state = GameLogic.SimulateState(lastState, currentTime, playerId, (float)(deltaTime - accumulator));
        simulationTime -= deltaTime;
        lastTime += deltaTime;
        accumulator = 0;
        lastTimeAccumulator = currentTime;
        // TODO: when receive the packet do Clients Approximate Physics Locally
        /*
        void clientUpdate( float time, Input input, State state )
        {
            Vector difference = state.position -
                                current.position;

            float distance = difference.length();

            if ( distance > 2.0f )
                current.position = state.position;
            else if ( distance > 0.1 )
                current.position += difference * 0.1f;

            current.velocity = velocity;

            current.input = input;
        }*/

        // TODO: Create the Client-Side Prediction
    }
    if (!isSlowerThanTickRate)
    {
        // This is if the player has more fps than tickrate, it will always be processed on the client side this should be the same as client-side prediction
        double accumulatorSimulationTime = currentTime - lastTimeAccumulator;
        accumulator += accumulatorSimulationTime;
        state = GameLogic.SimulateState(lastState, currentTime, playerId, (float)(accumulatorSimulationTime));
        lastTimeAccumulator = currentTime;
        // TODO: If inputs happen here, they should be sent to the server. Otherwise no because this can happen 3000 times per second
    }
    gameStates.Add(state);
    lastState = state;

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    PlayerLogic.ProcessCamera(state.PlayerStates.FirstOrDefault(p => p.Id == playerId).Position);
    GameLogic.DrawState(state);

    #region Debug
    // DEBUG
    var player = state.PlayerStates.FirstOrDefault(p => p.Id == playerId);
    Raylib.BeginTextureMode(target);
    Raylib.ClearBackground(Color.White);
    Raylib.DrawFPS(128, 12);
    Raylib.DrawText("dt: " + (int)(1 / deltaTime), 12, 12, 20, Color.Black);
    Raylib.DrawText("player gravityForce: " + player.Velocity.Y, 12, 32, 20, Color.Black);
    Raylib.DrawText($"player position: {(int)player.Position.X} {(int)player.Velocity.Y}", 12, 64, 20, Color.Black);
    Raylib.DrawText($"collision velocity:{player.Velocity.X}", 12, 129, 20, Color.Black);
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
    }
    if (Raylib.IsKeyPressed(KeyboardKey.F9))
    {
        client.Connect();
    }
    if (Raylib.IsKeyPressed(KeyboardKey.F10))
    {
        client.SendState(state);
        var receivedState = client.GetState();
        Console.WriteLine(receivedState);
    }

}

Raylib.CloseWindow();

