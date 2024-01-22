#pragma once
#include <raylib.h>
#include "Map.h"
class Combatant {
public:
	// Constructor
	Combatant(bool _hasCamera, Map* map) {
		texture = LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
		auto spawnPoint = Vector2{ GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f };
		position = { spawnPoint.x * -1, spawnPoint.y * -1 };

		if (_hasCamera)
		{
			camera = Camera2D{ spawnPoint, spawnPoint ,0,1 };
		}
	}
private:
	Vector2 position;
	int direction = 1;
	Texture2D texture;
	float maxMoveSpeed = 700;
	float acceleration = 1500;
	Camera2D camera;
	bool hasCamera;
	bool isTouchingTheGround = false;
	Rectangle collisionBox;

public:
	float moveSpeed = 0;
	float gravitationalForce = 0;
	void ProcessInput(float deltaTime);
	void ApplyGravitationalForce(float gravity);
	Vector2 GetPosition() const;
	float GetX() const;
	float GetY() const;
	float GetGravitationalForce() const;
	void ApplyCollisions(Map* map);
	void Draw();
	void ProcessCamera(Map* map);
};
