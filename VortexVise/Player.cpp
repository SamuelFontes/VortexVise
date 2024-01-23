#include "Player.h"
#include <raymath.h>

void Player::ProcessInput(float deltaTime)
{
	if (IsKeyDown(KEY_D)) {
		m_moveSpeed += m_acceleration * deltaTime;
		if (m_moveSpeed > m_maxMoveSpeed)// && gravitationalForce == 0)
			m_moveSpeed = m_maxMoveSpeed;
		m_direction = -1;
	}
	else if (IsKeyDown(KEY_A)) {
		m_moveSpeed -= m_acceleration * deltaTime;
		if (m_moveSpeed < m_maxMoveSpeed * -1)// && gravitationalForce == 0)
			m_moveSpeed = m_maxMoveSpeed * -1;
		m_direction = 1;
	}
	else {
		float desaceleration = m_isTouchingTheGround || m_gravitationalForce == 0 ? 10 : 0.5;
		m_moveSpeed = Lerp(m_moveSpeed, 0, 1 - expf(-desaceleration * GetFrameTime()));
	}

	if (m_moveSpeed != 0)
		m_position.x += m_moveSpeed * deltaTime * -1;

	//if (IsKeyDown(KEY_SPACE)
	if (IsKeyDown(KEY_SPACE) && m_isTouchingTheGround) {
		m_isTouchingTheGround = false;
		m_gravitationalForce = -600;
	}

}

void Player::ApplyGravitationalForce(float gravity)
{
	if (!m_isTouchingTheGround) {
		m_gravitationalForce += gravity * GetFrameTime();
		m_position.y -= m_gravitationalForce * GetFrameTime();
	}
}

Vector2 Player::GetPosition() const
{
	return m_position;
}

float Player::GetX() const
{
	return m_position.x;
}

float Player::GetY() const
{
	return m_position.y;
}

float Player::GetGravitationalForce() const
{
	return m_gravitationalForce;
}

float Player::GetMoveSpeed() const
{
	return m_moveSpeed;
}

void Player::ApplyCollisions(Map& map)
{
	Vector2 collisionOffset = { 20,12 };
	m_collisionBox = { m_position.x * -1 + collisionOffset.x,m_position.y * -1 + collisionOffset.y,25,40 };
	m_isTouchingTheGround = false;

	Vector2 mapSize = map.GetMapSize();
	// Apply ouside map collisions
	if (m_collisionBox.y <= 0) {
		m_position.y = 11.9;
		m_gravitationalForce = 0;
	}
	else if (m_collisionBox.y > mapSize.y) {
		// TODO: Kill the player
		m_position = { map.GetMapSize().x / 2 * -1,map.GetMapSize().y / 2 * -1 };
		m_gravitationalForce = 0;
		m_moveSpeed = 0;

	}
	if (m_collisionBox.x <= 0) {
		m_position.x = 0 - (m_collisionBox.x * -1 - m_position.x);
		m_moveSpeed = 0;
	}
	else if (m_collisionBox.x + m_collisionBox.width >= mapSize.x) {
		m_position.x = (mapSize.x - m_collisionBox.width - collisionOffset.x) * -1;
		m_moveSpeed = 0;
	}

	// Apply map collisions
	for (const auto& collision : map.GetCollisions())
	{
		if (CheckCollisionRecs(m_collisionBox, collision)) {
			// OMG THIS WORKS :)
			// This means the player is inside the thing 
			auto collisionOverlap = GetCollisionRec(m_collisionBox, collision);

			if (m_position.y == (collision.y - m_texture.height + collisionOffset.y) * -1)
				m_isTouchingTheGround = true;
			if (collisionOverlap.height < collisionOverlap.width) {
				if (collisionOverlap.y < collision.y + collision.height / 2) {
					// Feet collision
					m_position.y = (collision.y - m_texture.height + collisionOffset.y) * -1;
					m_collisionBox.y = (collision.y - m_collisionBox.height);
					m_gravitationalForce = 0;
					m_isTouchingTheGround = true;
				}
				else {
					// Head collision
					m_position.y -= collisionOverlap.height;
					m_collisionBox.y += collisionOverlap.height;
					m_gravitationalForce = 0.01;
				}
			}
			else {
				m_moveSpeed = 0;
				if (collisionOverlap.x > collision.x) {
					// Right collision
					m_position.x -= collisionOverlap.width;
					m_collisionBox.x -= collisionOverlap.width;
				}
				else {
					// Left collision
					m_position.x += collisionOverlap.width;
					m_collisionBox.x += collisionOverlap.width;
				}

			}
		}
	}
}

void Player::ProcessCamera(Map& map)
{
	if (m_hasCamera) {
		Vector2 target = { m_position.x * -1,m_position.y * -1 };// WHY IT IS INVERTED??????????

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
		m_camera.target.x = Lerp(m_camera.target.x, target.x, 1 - expf(-3 * GetFrameTime()));
		m_camera.target.y = Lerp(m_camera.target.y, target.y, 1 - expf(-3 * GetFrameTime()));

		BeginMode2D(m_camera);
	}
}

Vector2 Player::GetPlayerCenterPosition() const
{
	Vector2 position = { m_position.x * -1,m_position.y * -1 };
	position.x += m_texture.width / 2;
	position.y += m_texture.height / 2;
	return position;
}

bool Player::IsLookingRight() const
{
	return m_direction == -1;
}

void Player::Draw()
{
	Rectangle sourceRec = Rectangle{ 0.0f, 0.0f, (float)m_texture.width * m_direction, (float)m_texture.height };

	Rectangle destRec = Rectangle{ 0, 0, (float)m_texture.width, (float)m_texture.height };

	DrawTexturePro(m_texture, sourceRec, destRec, m_position, 0, WHITE);


	//DrawRectangleRec(collisionBox, GREEN); // Debug
}

