#pragma once
#include "Player.h"
#include <raylib.h>
#include <raymath.h>
#include "Utils.h"
class Hook {
private:
	Vector2 m_position{ 0,0 };
	Vector2 m_velocity{ 0,0 };
	Rectangle m_collision{ 0,0,16,16 };
	Texture2D m_texture = LoadTexture("Resources/Sprites/GFX/hook_head.png");
	float m_hookPullForce = 2500;
	float m_hookPullOffset = 50;
	float m_hookShootForce = 2000;
	float m_hookSizeLimit = 200;
	bool m_isHookAttached = false;
	bool m_isHookReleased = false;
	float m_hookTimeout = 0.2f;
	bool m_pressingHookKey = false;

public:
	void Simulate(Player& player, Map& map, float gravity, float deltaTime);
	void Draw(Player& player);
};