using Raylib_cs;
using System.Numerics;
using VortexVise.States;
using VortexVise.Utilities;

namespace VortexVise.GameObjects;

public class HookLogic
{
    Texture2D _texture = Raylib.LoadTexture("Resources/Sprites/GFX/hook_head.png");
    float _hookPullForce = 2500;
    float _hookPullOffset = 50;
    float _hookShootForce = 2000;
    float _hookSizeLimit = 200;
    float _hookTimeout = 0.2f;
    public HookState SimulateState(PlayerLogic player, MapLogic map, float gravity, float deltaTime, InputState input)
    {
        var state = GetState();
        if (input.CancelHook && state.IsHookAttached)
        {
            state.IsHookReleased = false;
            state.IsHookAttached = false;
            state.Velocity = new(0, 0);

        }
        if (!state.IsPressingHookKey && input.Hook)
        {
            // start Hook shoot
            state.IsHookReleased = true;
            state.IsHookAttached = false;
            state.Position = player.GetPlayerCenterPosition();
            state.Collision.X = state.Position.X;
            state.Collision.Y = state.Position.Y;

            // Reset velocity
            state.Velocity = new(0, 0);

            // Get hook direction
            if (input.Left && input.Down)
            {
                // ↙ 
                state.Velocity.X -= _hookShootForce;
                state.Velocity.Y += _hookShootForce;
            }
            else if (input.Right && input.Down)
            {
                // ↘
                state.Velocity.X += _hookShootForce;
                state.Velocity.Y += _hookShootForce;
            }
            else if (input.Down)
            {
                // ↓
                state.Velocity.Y += _hookShootForce;
            }
            else if (input.Left && input.Up)
            {
                // ↖
                state.Velocity.X -= _hookShootForce * 0.6f;
                state.Velocity.Y -= _hookShootForce;
            }
            else if (input.Right && input.Up)
            {
                // ↗
                state.Velocity.X += _hookShootForce * 0.6f;
                state.Velocity.Y -= _hookShootForce;
            }
            else if (input.Left)
            {
                // ↖
                state.Velocity.X -= _hookShootForce;
                state.Velocity.Y -= _hookShootForce;
            }
            else if (input.Right)
            {
                // ↗
                state.Velocity.X += _hookShootForce;
                state.Velocity.Y -= _hookShootForce;
            }
            else if (input.Up)
            {
                // ↑
                state.Velocity.Y -= _hookShootForce;
            }
            else
            {
                // This will use the player direction
                if (player.IsLookingRight())
                {
                    state.Velocity.X += _hookShootForce;
                    state.Velocity.Y -= _hookShootForce;
                }
                else
                {
                    state.Velocity.X -= _hookShootForce;
                    state.Velocity.Y -= _hookShootForce;

                }
            }
        }

        else if ((state.IsPressingHookKey && !input.Hook))
        {
            // Hook retracted
            state.IsHookReleased = false;
            state.IsHookAttached = false;
            state.Velocity = new(0, 0);
        }
        else if (!state.IsHookAttached)
        {
            // Shooting the hook
            state.Position = new(state.Position.X + state.Velocity.X * deltaTime, state.Position.Y + state.Velocity.Y * deltaTime);
            state.Position.Y += gravity * 0.5f * deltaTime;
            state.Collision.X = state.Position.X;
            state.Collision.Y = state.Position.Y;

        }
        else if (state.IsHookAttached)
        {
            // Should pull player here
            Vector2 direction = Utils.GetVector2Direction(player.GetPlayerCenterPosition(), state.Position);

            float distance = Raymath.Vector2Distance(state.Position, player.GetPosition());

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
                Vector2 velocity = Raymath.Vector2Scale(direction, _hookPullForce);
                player.AddVelocity(velocity, deltaTime);
            }
        }
        state.IsPressingHookKey = input.Hook;

        if (state.IsHookReleased)
        {
            foreach (var collision in map.GetCollisions())
            {
                if (Raylib.CheckCollisionRecs(state.Collision, collision))
                {
                    state.IsHookAttached = true;
                }
            }
        }
        return state;
    }

    public void DrawState(PlayerLogic player)
    {
        if (_isHookReleased)
        {
            Raylib.DrawLineEx(player.GetPlayerCenterPosition(), new Vector2( _position.X + 8, _position.Y + 8 ), 2, new Color( 159,79,0,255 ));
            Raylib.DrawTexture(_texture, (int)_position.X - 8, (int)_position.Y - 10, Color.White);

            if (Utils.Debug())
                Raylib.DrawRectangleRec(_collision, Color.Green); // Debug
        }



    }
}
