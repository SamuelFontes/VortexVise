﻿using Raylib_cs;
using System.Numerics;
using VortexVise.GameObjects;
using VortexVise.Models;
using VortexVise.Utilities;

float gravity = 900;
int tickrate = 64;
int screenWidth = 1920;
int screenHeight = 1080;
Raylib.InitWindow(screenWidth, screenHeight, "Vortex Vise");
Raylib.ToggleFullscreen();
Raylib.DisableCursor();

Map map = new();
map.LoadMap("SkyArchipelago");

Player player = new(true, map);
Hook hook = new();

RenderTexture2D target = Raylib.LoadRenderTexture(300, 300);

double currentTime = Raylib.GetTime();
var lastTime = currentTime;
var lastTimeAccumulator = currentTime;
double deltaTime = 1d / tickrate;

int tickCounter = 0;
int renderCounter = 0;
double accumulator = 0;
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

    Input input = player.GetInput();    // Only thing we send to the server here
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
        player.ProcessInput((float)(deltaTime - accumulator), input);
        player.ApplyGravitationalForce(gravity, (float)(deltaTime - accumulator));
        hook.Simulate(player, map, gravity, (float)(deltaTime - accumulator), input);
        player.ApplyVelocity((float)(deltaTime - accumulator));
        player.ApplyCollisions(map);
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
        player.ProcessInput((float)accumulatorSimulationTime, input);
        player.ApplyGravitationalForce(gravity, (float)accumulatorSimulationTime);
        hook.Simulate(player, map, gravity, (float)accumulatorSimulationTime, input);
        player.ApplyVelocity((float)accumulatorSimulationTime);
        player.ApplyCollisions(map);
        lastTimeAccumulator = currentTime;
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    player.ProcessCamera(map);

    renderCounter++;
    map.Draw();
    hook.Draw(player);
    player.Draw();

    #region Debug
    // DEBUG
    Raylib.BeginTextureMode(target);
    Raylib.ClearBackground(Color.White);
    Raylib.DrawFPS(128, 12);
    Raylib.DrawText("dt: " + (int)(1 / deltaTime), 12, 12, 20, Color.Black);
    Raylib.DrawText("player gravityForce: " + player.GetGravitationalForce(), 12, 32, 20, Color.Black);
    Raylib.DrawText($"tc: {tickCounter} {renderCounter}", 12, 90, 20, Color.Black);
    Raylib.DrawText($"player position: {(int)player.GetX()} {(int)player.GetY()}", 12, 64, 20, Color.Black);
    Raylib.DrawText($"collision velocity:{player.GetMoveSpeed()}", 12, 129, 20, Color.Black);
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
}

Raylib.CloseWindow();

