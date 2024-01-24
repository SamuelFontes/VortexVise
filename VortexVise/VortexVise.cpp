// VortexVise.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "raylib.h";
#include "Player.h"
#include "Map.h"
#include "Hook.h"


int main()
{
	float gravity = 900;
	int tickrate = 512; // Even my game has more than 64 tick, suck it CSGO
	int targetFPS = 0;
	int screenWidth = 1920;
	int screenHeight = 1080;
	InitWindow(screenWidth, screenHeight, "Vortex Vise");
	ToggleFullscreen();

	Map map;
	map.LoadMap("SkyArchipelago");

	Player player(true, map);
	Hook hook;

	RenderTexture2D target = LoadRenderTexture(300, 300);


	auto currentTime = GetTime();
	auto lastTime = currentTime;
	double const deltaTime = static_cast<double>(1) / tickrate;

	while (!WindowShouldClose()) {
		if (targetFPS != 0) {
			double time = static_cast<double>(1) / targetFPS;
			WaitTime(time);
		}

		currentTime = GetTime();
		auto simulationTime = currentTime - lastTime;



		int tickCounter = 0;



		while (simulationTime >= deltaTime) // perform one update for every interval passed
		{
			player.ProcessInput(deltaTime);
			player.ApplyGravitationalForce(gravity, deltaTime);
			hook.Simulate(player, map, gravity, deltaTime);
			player.ApplyVelocity(deltaTime);
			player.ApplyCollisions(map);
			simulationTime -= deltaTime;
			lastTime += deltaTime;
			tickCounter++;
		}


		BeginDrawing();
		ClearBackground(BLACK);
		player.ProcessCamera(map);

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
		DrawText(TextFormat("tc: %02i", tickCounter), 12, 90, 20, BLACK);
		DrawText(TextFormat("player position: %02i %02i", (int)player.GetX(), (int)player.GetY()), 12, 64, 20, BLACK);
		DrawText(TextFormat("collision velocity: %f", player.GetMoveSpeed()), 12, 129, 20, BLACK);
		EndTextureMode();

		auto rec = Rectangle{ 0,0, (float)target.texture.width,(float)target.texture.height };
		DrawTexturePro(target.texture, Rectangle{ 0,0, (float)target.texture.width,(float)target.texture.height * -1 }, rec, Vector2{ 0,0 }, 0, WHITE);
#pragma endregion

		EndDrawing();

	}

	CloseWindow();
}

