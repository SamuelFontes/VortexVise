// VortexVise.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "raylib.h";
#include "Combatant.h"
#include "Map.h"

float gravity = 0.5;

int main()
{
	int screenWidth = 1920;
	int screenHeight = 1080;
	InitWindow(screenWidth, screenHeight, "Vortex Vise");
	ToggleFullscreen();

	Map* map = new Map();
	map->LoadMap("SkyArchipelago");

	Combatant* player = new Combatant(true);

	//SetTargetFPS(60);               
	RenderTexture2D target = LoadRenderTexture(300, 300);


	while (!WindowShouldClose())
	{
		float deltaTime = GetFrameTime();


		player->CalculateGravitationalForce(gravity, deltaTime);

		player->ApplyCollisions(map);
		player->ApplyGravitationalForce();

		player->ProcessInput(deltaTime);

		BeginDrawing();
		ClearBackground(BLACK);
		player->ProcessCamera(map);

		map->Draw();
		player->Draw();

#pragma region Debug
		// DEBUG
		BeginTextureMode(target);
		ClearBackground(WHITE);
		DrawFPS(128, 12);
		DrawText(TextFormat("FPS: %02i", (int)(1 / deltaTime)), 12, 12, 20, BLACK);
		DrawText(TextFormat("player gravityForce: %04f", player->GetGravitationalForce()), 12, 32, 20, BLACK);
		DrawText(TextFormat("player position: %02i %02i", (int)player->GetX(), (int)player->GetY()), 12, 64, 20, BLACK);
		DrawText(TextFormat("collision position: %02i %02i", (int)player->collisionBox.x, (int)player->collisionBox.y), 12, 129, 20, BLACK);
		EndTextureMode();

		auto rec = Rectangle{ 0,0, (float)target.texture.width,(float)target.texture.height };
		DrawTexturePro(target.texture, Rectangle{ 0,0, (float)target.texture.width,(float)target.texture.height * -1 }, rec, Vector2{ 0,0 }, 0, WHITE);
#pragma endregion

		EndDrawing();

	}

	CloseWindow();
}

