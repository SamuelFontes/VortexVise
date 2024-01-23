#pragma once
#include <raylib.h>
#include "Map.h"
class Player {
public:
	// Constructor
	Player(bool hasCamera, Map& map) {
		m_texture = LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
		auto spawnPoint = Vector2{ GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f };
		m_position = { spawnPoint.x * -1, spawnPoint.y * -1 };

		if (hasCamera)
		{
			m_hasCamera = hasCamera;
			m_camera = Camera2D{ spawnPoint, spawnPoint ,0,1 };
		}
	}
private:
	Vector2 m_position{ 0,0 };
	Vector2 m_velocity{ 0,0 };
	int m_direction = 1;
	Texture2D m_texture;
	float m_maxMoveSpeed = 350;
	float m_acceleration = 750;
	Camera2D m_camera;
	bool m_hasCamera = false;
	bool m_isTouchingTheGround = false;
	Rectangle m_collisionBox;

public:
	void ProcessInput(float deltaTime);
	void ApplyGravitationalForce(float gravity);
	Vector2 GetPosition() const;
	float GetX() const;
	float GetY() const;
	float GetGravitationalForce() const;
	float GetMoveSpeed() const;
	void ApplyCollisions(Map& map);
	void Draw();
	void ProcessCamera(Map& map);
	Vector2 GetPlayerCenterPosition() const;
	bool IsLookingRight() const;
	void AddVelocity(Vector2 velocity);
	void ApplyVelocity();
};
