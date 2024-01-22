#pragma once
#include "Player.h"
#include <raylib.h>
class Hook{
private:
    Vector2 _position{ 0,0 };
	Rectangle _collision {0,0,30,30};
    float _hookPullForce = 6.0f;
    float _hookPullOffset = 3.0f;
    float _hookShootForce = 6.0f;
    float _hookSizeLimit = 200;
    bool _isHookAttached = false;
    bool _isHookReleased = false;
    float _hookTimeout = 0.2f; 
    bool _pressingHookKey = false;

public: 
	void Simulate(Player& player, Map& map);
	void Draw(Player& player);
};