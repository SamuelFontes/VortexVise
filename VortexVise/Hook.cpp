#include "Hook.h"
#include <raymath.h>
#include "Utils.h"

void Hook::Simulate(Player& player, Map& map, float gravity, float deltaTime)
{
	if (!m_pressingHookKey && IsMouseButtonDown(1)) {
		// start Hook shoot
		m_isHookReleased = true;
		m_isHookAttached = false;
		m_position = player.GetPlayerCenterPosition();
		m_collision.x = m_position.x;
		m_collision.y = m_position.y;

		// Reset velocity
		m_velocity = { 0,0 };

		// Get hook direction
		if (IsKeyDown(KEY_A) && IsKeyDown(KEY_S)) {
			// ↙ 
			m_velocity.x -= m_hookShootForce;
			m_velocity.y += m_hookShootForce;
		}
		else if (IsKeyDown(KEY_D) && IsKeyDown(KEY_S)) {
			// ↘
			m_velocity.x += m_hookShootForce;
			m_velocity.y += m_hookShootForce;
		}
		else if (IsKeyDown(KEY_S)) {
			// ↓
			m_velocity.y += m_hookShootForce;
		}
		else if (IsKeyDown(KEY_A) && IsKeyDown(KEY_W)) {
			// ↖
			m_velocity.x -= m_hookShootForce * 0.6;
			m_velocity.y -= m_hookShootForce;
		}
		else if (IsKeyDown(KEY_D) && IsKeyDown(KEY_W)) {
			// ↗
			m_velocity.x += m_hookShootForce * 0.6;
			m_velocity.y -= m_hookShootForce;
		}
		else if (IsKeyDown(KEY_A)) {
			// ↖
			m_velocity.x -= m_hookShootForce;
			m_velocity.y -= m_hookShootForce;
		}
		else if (IsKeyDown(KEY_D)) {
			// ↗
			m_velocity.x += m_hookShootForce;
			m_velocity.y -= m_hookShootForce;
		}
		else if (IsKeyDown(KEY_W)) {
			// ↑
			m_velocity.y -= m_hookShootForce;
		}
		else {
			// This will use the player direction
			if (player.IsLookingRight()) {
				m_velocity.x += m_hookShootForce;
				m_velocity.y -= m_hookShootForce;
			}
			else {
				m_velocity.x -= m_hookShootForce;
				m_velocity.y -= m_hookShootForce;

			}
		}
	}
	else if ((m_pressingHookKey && !IsMouseButtonDown(1))) {
		// Hook retracted
		m_isHookReleased = false;
		m_isHookAttached = false;
		m_velocity = { 0,0 };
	}
	else if (!m_isHookAttached) {
		// Shooting the hook
		m_position = { m_position.x + m_velocity.x * deltaTime,m_position.y + m_velocity.y * deltaTime };
		m_position.y += gravity * 0.5 * deltaTime;
		m_collision.x = m_position.x;
		m_collision.y = m_position.y;

	}
	else if (m_isHookAttached) {
		// Should pull player here
		Vector2 direction = Utils::GetVector2Direction(player.GetPlayerCenterPosition(),m_position);

		float distance = Vector2Distance(m_position, player.GetPosition());

		// TODO: implement this crap here
		//if((m_hookPullOffset > m_originalPullOffset && _offsetChanger < 0) || (_hookPullOffset < _originalPullOffset * 6 && _offsetChanger > 0))
		//{
		//_hookPullOffset += _offsetChanger * Time.deltaTime * 10;

		//if(_soundTimer == 0)  // This is to not spam the audio 
		//{
		//GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
		//_soundTimer += Time.deltaTime;
		//}
		

		if (distance > m_hookPullOffset)
		{
			Vector2 velocity = Vector2Scale(direction, m_hookPullForce);
			player.AddVelocity(velocity,deltaTime);
		}
	}
	m_pressingHookKey = IsMouseButtonDown(1);

	if (m_isHookReleased) {
		for (const auto& collision : map.GetCollisions()) {
			if (CheckCollisionRecs(m_collision, collision)) {
				m_isHookAttached = true;
			}
		}
	}
}

void Hook::Draw(Player& const player)
{
	if (m_isHookReleased) {
		DrawTexture(m_texture, m_position.x, m_position.y, WHITE);

		if(Utils::Debug)
			DrawRectangleRec(m_collision, GREEN); // Debug
	}

}
