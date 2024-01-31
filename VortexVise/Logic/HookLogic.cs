﻿using Raylib_cs;
using System.Numerics;
using VortexVise.States;
using VortexVise.Utilities;

namespace VortexVise.GameObjects;

public static class HookLogic
{
    public static int HookSize = 16;
    static Texture2D _texture = Raylib.LoadTexture("Resources/Sprites/GFX/hook_head.png");
    static float _hookPullForce = 5000;
    static float _hookPullOffset = 50;
    static float _hookShootForce = 2000;
    static float _hookSizeLimit = 200;
    static float _hookTimeout = 0.2f;

    public static HookState SimulateState(float gravity, float deltaTime, PlayerState playerState)
    {
        var state = playerState.HookState;
        if (playerState.Input.CancelHook && state.IsHookAttached)
        {
            state.IsHookReleased = false;
            state.IsHookAttached = false;
            state.Velocity = new(0, 0);

        }
        if (!state.IsPressingHookKey && playerState.Input.Hook)
        {
            // start Hook shoot
            state.IsHookReleased = true;
            state.IsHookAttached = false;
            state.Position = PlayerLogic.GetPlayerCenterPosition(playerState.Position);
            state.Collision = new Rectangle(state.Position, new(HookSize, HookSize));

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
                state.Velocity = new(state.Velocity.X - _hookShootForce * 0.6f, state.Velocity.Y - _hookPullForce);
            }
            else if (playerState.Input.Right && playerState.Input.Up)
            {
                // ↗
                state.Velocity = new(state.Velocity.X + _hookShootForce * 0.6f, state.Velocity.Y - _hookPullForce);
            }
            else if (playerState.Input.Left)
            {
                // ↖
                state.Velocity = new(state.Velocity.X - _hookShootForce, state.Velocity.Y - _hookPullForce);
            }
            else if (playerState.Input.Right)
            {
                // ↗
                state.Velocity = new(state.Velocity.X + _hookShootForce, state.Velocity.Y - _hookPullForce);
            }
            else if (playerState.Input.Up)
            {
                // ↑
                state.Velocity = new(state.Velocity.X, state.Velocity.Y - _hookPullForce);
            }
            else
            {
                // This will use the player direction
                if (playerState.IsLookingRight())
                {
                    state.Velocity = new(state.Velocity.X + _hookShootForce, state.Velocity.Y - _hookPullForce);
                }
                else
                {
                    state.Velocity = new(state.Velocity.X - _hookShootForce, state.Velocity.Y - _hookPullForce);

                }
            }
        }

        else if ((state.IsPressingHookKey && !playerState.Input.Hook))
        {
            // Hook retracted
            state.IsHookReleased = false;
            state.IsHookAttached = false;
            state.Velocity = new(0, 0);
        }
        else if (!state.IsHookAttached && state.IsHookReleased)
        {
            // Shooting the hook
            state.Velocity += new Vector2(0,gravity * 0.5f * deltaTime);
            state.Position = new(state.Position.X + state.Velocity.X * deltaTime * 0.5f, state.Position.Y + state.Velocity.Y * deltaTime * 0.5f);
            state.Collision = new Rectangle(state.Position,state.Collision.Width,state.Collision.Height);

        }
        else if (state.IsHookAttached)
        {
            // Should pull player here
            Vector2 direction = Utils.GetVector2Direction(PlayerLogic.GetPlayerCenterPosition(playerState.Position), state.Position);

            float distance = Raymath.Vector2Distance(state.Position, playerState.Position);

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
                playerState.AddVelocity(velocity, deltaTime);
            }
        }
        state.IsPressingHookKey = playerState.Input.Hook;

        if (state.IsHookReleased)
        {
            foreach (var collision in MapLogic.GetCollisions())
            {
                if (Raylib.CheckCollisionRecs(state.Collision, collision))
                {
                    state.IsHookAttached = true;
                }
            }
        }
        return state;
    }

    public static void DrawState(PlayerState playerState)
    {
        if (playerState.HookState.IsHookReleased)
        {
            Raylib.DrawLineEx(PlayerLogic.GetPlayerCenterPosition(playerState.Position), new Vector2(playerState.HookState.Position.X + 8, playerState.HookState.Position.Y + 8), 2, new Color(159, 79, 0, 255));
            Raylib.DrawTexture(_texture, (int)playerState.HookState.Position.X - 8, (int)playerState.HookState.Position.Y - 10, Color.White);

            if (Utils.Debug())
                Raylib.DrawRectangleRec(playerState.HookState.Collision, Color.Green); // Debug
        }



    }
}
