#include "Combatant.h"

void Combatant::ProcessInput(float deltaTime)
{
	if (IsKeyDown(KEY_D))
	{
		if (direction != -1)
		{
			// Player changed direction
			moveSpeed = 0;
		}
		moveSpeed += acceleration * deltaTime;
		if (moveSpeed > maxMoveSpeed && gravitationalForce == 0)
			moveSpeed = maxMoveSpeed;
		direction = -1;
	}
	else if (IsKeyDown(KEY_A))
	{
		if (direction != 1)
		{
			// Player changed direction
			moveSpeed = 0;
		}
		moveSpeed += acceleration * deltaTime;
		if (moveSpeed > maxMoveSpeed && gravitationalForce == 0)
			moveSpeed = maxMoveSpeed;
		direction = 1;
	}
	else
	{
		if (gravitationalForce != 0)
			moveSpeed -= acceleration * deltaTime / 5;
		else
			moveSpeed -= acceleration * deltaTime * 2;
		if (moveSpeed < 0)
		{
			moveSpeed = 0;
		}
	}

	if (direction == 1)
	{
		position.x += moveSpeed * deltaTime;
	}
	else
	{
		position.x -= moveSpeed * deltaTime;
	}

	if (IsKeyDown(KEY_SPACE) && gravitationalForce == 0)
	{
		gravitationalForce = -0.2;
	}

}

void Combatant::CalculateGravitationalForce(float force, float deltaTime)
{
	gravitationalForce += force * deltaTime;
}

void Combatant::ApplyGravitationalForce()
{
	position.y -= gravitationalForce;
}

Vector2 Combatant::GetPosition()
{
	return position;
}

float Combatant::GetX()
{
	return position.x;
}

float Combatant::GetY()
{
	return position.y;
}

float Combatant::GetGravitationalForce()
{
	return gravitationalForce;
}

void Combatant::CheckCollisions(int screenHeight)
{
	// Check Bottom of screen for collision
	float playerFeet = position.y - texture.height;
	auto screenBottom = (screenHeight / 2 * -1);
	if (playerFeet <= screenBottom)
	{
		if (gravitationalForce > 0)
			gravitationalForce = 0;
		position.y = screenBottom + texture.height;
	}
}

void Combatant::ProcessCamera()
{
	if (hasCamera) {
		camera.offset = position;
		BeginMode2D(camera);
	}
}

void Combatant::Draw(int screenWidth, int screenHeight)
{
	Rectangle sourceRec = Rectangle{ 0.0f, 0.0f, (float)texture.width * direction, (float)texture.height };

	Rectangle destRec = Rectangle{ screenWidth / 2.0f, screenHeight / 2.0f, (float)texture.width, (float)texture.height };

	DrawTexturePro(texture, sourceRec, destRec, position, 0, WHITE);
}

