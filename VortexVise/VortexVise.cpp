// VortexVise.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "raylib.h";

float gravity = 0.5;

typedef struct Player {
	Vector2 position;
	float speed;
	float gravitacionalForce;
	bool canJump;
	int direction;
	Texture2D texture;
	float moveSpeed;
	float maxMoveSpeed;
	float acceleration;
} Player;

int main()
{
	int screenWidth = 1920;
	int screenHeight = 1080;
	InitWindow(screenWidth, screenHeight, "Vortex Vise");
	ToggleFullscreen();

	Player player = { 0 };

	player.texture = LoadTexture("Resources/Sprites/Skins/fatso.png");

	player.position = Vector2{ 0,0 };
	//SetTargetFPS(60);               // TODO: Implement deltatime 


	player.gravitacionalForce = 0;
	player.direction = 1;
	player.moveSpeed = 0;
	player.maxMoveSpeed = 600;
	player.acceleration = 1500;

	while (!WindowShouldClose())
	{
		float deltaTime = GetFrameTime();

		DrawText(TextFormat("FPS: %02i",(int)(1/deltaTime)), 12, 12, 20, BLACK);
		DrawText(TextFormat("gravityForce: %04f",player.gravitacionalForce), 12, 32, 20, BLACK);
		DrawText(TextFormat("position: %02i %02i",(int)player.position.x, (int)player.position.y), 12, 64, 20, BLACK);

		player.gravitacionalForce += gravity * deltaTime;
		auto playerFeet = player.position.y - player.texture.height; 

		auto screenBottom = (screenHeight / 2 * -1);
		if(playerFeet <= screenBottom )
		{
			if(player.gravitacionalForce > 0)
				player.gravitacionalForce = 0;
			player.position.y = screenBottom + player.texture.height;
		}
		player.position.y -= player.gravitacionalForce;

		if (IsKeyDown(KEY_D))
		{
			if(player.direction != -1)
			{
				// Player changed direction
				player.moveSpeed = 0;
			}
			player.moveSpeed += player.acceleration * deltaTime;
			if (player.moveSpeed > player.maxMoveSpeed)
				player.moveSpeed = player.maxMoveSpeed;
			player.direction = -1;
		} else if (IsKeyDown(KEY_A))
		{
			if(player.direction != 1)
			{
				// Player changed direction
				player.moveSpeed = 0;
			}
			player.moveSpeed += player.acceleration * deltaTime;
			if (player.moveSpeed > player.maxMoveSpeed)
				player.moveSpeed = player.maxMoveSpeed;
			player.direction = 1;
		}
		else
		{
			player.moveSpeed -= player.acceleration * deltaTime * 3;  
			if(player.moveSpeed < 0)
			{
				player.moveSpeed = 0;
			}
		}

		if(player.direction == 1)
		{
			player.position.x += player.moveSpeed * deltaTime;
		}
		else
		{
			player.position.x -= player.moveSpeed * deltaTime;
		}

		if (IsKeyDown(KEY_SPACE))
		{
			//player.position.y += 1500 * deltaTime;
			player.gravitacionalForce = -(1000 * deltaTime);
		}

		BeginDrawing();
		ClearBackground(WHITE);

		Rectangle sourceRec = Rectangle{ 0.0f, 0.0f, (float)player.texture.width * player.direction, (float)player.texture.height};

		Rectangle destRec = Rectangle{ screenWidth/2.0f, screenHeight/2.0f, (float)player.texture.width, (float)player.texture.height};

		DrawTexturePro(player.texture,sourceRec,destRec, player.position,0, WHITE);

		EndDrawing();
	}

	CloseWindow();
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
