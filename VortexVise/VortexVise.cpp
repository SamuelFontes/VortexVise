// VortexVise.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "raylib.h";
#include "Combatant.h"
#include "Map.h"


int main()
{
	float gravity = 900;
	int screenWidth = 1920;
	int screenHeight = 1080;
	InitWindow(screenWidth, screenHeight, "Vortex Vise");
	ToggleFullscreen();

	Map map;
	map.LoadMap("SkyArchipelago");

	Combatant player(true, map);

	//SetTargetFPS(30);               
	RenderTexture2D target = LoadRenderTexture(300, 300);


	while (!WindowShouldClose())
	{
		float deltaTime = GetFrameTime();




		player.ProcessInput(deltaTime);
		player.ApplyGravitationalForce(gravity);
		player.ApplyCollisions(map);

		BeginDrawing();
		ClearBackground(BLACK);
		player.ProcessCamera(map);

		map.Draw();
		player.Draw();

#pragma region Debug
		// DEBUG
		BeginTextureMode(target);
		ClearBackground(WHITE);
		DrawFPS(128, 12);
		DrawText(TextFormat("FPS: %02i", (int)(1 / deltaTime)), 12, 12, 20, BLACK);
		DrawText(TextFormat("player gravityForce: %04f", player.GetGravitationalForce()), 12, 32, 20, BLACK);
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

