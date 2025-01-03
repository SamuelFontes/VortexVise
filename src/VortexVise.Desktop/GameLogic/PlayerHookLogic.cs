using System.Numerics;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.States;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Logic;

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
    public static void SimulateHookState(ICollisionService collisionService,PlayerState currentPlayerState, float gravity, float deltaTime, GameCore gameCore)
    {
        if (currentPlayerState.Input.CancelHook && currentPlayerState.HookState.IsHookAttached)
        {
            currentPlayerState.HookState.IsHookReleased = false;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Velocity = new(0, 0);
            PlayerLogic.MakePlayerDashOrDoubleJump(currentPlayerState, false,gameCore);
        }
        else if (!currentPlayerState.HookState.IsPressingHookKey && currentPlayerState.Input.Hook)
        {
            // start Hook shoot
            currentPlayerState.HookState.IsHookReleased = true;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Position = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
            currentPlayerState.HookState.Position = new(currentPlayerState.HookState.Position.X - 4, currentPlayerState.HookState.Position.Y);

            // Play hook shoot sound
            if (PlayerLogic.IsPlayerLocal(currentPlayerState.Id,gameCore))
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.HookShoot,gameCore);

            // Reset velocity
            currentPlayerState.HookState.Velocity = new(0, 0);

            // Get hook direction
            if (currentPlayerState.Input.Left && currentPlayerState.Input.Down)
            {
                // ↙ 
                currentPlayerState.HookState.Velocity = new(-GameMatch.HookShootForce, GameMatch.HookShootForce);
            }
            else if (currentPlayerState.Input.Right && currentPlayerState.Input.Down)
            {
                // ↘
                currentPlayerState.HookState.Velocity = new(GameMatch.HookShootForce, GameMatch.HookShootForce);
            }
            else if (currentPlayerState.Input.Down)
            {
                // ↓
                currentPlayerState.HookState.Velocity = new(0, GameMatch.HookShootForce);
            }
            else if (currentPlayerState.Input.Left && currentPlayerState.Input.Up)
            {
                // ↖
                currentPlayerState.HookState.Velocity = new(-GameMatch.HookShootForce * 0.5f, -GameMatch.HookShootForce);
            }
            else if (currentPlayerState.Input.Right && currentPlayerState.Input.Up)
            {
                // ↗
                currentPlayerState.HookState.Velocity = new(GameMatch.HookShootForce * 0.5f, -GameMatch.HookShootForce);
            }
            else if (currentPlayerState.Input.Left)
            {
                // ↖
                currentPlayerState.HookState.Velocity = new(-GameMatch.HookShootForce, -GameMatch.HookShootForce);
            }
            else if (currentPlayerState.Input.Right)
            {
                // ↗
                currentPlayerState.HookState.Velocity = new(GameMatch.HookShootForce, -GameMatch.HookShootForce);
            }
            else if (currentPlayerState.Input.Up)
            {
                // ↑
                currentPlayerState.HookState.Velocity = new(0, -GameMatch.HookShootForce * 1.5f);
            }
            else
            {
                // This will use the player direction
                if (currentPlayerState.IsLookingRight())
                {
                    currentPlayerState.HookState.Velocity = new(0 + GameMatch.HookShootForce, -GameMatch.HookShootForce);
                }
                else
                {
                    currentPlayerState.HookState.Velocity = new(0 - GameMatch.HookShootForce, -GameMatch.HookShootForce);

                }
            }
            currentPlayerState.HookState.Velocity = Utils.OnlyAddVelocity(currentPlayerState.HookState.Velocity, currentPlayerState.Velocity, 2);
            //currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + currentPlayerState.Velocity.X, currentPlayerState.HookState.Velocity.Y);
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

        }
        else if (currentPlayerState.HookState.IsHookAttached)
        {
            // Should pull player here
            Vector2 direction = Utils.GetVector2Direction(PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position), currentPlayerState.HookState.Position);

            float distance = RayMath.Vector2Distance(currentPlayerState.HookState.Position, currentPlayerState.Position);

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
                if (collisionService.CheckCollisionRecs(currentPlayerState.HookState.Collision, collision))
                {
                    // Hook colided
                    currentPlayerState.HookState.IsHookAttached = true;
                    if (PlayerLogic.IsPlayerLocal(currentPlayerState.Id,gameCore))
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.HookHit, gameCore, volume: 0.5f);
                }
            }
        }
    }
}
