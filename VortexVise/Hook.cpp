﻿#include "Hook.h"

void Hook::Simulate(Player& player, Map& map, float gravity)
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
		else if (IsKeyDown(KEY_A) && IsKeyDown(KEY_W)){
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
	else if (m_pressingHookKey && !IsMouseButtonDown(1)) {
		// Hook retracted
		m_isHookReleased = false;
		m_isHookAttached = false;
		m_velocity = { 0,0 };
	}
	else if (!m_isHookAttached) {
		// Shooting the hook
		m_position = { m_position.x + m_velocity.x * GetFrameTime(),m_position.y + m_velocity.y * GetFrameTime() };
		m_position.y += gravity * 0.5  * GetFrameTime();
		m_collision.x = m_position.x;
		m_collision.y = m_position.y;

	}
	else {
		// Should pull player here
	}
	m_pressingHookKey = IsMouseButtonDown(1);

	for (const auto& collision : map.GetCollisions()) {
		if (CheckCollisionRecs(m_collision, collision)) {
			m_isHookAttached = true;
		}
	}
}

void Hook::Draw(Player& const player)
{
	if (m_isHookReleased) {
		DrawRectangleRec(m_collision, GREEN); // Debug
	}

}
