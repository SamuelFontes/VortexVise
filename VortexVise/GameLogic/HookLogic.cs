using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

public static class HookLogic
{
    public static int HookSize = 8;
    public static Texture2D _texture;
    static float _hookPullForce = 3000;
    static float _hookPullOffset = 64;
    static float _hookShootForce = 1000;
    static float _hookSizeLimit = 1000;
    static float _hookTimeout = 0.2f;

    public static HookState SimulateState(float gravity, float deltaTime, PlayerState playerState)
    {
        var state = playerState.HookState;
        if (playerState.Input.CancelHook && state.IsHookAttached)
        {
            GameSounds.PlaySound(GameSounds.Dash, volume: 0.8f);
            state.IsHookReleased = false;
            state.IsHookAttached = false;
            state.Velocity = new(0, 0);
            if (playerState.IsLookingRight())
                playerState.AddVelocity(new(PlayerLogic._jumpForce, -PlayerLogic._jumpForce * 0.2f));
            else
                playerState.AddVelocity(new(-PlayerLogic._jumpForce, -PlayerLogic._jumpForce * 0.2f));

        }
        if (!state.IsPressingHookKey && playerState.Input.Hook)
        {
            // start Hook shoot
            state.IsHookReleased = true;
            state.IsHookAttached = false;
            state.Position = PlayerLogic.GetPlayerCenterPosition(playerState.Position);
            state.Position = new(state.Position.X - _texture.width * 0.5f, playerState.Position.Y);
            state.Collision = new Rectangle(state.Position.X, state.Position.Y, HookSize, HookSize);

            // Play hook shoot sound
            GameSounds.PlaySound(GameSounds.HookShoot);

            // Reset velocity
            state.Velocity = new(0, 0);

            // Get hook direction
            if (playerState.Input.Left && playerState.Input.Down)
            {
                // ↙ 
                state.Velocity = new(state.Velocity.X - _hookShootForce, state.Velocity.Y + _hookPullForce);
            }
            else if (playerState.Input.Right && playerState.Input.Down)
            {
                // ↘
                state.Velocity = new(state.Velocity.X + _hookShootForce, state.Velocity.Y + _hookPullForce);
            }
            else if (playerState.Input.Down)
            {
                // ↓
                state.Velocity = new(state.Velocity.X, state.Velocity.Y + _hookPullForce);
            }
            else if (playerState.Input.Left && playerState.Input.Up)
            {
                // ↖
                state.Velocity = new(state.Velocity.X - _hookShootForce * 0.6f, state.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (playerState.Input.Right && playerState.Input.Up)
            {
                // ↗
                state.Velocity = new(state.Velocity.X + _hookShootForce * 0.6f, state.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (playerState.Input.Left)
            {
                // ↖
                state.Velocity = new(state.Velocity.X - _hookShootForce, state.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (playerState.Input.Right)
            {
                // ↗
                state.Velocity = new(state.Velocity.X + _hookShootForce, state.Velocity.Y - _hookPullForce * 0.5f);
            }
            else if (playerState.Input.Up)
            {
                // ↑
                state.Velocity = new(0, state.Velocity.Y - _hookPullForce * 0.8f);
            }
            else
            {
                // This will use the player direction
                if (playerState.IsLookingRight())
                {
                    state.Velocity = new(state.Velocity.X + _hookShootForce, state.Velocity.Y - _hookPullForce * 0.5f);
                }
                else
                {
                    state.Velocity = new(state.Velocity.X - _hookShootForce, state.Velocity.Y - _hookPullForce * 0.5f);

                }
            }
            state.Velocity = new(state.Velocity.X + playerState.Velocity.X, state.Velocity.Y);
        }

        else if (state.IsPressingHookKey && !playerState.Input.Hook)
        {
            // Hook retracted
            state.IsHookReleased = false;
            state.IsHookAttached = false;
            state.Velocity = new(0, 0);
        }
        else if (!state.IsHookAttached && state.IsHookReleased)
        {
            // Shooting the hook
            state.Velocity += new Vector2(0, gravity * 0.5f * deltaTime);

            Vector2 direction = Utils.GetVector2Direction(PlayerLogic.GetPlayerCenterPosition(playerState.Position), state.Position);
            float distance = RayMath.Vector2Distance(state.Position, playerState.Position);

            if (distance > _hookSizeLimit && (state.Velocity.X != 0 || state.Velocity.Y < 0))
            {
                state.Velocity = new(0, 0);  // Stop hook in all directions
            }

            state.Position = new(state.Position.X + state.Velocity.X * deltaTime * 0.5f, state.Position.Y + state.Velocity.Y * deltaTime * 0.5f);
            state.Collision = new Rectangle(state.Position.X, state.Position.Y, state.Collision.Width, state.Collision.Height);

        }
        else if (state.IsHookAttached)
        {
            // Should pull player here
            Vector2 direction = Utils.GetVector2Direction(PlayerLogic.GetPlayerCenterPosition(playerState.Position), state.Position);

            float distance = RayMath.Vector2Distance(state.Position, playerState.Position);

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
                playerState.AddVelocityWithDeltaTime(velocity, deltaTime);
            }
        }
        state.IsPressingHookKey = playerState.Input.Hook;

        if (state.IsHookReleased && !state.IsHookAttached)
        {
            foreach (var collision in MapLogic.GetCollisions())
            {
                if (Raylib.CheckCollisionRecs(state.Collision, collision))
                {
                    // Hook colided
                    state.IsHookAttached = true;
                    GameSounds.PlaySound(GameSounds.HookHit, volume: 0.5f);
                }
            }
        }
        return state;
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
