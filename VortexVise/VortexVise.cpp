// VortexVise.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "raylib.h";
#include "Combatant.h"

float gravity = 0.5;

int main()
{
	int screenWidth = 1920;
	int screenHeight = 1080;
	InitWindow(screenWidth, screenHeight, "Vortex Vise");
	ToggleFullscreen();

	Combatant* player = new Combatant(true);

	auto map = LoadTexture("Resources/Sprites/Maps/SkyArchipelago.png");
	//SetTargetFPS(60);               
	RenderTexture2D target = LoadRenderTexture(300, 300);


	while (!WindowShouldClose())
	{
		float deltaTime = GetFrameTime();


		player->CalculateGravitationalForce(gravity, deltaTime);

		player->CheckCollisions(screenHeight);
		player->ApplyGravitationalForce();

		player->ProcessInput(deltaTime);

		BeginDrawing();
		ClearBackground(WHITE);
		player->ProcessCamera();

		DrawTextureEx(map, Vector2{ 0, 0 }, 0, 0.27, WHITE);
		player->Draw(screenWidth, screenHeight);

		// DEBUG
		BeginTextureMode(target);
		ClearBackground(WHITE);
		DrawFPS(128, 12);
		DrawText(TextFormat("FPS: %02i", (int)(1 / deltaTime)), 12, 12, 20, BLACK);
		DrawText(TextFormat("gravityForce: %04f", player->GetGravitationalForce()), 12, 32, 20, BLACK);
		DrawText(TextFormat("position: %02i %02i", (int)player->GetX(), (int)player->GetY()), 12, 64, 20, BLACK);
		EndTextureMode();

		auto rec = Rectangle{0,0, (float)target.texture.width,(float)target.texture.height };
		DrawTexturePro(target.texture,Rectangle{0,0, (float)target.texture.width,(float)target.texture.height * -1}, rec, Vector2{0,0}, 0, WHITE);
		EndDrawing();

	}

	CloseWindow();
}

