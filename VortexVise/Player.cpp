#include "Player.h"

void Player::ProcessInput(float deltaTime)
{
	if (IsKeyDown(KEY_D)) {
		m_velocity.x += m_acceleration * deltaTime;
		if (m_velocity.x > m_maxMoveSpeed)// && gravitationalForce == 0)
			m_velocity.x = m_maxMoveSpeed;
		m_direction = -1;
	}
	else if (IsKeyDown(KEY_A)) {
		m_velocity.x -= m_acceleration * deltaTime;
		if (m_velocity.x < m_maxMoveSpeed * -1)// && gravitationalForce == 0)
			m_velocity.x = m_maxMoveSpeed * -1;
		m_direction = 1;
	}
	else {
		float desaceleration = m_isTouchingTheGround || m_velocity.y == 0 ? 10 : 0.5;
		m_velocity.x = Lerp(m_velocity.x, 0, 1 - expf(-desaceleration * deltaTime));
	}

	if (m_velocity.x != 0)
		m_position.x += m_velocity.x * deltaTime;

	//if (IsKeyDown(KEY_SPACE)
	if (IsKeyDown(KEY_SPACE) && m_isTouchingTheGround) {
		m_isTouchingTheGround = false;
		m_velocity.y = -400;
	}

}

void Player::ApplyGravitationalForce(float gravity, float deltaTime)
{
	float maxGravity = 500;
	if (!m_isTouchingTheGround) {
		m_velocity.y += gravity * deltaTime;
		if (m_velocity.y >= maxGravity)
			m_velocity.y = maxGravity;
		m_position.y += m_velocity.y * deltaTime;
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
	return m_velocity.y;
}

float Player::GetMoveSpeed() const
{
	return m_velocity.x;
}

void Player::ApplyCollisions(Map& map)
{
	Vector2 collisionOffset = { 20,12 };
	Rectangle endingCollision = { m_position.x + collisionOffset.x,m_position.y + collisionOffset.y,25,40 };
	m_isTouchingTheGround = false;

	Vector2 mapSize = map.GetMapSize();
	// Apply ouside map collisions
	if (endingCollision.y <= 0) {
		m_position.y = 11.9;
		m_velocity.y = 0;
	}
	else if (endingCollision.y > mapSize.y) {
		// TODO: Kill the player
		m_position = { map.GetMapSize().x / 2,map.GetMapSize().y / 2 };
		m_velocity.y = 0;
		m_velocity.x = 0;

	}
	if (endingCollision.x <= 0) {
		m_position.x = 0 - (endingCollision.x - m_position.x);
		m_velocity.x = 0;
	}
	else if (endingCollision.x + endingCollision.width >= mapSize.x) {
		m_position.x = mapSize.x - endingCollision.width - collisionOffset.x;
		m_velocity.x = 0;
	}

	// This will interpolate the collisions when the player is fast, otherwise he will go through stuff easily
	// WARNING: This solution only works if the player never goes in the minus coordinates, why? because at least for now he can't, if this changes please redo this collision interpolation crap
	std::list<Rectangle> playerCollisions;
	float interpolationAmount = 3;
	for (float i = 3; i > 0; i -= 0.1) {
		Rectangle interpolatedCollision = endingCollision;
		if (m_collisionBox.x < endingCollision.x && endingCollision.x - m_collisionBox.x >= m_collisionBox.width * i) {
			interpolatedCollision.x += m_collisionBox.width * i;
		}
		else if (m_collisionBox.x > endingCollision.x && m_collisionBox.x - endingCollision.x >= m_collisionBox.width * i) {
			interpolatedCollision.x -= m_collisionBox.width * i;
		}

		if (m_collisionBox.y < endingCollision.y && endingCollision.y - m_collisionBox.y >= m_collisionBox.height * i) {
			interpolatedCollision.y += m_collisionBox.height * i;
		}
		else if (m_collisionBox.y > endingCollision.y && m_collisionBox.y - endingCollision.y >= m_collisionBox.height * i) {
			interpolatedCollision.y -= m_collisionBox.height * i;
		}
		playerCollisions.push_front(interpolatedCollision);
	}


	playerCollisions.push_front(endingCollision);

	// Apply map collisions
	for (auto& playerCollision : playerCollisions) {
		for (const auto& collision : map.GetCollisions())
		{
			if (CheckCollisionRecs(playerCollision, collision)) {
				// OMG THIS WORKS :)
				// This means the player is inside the thing 
				auto collisionOverlap = GetCollisionRec(playerCollision, collision);

				if (m_position.y == collision.y - m_texture.height + collisionOffset.y)
					m_isTouchingTheGround = true;
				if (collisionOverlap.height < collisionOverlap.width) {
					if (collisionOverlap.y < collision.y + collision.height / 2) {
						// Feet collision
						m_position.y = collision.y - m_texture.height + collisionOffset.y;
						playerCollision.y = (collision.y - playerCollision.height);
						m_velocity.y = 0;
						m_isTouchingTheGround = true;
						m_collisionBox = playerCollision;
						return;
					}
					else {
						// Head collision
						//m_position.y = collision.y + collision.height + collisionOffset.y;
						//playerCollision.y = collision.y + collision.height;
						m_position.y += collisionOverlap.height;
						playerCollision.y += collisionOverlap.height;
						m_velocity.y = 0.01;
						m_collisionBox = playerCollision;
						return;
					}
				}
				else {
					m_velocity.x = 0;
					if (collisionOverlap.x > collision.x) {
						// Right side of collision block on map
						m_position.x += collisionOverlap.width;
						playerCollision.x += collisionOverlap.width;
						m_collisionBox = playerCollision;
						return;
					}
					else {
						// Left collision
						m_position.x += collisionOverlap.width;
						playerCollision.x += collisionOverlap.width;
						m_collisionBox = playerCollision;
						return;
					}

				}
			}
		}

	}
	m_collisionBox = endingCollision;
}

void Player::ProcessCamera(Map& map)
{
	if (m_hasCamera) {
		Vector2 target = { m_position.x ,m_position.y };

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
	Vector2 position = { m_position.x, m_position.y };
	position.x += m_texture.width / 2;
	position.y += m_texture.height / 2;
	return position;
}

bool Player::IsLookingRight() const
{
	return m_direction == -1;
}

void Player::AddVelocity(Vector2 velocity, float deltaTime)
{
	m_velocity.x += velocity.x * deltaTime;
	m_velocity.y += velocity.y * deltaTime;
}

void Player::ApplyVelocity(float deltaTime)
{
	m_position.x += m_velocity.x * deltaTime;
	m_position.y += m_velocity.y * deltaTime;
}

void Player::Draw()
{
	Rectangle sourceRec = Rectangle{ 0.0f, 0.0f, (float)m_texture.width * m_direction, (float)m_texture.height };

	Rectangle destRec = Rectangle{ 0, 0, (float)m_texture.width, (float)m_texture.height };

	DrawTexturePro(m_texture, sourceRec, destRec, { m_position.x * -1,m_position.y * -1 }, 0, WHITE);


	if (Utils::Debug())
		DrawRectangleRec(m_collisionBox, GREEN); // Debug
}

