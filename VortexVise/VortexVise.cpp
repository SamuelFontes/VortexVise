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

	//SetTargetFPS(60);               // TODO: Implement deltatime 

	while (!WindowShouldClose())
	{
		float deltaTime = GetFrameTime();

		DrawText(TextFormat("FPS: %02i", (int)(1 / deltaTime)), 12, 12, 20, BLACK);
		DrawText(TextFormat("gravityForce: %04f", player->GetGravitationalForce()), 12, 32, 20, BLACK);
		DrawText(TextFormat("position: %02i %02i", (int)player->GetX(), (int)player->GetY()), 12, 64, 20, BLACK);

		player->CalculateGravitationalForce(gravity, deltaTime);

		player->CheckCollisions(screenHeight);
		player->ApplyGravitationalForce();

		player->ProcessInput(deltaTime);

		BeginDrawing();
		ClearBackground(WHITE);
		player->ProcessCamera();

		player->Draw(screenWidth, screenHeight);

		EndDrawing();
	}

	CloseWindow();
}

