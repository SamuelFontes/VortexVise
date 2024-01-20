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

	Combatant* player = new Combatant;
	Camera2D camera = { 0 };
	camera.target = player->GetPosition();
	camera.offset = Vector2{ screenWidth / 2.0f, screenHeight / 2.0f };
	camera.rotation = 0.0f;
	camera.zoom = 1;

	//SetTargetFPS(60);               // TODO: Implement deltatime 

	while (!WindowShouldClose())
	{
		float deltaTime = GetFrameTime();

		DrawText(TextFormat("FPS: %02i", (int)(1 / deltaTime)), 12, 12, 20, BLACK);
		DrawText(TextFormat("gravityForce: %04f", player->GetGravitationalForce()), 12, 32, 20, BLACK);
		DrawText(TextFormat("position: %02i %02i", (int)player->GetX(), (int)player->GetY()), 12, 64, 20, BLACK);
		DrawText(TextFormat("camera-target: %02i %02i", (int)camera.target.x, (int)camera.target.y), 12, 80, 20, BLACK);
		DrawText(TextFormat("cameraOffset: %02i %02i", (int)camera.offset.x, (int)camera.offset.y), 12, 100, 20, BLACK);

		player->CalculateGravitationalForce(gravity, deltaTime);

		player->CheckCollisions(screenHeight);
		player->ApplyGravitationalForce();

		player->ProcessInput(deltaTime);
		camera.offset = player->GetPosition();
		//camera.target = Vector2{ player->GetX()+20,player->GetY()+20 };

		BeginDrawing();
		ClearBackground(WHITE);
		BeginMode2D(camera);

		player->Draw(screenWidth, screenHeight);

		EndDrawing();
	}

	CloseWindow();
}

