#include "Combatant.h"
#include <raymath.h>

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
		if (moveSpeed > maxMoveSpeed)// && gravitationalForce == 0)
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
		if (moveSpeed > maxMoveSpeed)// && gravitationalForce == 0)
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
	Vector2 collisionOffset = { 20,12 };
	collisionBox = { position.x * -1 + collisionOffset.x,position.y * -1 + collisionOffset.y,25,40 };

	Vector2 mapSize = map->GetMapSize();
	// Apply ouside map collisions
	if (collisionBox.y <= 0)
	{
		position.y = 11.9;
		gravitationalForce = 0;
	}
	else if (collisionBox.y > mapSize.y) {
		// TODO: Kill the player

	}
	if (collisionBox.x <= 0)
	{
		position.x = 0 - (collisionBox.x * -1 - position.x);
		moveSpeed = 0;
	}
	else if (collisionBox.x + collisionBox.width >= mapSize.x)
	{
		position.x = (mapSize.x - collisionBox.width - collisionOffset.x) * -1;
		moveSpeed = 0;
	}

	// Apply map collisions
	for (const auto& collision : map->GetCollisions())
	{
		if (CheckCollisionRecs(collisionBox, collision))
		{
			// OMG THIS WORKS :)
			// This means the player is inside the thing 
			auto collisionOverlap = GetCollisionRec(collisionBox, collision);

			if (collisionOverlap.height < collisionOverlap.width)
			{
				gravitationalForce = 0;
				if (collisionOverlap.y < collision.y + collision.height / 2)
				{
					// Feet collision
					position.y += collisionOverlap.height;
					collisionBox.y += collisionOverlap.height;
				}
				else
				{
					// Head collision
					position.y -= collisionOverlap.height;
					collisionBox.y -= collisionOverlap.height;
				}
			}
			else
			{
				moveSpeed = 0;
				if (collisionOverlap.x > collision.x)
				{
					// Right collision
					position.x -= collisionOverlap.width;
					collisionBox.x -= collisionOverlap.width;
				}
				else
				{
					// Left collision
					position.x += collisionOverlap.width;
					collisionBox.x += collisionOverlap.width;
				}

			}
		}
	}
}

void Combatant::ProcessCamera(Map* map)
{
	if (hasCamera) {
		Vector2 target = { position.x * -1,position.y * -1 };// WHY IT IS INVERTED??????????

		// Make it stay inside the map
		if (target.x - GetScreenWidth() / 2 <= 0)
			target.x = GetScreenWidth() / 2;
		else if (target.x + GetScreenWidth() / 2 >= map->GetMapSize().x)
			target.x = map->GetMapSize().x - GetScreenWidth() / 2;

		if (target.y - GetScreenHeight() / 2 <= 0)
			target.y = GetScreenHeight() / 2;
		else if (target.y + GetScreenHeight() / 2 >= map->GetMapSize().y)
			target.y = map->GetMapSize().y - GetScreenHeight() / 2;

		// Make camera smooth
		camera.target.x = Lerp(camera.target.x, target.x,1 - expf(-3 * GetFrameTime()));
		camera.target.y = Lerp(camera.target.y, target.y,1 - expf(-3 * GetFrameTime()));

		BeginMode2D(camera);
	}
}

void Combatant::Draw()
{
	Rectangle sourceRec = Rectangle{ 0.0f, 0.0f, (float)texture.width * direction, (float)texture.height };

	Rectangle destRec = Rectangle{ 0, 0, (float)texture.width, (float)texture.height };

	DrawTexturePro(texture, sourceRec, destRec, position, 0, WHITE);


	DrawRectangleRec(collisionBox, PURPLE); // Debug
}

