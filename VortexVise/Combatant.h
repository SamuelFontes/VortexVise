#pragma once
#include <raylib.h>
class Combatant {
public:
	// Constructor
	Combatant() {
		texture = LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
		position = Vector2{ 0,0 };
	}
private: 
	Vector2 position;
	float gravitationalForce = 0;
	int direction = 1;
	Texture2D texture;
	float moveSpeed = 0;
	float maxMoveSpeed = 700;
	float acceleration = 1500;

public:
	void ProcessInput(float deltaTime);
	void ApplyGravitationalForce();
	void CalculateGravitationalForce(float force, float deltaTime);
	Vector2 GetPosition();
	float GetX();
	float GetY();
	float GetGravitationalForce();
	void CheckCollisions(int screenHeight);
	void Draw(int screenWidth, int screenHeight);
};
