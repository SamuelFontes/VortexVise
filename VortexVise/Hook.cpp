#include "Hook.h"

void Hook::Simulate(Player& player, Map& map, const float& gravity, const float& deltaTime, Input& input)
{
	if (input.CancelHook && m_isHookAttached) {
		m_isHookReleased = false;
		m_isHookAttached = false;
		m_velocity = { 0,0 };

	}
	if (!m_pressingHookKey && input.Hook) {
		// start Hook shoot
		m_isHookReleased = true;
		m_isHookAttached = false;
		m_position = player.GetPlayerCenterPosition();
		m_collision.x = m_position.x;
		m_collision.y = m_position.y;

		// Reset velocity
		m_velocity = { 0,0 };

		// Get hook direction
		if (input.Left && input.Down){
			// ↙ 
			m_velocity.x -= m_hookShootForce;
			m_velocity.y += m_hookShootForce;
		}
		else if (input.Right && input.Down){
			// ↘
			m_velocity.x += m_hookShootForce;
			m_velocity.y += m_hookShootForce;
		}
		else if (input.Down){
			// ↓
			m_velocity.y += m_hookShootForce;
		}
		else if (input.Left && input.Up) {
			// ↖
			m_velocity.x -= m_hookShootForce * 0.6;
			m_velocity.y -= m_hookShootForce;
		}
		else if (input.Right && input.Up) {
			// ↗
			m_velocity.x += m_hookShootForce * 0.6;
			m_velocity.y -= m_hookShootForce;
		}
		else if (input.Left) {
			// ↖
			m_velocity.x -= m_hookShootForce;
			m_velocity.y -= m_hookShootForce;
		}
		else if (input.Right) {
			// ↗
			m_velocity.x += m_hookShootForce;
			m_velocity.y -= m_hookShootForce;
		}
		else if (input.Up) {
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
	else if ((m_pressingHookKey && !input.Hook)) {
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
		Vector2 direction = Utils::GetVector2Direction(player.GetPlayerCenterPosition(), m_position);

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
			player.AddVelocity(velocity, deltaTime);
		}
	}
	m_pressingHookKey = input.Hook;

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
		DrawLineEx(player.GetPlayerCenterPosition(), { m_position.x + 8, m_position.y + 8 }, 2, { 159,79,0,255 });
		DrawTexture(m_texture, m_position.x - 8, m_position.y - 10, WHITE);

		if (Utils::Debug())
			DrawRectangleRec(m_collision, GREEN); // Debug
	}

}
