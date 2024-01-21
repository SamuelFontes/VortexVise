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

	if (IsKeyDown(KEY_SPACE))//&& gravitationalForce == 0)
	{
		gravitationalForce = -0.24;
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

Vector2 Combatant::GetPosition() const
{
	return position;
}

float Combatant::GetX() const
{
	return position.x;
}

float Combatant::GetY() const
{
	return position.y;
}

float Combatant::GetGravitationalForce() const
{
	return gravitationalForce;
}

void Combatant::ApplyCollisions(Map* map)
{
	// Check Bottom of screen for collision

		// Get collision rectangle (only on collision)
	//float playerFeet = position.y - texture.height;
	//auto screenBottom = (screenHeight / 2 * -1);
	//if (playerFeet <= screenBottom)
	//{
	//	if (gravitationalForce > 0)
	//		gravitationalForce = 0;
	//	position.y = screenBottom + texture.height;
	//}

	for (const auto& collision : map->GetCollisions()) 
	{
		if (CheckCollisionRecs(collisionBox, collision)) 
		{
			// This means the player is inside the thing 
			auto collisionOverlap = GetCollisionRec(collisionBox, collision);
		}
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


	collisionBox = { position.x * -1 + destRec.x,position.y * -1 + destRec.y,50,50 }; // TODO: move this 
	DrawRectangleRec(collisionBox, PURPLE);
}

