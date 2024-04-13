using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

/// <summary>
/// Handles player ninja rope/grappling hook logic.
/// </summary>
public static class PlayerHookLogic
{
    /// <summary>
    /// Simulate hook state for current frame.
    /// </summary>
    /// <param name="currentPlayerState">Current player state.</param>
    /// <param name="gravity">Current gravity</param>
    /// <param name="deltaTime">Time since last frame</param>
    public static void SimulateHookState(PlayerState currentPlayerState, float gravity, float deltaTime)
    {
        if (currentPlayerState.Input.CancelHook && currentPlayerState.HookState.IsHookAttached)
        {
            currentPlayerState.HookState.IsHookReleased = false;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Velocity = new(0, 0);
            PlayerLogic.MakePlayerDashOrDoubleJump(currentPlayerState, false);
        }
        else if (!currentPlayerState.HookState.IsPressingHookKey && currentPlayerState.Input.Hook)
        {
            // start Hook shoot
            currentPlayerState.HookState.IsHookReleased = true;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Position = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
            currentPlayerState.HookState.Position = new(currentPlayerState.HookState.Position.X - GameAssets.Gameplay.HookTexture.width * 0.5f, currentPlayerState.HookState.Position.Y);
            currentPlayerState.HookState.Collision = new Rectangle(currentPlayerState.HookState.Position.X, currentPlayerState.HookState.Position.Y, GameMatch.HookSize, GameMatch.HookSize);

            // Play hook shoot sound
            if(PlayerLogic.IsPlayerLocal(currentPlayerState.Id))
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.HookShoot);

            // Reset velocity
            currentPlayerState.HookState.Velocity = new(0, 0);

            // Get hook direction
            if (currentPlayerState.Input.Left && currentPlayerState.Input.Down)
            {
                // ↙ 
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - GameMatch.HookShootForce, currentPlayerState.HookState.Velocity.Y + GameMatch.HookPullForce);
            }
            else if (currentPlayerState.Input.Right && currentPlayerState.Input.Down)
            {
                // ↘
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + GameMatch.HookShootForce, currentPlayerState.HookState.Velocity.Y + GameMatch.HookPullForce);
            }
            else if (currentPlayerState.Input.Down)
            {
                // ↓
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X, currentPlayerState.HookState.Velocity.Y + GameMatch.HookPullForce);
            }
            else if (currentPlayerState.Input.Left && currentPlayerState.Input.Up)
            {
                // ↖
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - GameMatch.HookShootForce * 0.6f, currentPlayerState.HookState.Velocity.Y - GameMatch.HookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Right && currentPlayerState.Input.Up)
            {
                // ↗
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + GameMatch.HookShootForce * 0.6f, currentPlayerState.HookState.Velocity.Y - GameMatch.HookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Left)
            {
                // ↖
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - GameMatch.HookShootForce, currentPlayerState.HookState.Velocity.Y - GameMatch.HookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Right)
            {
                // ↗
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + GameMatch.HookShootForce, currentPlayerState.HookState.Velocity.Y - GameMatch.HookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Up)
            {
                // ↑
                currentPlayerState.HookState.Velocity = new(0, currentPlayerState.HookState.Velocity.Y - GameMatch.HookPullForce * 0.8f);
            }
            else
            {
                // This will use the player direction
                if (currentPlayerState.IsLookingRight())
                {
                    currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + GameMatch.HookShootForce, currentPlayerState.HookState.Velocity.Y - GameMatch.HookPullForce * 0.5f);
                }
                else
                {
                    currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - GameMatch.HookShootForce, currentPlayerState.HookState.Velocity.Y - GameMatch.HookPullForce * 0.5f);

                }
            }
            currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + currentPlayerState.Velocity.X, currentPlayerState.HookState.Velocity.Y);
        }

        else if (currentPlayerState.HookState.IsPressingHookKey && !currentPlayerState.Input.Hook)
        {
            // Allow player to jump again if the hook retract while was attached
            if (currentPlayerState.HookState.IsHookAttached)
                currentPlayerState.CanDash = true;

            // Hook retracted
            currentPlayerState.HookState.IsHookReleased = false;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Velocity = new(0, 0);

        }
        else if (!currentPlayerState.HookState.IsHookAttached && currentPlayerState.HookState.IsHookReleased)
        {
            // Shooting the hook
            currentPlayerState.HookState.Velocity += new Vector2(0, gravity * 0.5f * deltaTime);

            float distance = RayMath.Vector2Distance(currentPlayerState.HookState.Position, currentPlayerState.Position);

            if (distance > GameMatch.HookSizeLimit && (currentPlayerState.HookState.Velocity.X != 0 || currentPlayerState.HookState.Velocity.Y < 0))
            {
                currentPlayerState.HookState.Velocity = new(0, 0);  // Stop hook in all directions
            }

            currentPlayerState.HookState.Position = new(currentPlayerState.HookState.Position.X + currentPlayerState.HookState.Velocity.X * deltaTime * 0.5f, currentPlayerState.HookState.Position.Y + currentPlayerState.HookState.Velocity.Y * deltaTime * 0.5f);
            currentPlayerState.HookState.Collision = new Rectangle(currentPlayerState.HookState.Position.X, currentPlayerState.HookState.Position.Y, currentPlayerState.HookState.Collision.Width, currentPlayerState.HookState.Collision.Height);

        }
        else if (currentPlayerState.HookState.IsHookAttached)
        {
            // Should pull player here
            Vector2 direction = Utils.GetVector2Direction(PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position), currentPlayerState.HookState.Position);

            float distance = RayMath.Vector2Distance(currentPlayerState.HookState.Position, currentPlayerState.Position);

            // TODO: implement this crap here
            //if((GameMatch.HookPullOffset > _originalPullOffset && _offsetChanger < 0) || (GameMatch.HookPullOffset < _originalPullOffset * 6 && _offsetChanger > 0))
            //{
            //GameMatch.HookPullOffset += _offsetChanger * Time.deltaTime * 10;

            //if(_soundTimer == 0)  // This is to not spam the audio 
            //{
            //GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
            //_soundTimer += Time.deltaTime;
            //}


            if (distance > GameMatch.HookPullOffset)
            {
                Vector2 velocity = RayMath.Vector2Scale(direction, GameMatch.HookPullForce);
                currentPlayerState.AddVelocityWithDeltaTime(velocity, deltaTime);
            }
        }
        currentPlayerState.HookState.IsPressingHookKey = currentPlayerState.Input.Hook;

        if (currentPlayerState.HookState.IsHookReleased && !currentPlayerState.HookState.IsHookAttached)
        {
            foreach (var collision in MapLogic.GetCollisions())
            {
                if (Raylib.CheckCollisionRecs(currentPlayerState.HookState.Collision, collision))
                {
                    // Hook colided
                    currentPlayerState.HookState.IsHookAttached = true;
                    if(PlayerLogic.IsPlayerLocal(currentPlayerState.Id))
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.HookHit, volume: 0.5f);
                }
            }
        }
    }

    public static Rectangle GetHookCollision(Vector2 position)
    {
        return new(position.X, position.Y, GameMatch.HookSize, GameMatch.HookSize);
    }
}
