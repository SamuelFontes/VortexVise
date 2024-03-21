using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

public static class PlayerHookLogic
{
    public static int HookSize = 8;
    public static Texture _texture;
    static float _hookPullForce = 3000;
    static float _hookPullOffset = 64;
    static float _hookShootForce = 1000;
    static float _hookSizeLimit = 1000;
    static float _hookTimeout = 0.2f;

    public static void SimulateHookState(PlayerState currentPlayerState, float gravity, float deltaTime)
    {
        if (currentPlayerState.Input.CancelHook && currentPlayerState.HookState.IsHookAttached)
        {
            currentPlayerState.HookState.IsHookReleased = false;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Velocity = new(0, 0);
            PlayerLogic.MakePlayerDashOrDoubleJump(currentPlayerState, false);
        }
        if (!currentPlayerState.HookState.IsPressingHookKey && currentPlayerState.Input.Hook)
        {
            // start Hook shoot
            currentPlayerState.HookState.IsHookReleased = true;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Position = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
            currentPlayerState.HookState.Position = new(currentPlayerState.HookState.Position.X - _texture.width * 0.5f, currentPlayerState.HookState.Position.Y);
            currentPlayerState.HookState.Collision = new Rectangle(currentPlayerState.HookState.Position.X, currentPlayerState.HookState.Position.Y, HookSize, HookSize);

            // Play hook shoot sound
            GameSounds.PlaySound(GameSounds.HookShoot);

            // Reset velocity
            currentPlayerState.HookState.Velocity = new(0, 0);

            // Get hook direction
            if (currentPlayerState.Input.Left && currentPlayerState.Input.Down)
            {
                // ↙ 
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - _hookShootForce, currentPlayerState.HookState.Velocity.Y + _hookPullForce);
            }
            else if (currentPlayerState.Input.Right && currentPlayerState.Input.Down)
            {
                // ↘
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + _hookShootForce, currentPlayerState.HookState.Velocity.Y + _hookPullForce);
            }
            else if (currentPlayerState.Input.Down)
            {
                // ↓
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X, currentPlayerState.HookState.Velocity.Y + _hookPullForce);
            }
            else if (currentPlayerState.Input.Left && currentPlayerState.Input.Up)
            {
                // ↖
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - _hookShootForce * 0.6f, currentPlayerState.HookState.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Right && currentPlayerState.Input.Up)
            {
                // ↗
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + _hookShootForce * 0.6f, currentPlayerState.HookState.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Left)
            {
                // ↖
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - _hookShootForce, currentPlayerState.HookState.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Right)
            {
                // ↗
                currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + _hookShootForce, currentPlayerState.HookState.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (currentPlayerState.Input.Up)
            {
                // ↑
                currentPlayerState.HookState.Velocity = new(0, currentPlayerState.HookState.Velocity.Y - _hookPullForce * 0.8f);
            }
            else
            {
                // This will use the player direction
                if (currentPlayerState.IsLookingRight())
                {
                    currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + _hookShootForce, currentPlayerState.HookState.Velocity.Y - _hookPullForce * 0.5f);
                }
                else
                {
                    currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X - _hookShootForce, currentPlayerState.HookState.Velocity.Y - _hookPullForce * 0.5f);

                }
            }
            currentPlayerState.HookState.Velocity = new(currentPlayerState.HookState.Velocity.X + currentPlayerState.Velocity.X, currentPlayerState.HookState.Velocity.Y);
        }

        else if (currentPlayerState.HookState.IsPressingHookKey && !currentPlayerState.Input.Hook)
        {
            // Hook retracted
            currentPlayerState.HookState.IsHookReleased = false;
            currentPlayerState.HookState.IsHookAttached = false;
            currentPlayerState.HookState.Velocity = new(0, 0);
        }
        else if (!currentPlayerState.HookState.IsHookAttached && currentPlayerState.HookState.IsHookReleased)
        {
            // Shooting the hook
            currentPlayerState.HookState.Velocity += new Vector2(0, gravity * 0.5f * deltaTime);

            Vector2 direction = Utils.GetVector2Direction(PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position), currentPlayerState.HookState.Position);
            float distance = RayMath.Vector2Distance(currentPlayerState.HookState.Position, currentPlayerState.Position);

            if (distance > _hookSizeLimit && (currentPlayerState.HookState.Velocity.X != 0 || currentPlayerState.HookState.Velocity.Y < 0))
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
            //if((_hookPullOffset > _originalPullOffset && _offsetChanger < 0) || (_hookPullOffset < _originalPullOffset * 6 && _offsetChanger > 0))
            //{
            //_hookPullOffset += _offsetChanger * Time.deltaTime * 10;

            //if(_soundTimer == 0)  // This is to not spam the audio 
            //{
            //GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
            //_soundTimer += Time.deltaTime;
            //}


            if (distance > _hookPullOffset)
            {
                Vector2 velocity = RayMath.Vector2Scale(direction, _hookPullForce);
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
                    GameSounds.PlaySound(GameSounds.HookHit, volume: 0.5f);
                }
            }
        }
    }

    public static void DrawState(PlayerState playerState)
    {
        if (playerState.HookState.IsHookReleased)
        {
            Raylib.DrawLineEx(PlayerLogic.GetPlayerCenterPosition(playerState.Position), new Vector2(playerState.HookState.Position.X + 3, playerState.HookState.Position.Y + 3), 1, new Color(159, 79, 0, 255));
            Raylib.DrawTexture(_texture, (int)playerState.HookState.Position.X, (int)playerState.HookState.Position.Y, Raylib.WHITE);

            if (Utils.Debug())
                Raylib.DrawRectangleRec(playerState.HookState.Collision, Raylib.GREEN); // Debug
        }



    }
    public static Rectangle GetHookCollision(Vector2 position)
    {
        return new(position.X, position.Y, HookSize, HookSize);
    }
}
