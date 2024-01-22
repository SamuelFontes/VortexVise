#pragma once
#include <raylib.h>
#include "Map.h"
class Combatant {
public:
	// Constructor
	Combatant(bool hasCamera, Map& map) {
		_texture = LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
		auto spawnPoint = Vector2{ GetScreenWidth() / 2.0f, GetScreenHeight() / 2.0f };
		_position = { spawnPoint.x * -1, spawnPoint.y * -1 };

		if (hasCamera)
		{
			_hasCamera = hasCamera;
			_camera = Camera2D{ spawnPoint, spawnPoint ,0,1 };
		}
	}
private:
	Vector2 _position;
	int _direction = 1;
	Texture2D _texture;
	float _maxMoveSpeed = 700;
	float _acceleration = 1500;
	Camera2D _camera;
	bool _hasCamera = false;
	bool _isTouchingTheGround = false;
	Rectangle _collisionBox;
	float _moveSpeed = 0;
	float _gravitationalForce = 0;

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
};
