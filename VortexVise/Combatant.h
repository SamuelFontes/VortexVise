#pragma once
#include <raylib.h>
#include "Map.h"
class Combatant {
public:
	// Constructor
	Combatant(bool _hasCamera) {
		texture = LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
		auto spawnPoint = Vector2{ GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f };
		position = { 30,30 };

		if (_hasCamera)
		{
			camera = Camera2D{ spawnPoint,{0,0} ,0,1 };
		}
	}
private:
	Vector2 position;
	float gravitationalForce = 0;
	int direction = 1;
	Texture2D texture;
	float moveSpeed = 0;
	float maxMoveSpeed = 700;
	float acceleration = 1500;
	Camera2D camera;
	bool hasCamera;

public:
	Rectangle collisionBox;
	void ProcessInput(float deltaTime);
	void ApplyGravitationalForce();
	void CalculateGravitationalForce(float force, float deltaTime);
	Vector2 GetPosition() const;
	float GetX() const;
	float GetY() const;
	float GetGravitationalForce() const;
	void ApplyCollisions(Map* map);
	void Draw();
	void ProcessCamera();
};
