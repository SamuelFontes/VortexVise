// VortexVise.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <cstring>
#include "raylib.h"
#include "Player.h"
#include "Map.h"
#include "Hook.h"

int main(int argc, char *argv[])
{
	if (argc > 1)
	{
		auto argument = argv[1];
		if (std::strcmp(argument, "server") == 0)
		{
			std::cout << "VortexVise Server Started!" << std::endl;
			// TODO: Run server
			return 0;
		}
	}

	float gravity = 900;
	int tickrate = 64;
	int screenWidth = 1920;
	int screenHeight = 1080;
	InitWindow(screenWidth, screenHeight, "Vortex Vise");
	ToggleFullscreen();
	DisableCursor();

	Map map;
	map.LoadMap("SkyArchipelago");

	Player player(true, map);
	Hook hook;

	RenderTexture2D target = LoadRenderTexture(300, 300);

	auto currentTime = GetTime();
	auto lastTime = currentTime;
	auto lastTimeAccumulator = currentTime;
	double const deltaTime = static_cast<double>(1) / tickrate;

	int tickCounter = 0;
	int renderCounter = 0;
	double accumulator = 0;
	while (!WindowShouldClose())
	{
		bool isSlowerThanTickRate = false;
		int targetFPS = Utils::GetFPS();
		if (targetFPS != 0)
		{
			double time = static_cast<double>(1) / targetFPS;
			WaitTime(time);
		}

		currentTime = GetTime();
		double simulationTime = currentTime - lastTime;

		Input input = player.GetInput();	// Only thing we send to the server here
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
			player.ProcessInput(deltaTime - accumulator, input);
			player.ApplyGravitationalForce(gravity, deltaTime - accumulator);
			hook.Simulate(player, map, gravity, deltaTime - accumulator, input);
			player.ApplyVelocity(deltaTime - accumulator);
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
			player.ProcessInput(accumulatorSimulationTime, input);
			player.ApplyGravitationalForce(gravity, accumulatorSimulationTime);
			hook.Simulate(player, map, gravity, accumulatorSimulationTime, input);
			player.ApplyVelocity(accumulatorSimulationTime);
			player.ApplyCollisions(map);
			lastTimeAccumulator = currentTime;
		}

		BeginDrawing();
		ClearBackground(BLACK);
		player.ProcessCamera(map);

		renderCounter++;
		map.Draw();
		hook.Draw(player);
		player.Draw();

#pragma region Debug
		// DEBUG
		BeginTextureMode(target);
		ClearBackground(WHITE);
		DrawFPS(128, 12);
		DrawText(TextFormat("dt: %02i", (int)(1 / deltaTime)), 12, 12, 20, BLACK);
		DrawText(TextFormat("player gravityForce: %04f", player.GetGravitationalForce()), 12, 32, 20, BLACK);
		DrawText(TextFormat("tc: %02i %02i", tickCounter, renderCounter), 12, 90, 20, BLACK);
		DrawText(TextFormat("player position: %02i %02i", (int)player.GetX(), (int)player.GetY()), 12, 64, 20, BLACK);
		DrawText(TextFormat("collision velocity: %f", player.GetMoveSpeed()), 12, 129, 20, BLACK);
		EndTextureMode();

		auto rec = Rectangle{0, 0, (float)target.texture.width, (float)target.texture.height};
		DrawTexturePro(target.texture, Rectangle{0, 0, (float)target.texture.width, (float)target.texture.height * -1}, rec, Vector2{0, 0}, 0, WHITE);
#pragma endregion

		EndDrawing();
		if (IsKeyPressed(KEY_F7))
		{

			Utils::SwitchDebug();
		}

		if (IsKeyPressed(KEY_F8))
		{

			Utils::UnlockFPS();
		}
	}

	CloseWindow();
}
