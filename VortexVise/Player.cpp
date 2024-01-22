#include "Player.h"
#include <raymath.h>

void Player::ProcessInput(float deltaTime)
{
	if (IsKeyDown(KEY_D))
	{
		if (_direction != -1 && _isTouchingTheGround)
		{
			// Player changed direction
			_moveSpeed = 0;
		}
		_moveSpeed += _acceleration * deltaTime;
		if (_moveSpeed > _maxMoveSpeed)// && gravitationalForce == 0)
			_moveSpeed = _maxMoveSpeed;
		_direction = -1;
	}
	else if (IsKeyDown(KEY_A))
	{
		if (_direction != 1 && _isTouchingTheGround)
		{
			// Player changed direction
			_moveSpeed = 0;
		}
		_moveSpeed -= _acceleration * deltaTime;
		if (_moveSpeed < _maxMoveSpeed * -1)// && gravitationalForce == 0)
			_moveSpeed = _maxMoveSpeed * -1;
		_direction = 1;
	}
	else
	{
		float desaceleration = _isTouchingTheGround || _gravitationalForce == 0 ? 10 : 0.5;
		_moveSpeed = Lerp(_moveSpeed, 0, 1 - expf(-desaceleration * GetFrameTime()));
	}

	if (_moveSpeed != 0)
		_position.x += _moveSpeed * deltaTime * -1;

	//if (IsKeyDown(KEY_SPACE)
	if (IsKeyDown(KEY_SPACE) && _isTouchingTheGround)
	{
		_isTouchingTheGround = false;
		_gravitationalForce = -600;
	}

}

void Player::ApplyGravitationalForce(float gravity)
{
	if (!_isTouchingTheGround)
	{
		_gravitationalForce += gravity * GetFrameTime();
		_position.y -= _gravitationalForce * GetFrameTime();
	}
}

Vector2 Player::GetPosition() const
{
	return _position;
}

float Player::GetX() const
{
	return _position.x;
}

float Player::GetY() const
{
	return _position.y;
}

float Player::GetGravitationalForce() const
{
	return _gravitationalForce;
}

float Player::GetMoveSpeed() const
{
	return _moveSpeed;
}

void Player::ApplyCollisions(Map& map)
{
	Vector2 collisionOffset = { 20,12 };
	_collisionBox = { _position.x * -1 + collisionOffset.x,_position.y * -1 + collisionOffset.y,25,40 };
	_isTouchingTheGround = false;

	Vector2 mapSize = map.GetMapSize();
	// Apply ouside map collisions
	if (_collisionBox.y <= 0)
	{
		_position.y = 11.9;
		_gravitationalForce = 0;
	}
	else if (_collisionBox.y > mapSize.y) {
		// TODO: Kill the player
		_position = { map.GetMapSize().x / 2 * -1,map.GetMapSize().y / 2 * -1 };
		_gravitationalForce = 0;
		_moveSpeed = 0;

	}
	if (_collisionBox.x <= 0)
	{
		_position.x = 0 - (_collisionBox.x * -1 - _position.x);
		_moveSpeed = 0;
	}
	else if (_collisionBox.x + _collisionBox.width >= mapSize.x)
	{
		_position.x = (mapSize.x - _collisionBox.width - collisionOffset.x) * -1;
		_moveSpeed = 0;
	}

	// Apply map collisions
	for (const auto& collision : map.GetCollisions())
	{
		if (CheckCollisionRecs(_collisionBox, collision))
		{
			// OMG THIS WORKS :)
			// This means the player is inside the thing 
			auto collisionOverlap = GetCollisionRec(_collisionBox, collision);

			if (_position.y == (collision.y - _texture.height + collisionOffset.y) * -1)
				_isTouchingTheGround = true;
			if (collisionOverlap.height < collisionOverlap.width)
			{
				if (collisionOverlap.y < collision.y + collision.height / 2)
				{
					// Feet collision
					_position.y = (collision.y - _texture.height + collisionOffset.y) * -1;
					_collisionBox.y = (collision.y - _collisionBox.height);
					_gravitationalForce = 0;
					_isTouchingTheGround = true;
				}
				else
				{
					// Head collision
					_position.y -= collisionOverlap.height;
					_collisionBox.y += collisionOverlap.height;
					_gravitationalForce = 0.01;
				}
			}
			else
			{
				_moveSpeed = 0;
				if (collisionOverlap.x > collision.x)
				{
					// Right collision
					_position.x -= collisionOverlap.width;
					_collisionBox.x -= collisionOverlap.width;
				}
				else
				{
					// Left collision
					_position.x += collisionOverlap.width;
					_collisionBox.x += collisionOverlap.width;
				}

			}
		}
	}
}

void Player::ProcessCamera(Map& map)
{
	if (_hasCamera) {
		Vector2 target = { _position.x * -1,_position.y * -1 };// WHY IT IS INVERTED??????????

		// Make it stay inside the map
		if (target.x - GetScreenWidth() / 2 <= 0)
			target.x = GetScreenWidth() / 2;
		else if (target.x + GetScreenWidth() / 2 >= map.GetMapSize().x)
			target.x = map.GetMapSize().x - GetScreenWidth() / 2;

		if (target.y - GetScreenHeight() / 2 <= 0)
			target.y = GetScreenHeight() / 2;
		else if (target.y + GetScreenHeight() / 2 >= map.GetMapSize().y)
			target.y = map.GetMapSize().y - GetScreenHeight() / 2;

		// Make camera smooth
		// FIXME: fix camera jerkness when almost hitting the target
		_camera.target.x = Lerp(_camera.target.x, target.x, 1 - expf(-3 * GetFrameTime()));
		_camera.target.y = Lerp(_camera.target.y, target.y, 1 - expf(-3 * GetFrameTime()));

		BeginMode2D(_camera);
	}
}

Vector2 Player::GetPlayerCenterPosition() const
{
	Vector2 position = { _position.x * -1,_position.y * -1 };
	position.x += _texture.width / 2;
	position.y += _texture.height / 2;
	return position;
}

void Player::Draw()
{
	Rectangle sourceRec = Rectangle{ 0.0f, 0.0f, (float)_texture.width * _direction, (float)_texture.height };

	Rectangle destRec = Rectangle{ 0, 0, (float)_texture.width, (float)_texture.height };

	DrawTexturePro(_texture, sourceRec, destRec, _position, 0, WHITE);


	//DrawRectangleRec(collisionBox, GREEN); // Debug
}

