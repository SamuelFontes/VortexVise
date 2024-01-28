using Raylib_cs;
using System.Net.Sockets;
using System.Net;
using System.Numerics;
using System.Text;
using VortexVise.GameObjects;
using VortexVise.Utilities;
using System.Text.Json;
using VortexVise.States;
using VortexVise.Logic;

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
var lastTime = currentTime;
var lastTimeAccumulator = currentTime;
double deltaTime = 1d / tickrate;

int tickCounter = 0;
double accumulator = 0;

GameState lastState = new();
lastState.CurrentTime = currentTime;
lastState.Gravity = gravity;
Guid playerId = Guid.NewGuid();
lastState.PlayerStates.Add(new(playerId));

List<GameState> gameStates = new List<GameState>();
gameStates.Add(lastState);
PlayerLogic.Init();
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

    GameState state = new GameState();
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
        tickCounter++;
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
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    PlayerLogic.ProcessCamera(state.PlayerStates.FirstOrDefault(p => p.Id == playerId).Position);
    GameLogic.DrawState(state);
/*
    #region Debug
    // DEBUG
    Raylib.BeginTextureMode(target);
    Raylib.ClearBackground(Color.White);
    Raylib.DrawFPS(128, 12);
    Raylib.DrawText("dt: " + (int)(1 / deltaTime), 12, 12, 20, Color.Black);
    //Raylib.DrawText("player gravityForce: " + player.GetGravitationalForce(), 12, 32, 20, Color.Black);
    Raylib.DrawText($"tc: {tickCounter} {renderCounter}", 12, 90, 20, Color.Black);
    //Raylib.DrawText($"player position: {(int)player.GetX()} {(int)player.GetY()}", 12, 64, 20, Color.Black);
    //Raylib.DrawText($"collision velocity:{player.GetMoveSpeed()}", 12, 129, 20, Color.Black);
    Raylib.EndTextureMode();

    var rec = new Rectangle() { X = 0, Y = 0, Width = (float)target.Texture.Width, Height = (float)target.Texture.Height };
    Raylib.DrawTexturePro(target.Texture, new Rectangle(0, 0, (float)target.Texture.Width, (float)target.Texture.Height * -1), rec, new Vector2(0, 0), 0, Color.White);
    #endregion
*/
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
        // This constructor arbitrarily assigns the local port number.
        UdpClient udpClient = new UdpClient(11000);
        try
        {
            udpClient.Connect("localhost", 9050);

            // Sends a message to the host to which you have connected.
            state.PrepareSerialization();
            string json = JsonSerializer.Serialize(state);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(json);

            udpClient.Send(sendBytes, sendBytes.Length);


            //IPEndPoint object will allow us to read datagrams sent from any source.
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Blocks until a message returns on this socket from a remote host.
            Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receiveBytes);

            // Uses the IPEndPoint object to determine which of these two hosts responded.
            Console.WriteLine("This is the message you received " +
                                         returnData.ToString());
            Console.WriteLine("This message was sent from " +
                                        RemoteIpEndPoint.Address.ToString() +
                                        " on their port number " +
                                        RemoteIpEndPoint.Port.ToString());

            udpClient.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    gameStates.Add(state);
    lastState = state;
}

Raylib.CloseWindow();

