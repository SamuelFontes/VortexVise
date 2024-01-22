#include "Hook.h"

void Hook::Simulate(Player& player, Map& map)
{
	if (!_pressingHookKey && IsMouseButtonDown(1)) 
	{
		// start Hook shoot
		_isHookReleased = true;
		_position = player.GetPlayerCenterPosition();
		_collision.x = _position.x;
		_collision.y = _position.y;
	} else if (_pressingHookKey && !IsMouseButtonDown(1)) 
	{
		// Hook retracted
		_isHookReleased = false;
	}
	else if (!_isHookAttached) 
	{
		// Shooting the hook
	}
	else 
	{
		// Should pull player here
	}

	_pressingHookKey = IsMouseButtonDown(1);
}

void Hook::Draw(Player& const player)
{
	if (_isHookReleased) 
	{
		DrawRectangleRec(_collision, GREEN); // Debug
	}

}
