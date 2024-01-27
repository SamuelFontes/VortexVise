using Raylib_cs;
using System.Numerics;
using VortexVise.Models;
using VortexVise.Utilities;

namespace VortexVise.GameObjects
{
    internal class Hook
    {
        Vector2 _position = new(0, 0);
        Vector2 _velocity = new(0, 0);
        Rectangle _collision = new(0, 0, 16, 16);
        Texture2D _texture = Raylib.LoadTexture("Resources/Sprites/GFX/hook_head.png");
        float _hookPullForce = 2500;
        float _hookPullOffset = 50;
        float _hookShootForce = 2000;
        float _hookSizeLimit = 200;
        bool _isHookAttached = false;
        bool _isHookReleased = false;
        float _hookTimeout = 0.2f;
        bool _pressingHookKey = false;
        public void Simulate(Player player, Map map, float gravity, float deltaTime, Input input)
        {
            if (input.CancelHook && _isHookAttached)
            {
                _isHookReleased = false;
                _isHookAttached = false;
                _velocity = new(0, 0);

            }
            if (!_pressingHookKey && input.Hook)
            {
                // start Hook shoot
                _isHookReleased = true;
                _isHookAttached = false;
                _position = player.GetPlayerCenterPosition();
                _collision.X = _position.X;
                _collision.Y = _position.Y;

                // Reset velocity
                _velocity = new(0, 0);

                // Get hook direction
                if (input.Left && input.Down)
                {
                    // ↙ 
                    _velocity.X -= _hookShootForce;
                    _velocity.Y += _hookShootForce;
                }
                else if (input.Right && input.Down)
                {
                    // ↘
                    _velocity.X += _hookShootForce;
                    _velocity.Y += _hookShootForce;
                }
                else if (input.Down)
                {
                    // ↓
                    _velocity.Y += _hookShootForce;
                }
                else if (input.Left && input.Up)
                {
                    // ↖
                    _velocity.X -= _hookShootForce * 0.6f;
                    _velocity.Y -= _hookShootForce;
                }
                else if (input.Right && input.Up)
                {
                    // ↗
                    _velocity.X += _hookShootForce * 0.6f;
                    _velocity.Y -= _hookShootForce;
                }
                else if (input.Left)
                {
                    // ↖
                    _velocity.X -= _hookShootForce;
                    _velocity.Y -= _hookShootForce;
                }
                else if (input.Right)
                {
                    // ↗
                    _velocity.X += _hookShootForce;
                    _velocity.Y -= _hookShootForce;
                }
                else if (input.Up)
                {
                    // ↑
                    _velocity.Y -= _hookShootForce;
                }
                else
                {
                    // This will use the player direction
                    if (player.IsLookingRight())
                    {
                        _velocity.X += _hookShootForce;
                        _velocity.Y -= _hookShootForce;
                    }
                    else
                    {
                        _velocity.X -= _hookShootForce;
                        _velocity.Y -= _hookShootForce;

                    }
                }
            }

            else if ((_pressingHookKey && !input.Hook))
            {
                // Hook retracted
                _isHookReleased = false;
                _isHookAttached = false;
                _velocity = new(0, 0);
            }
            else if (!_isHookAttached)
            {
                // Shooting the hook
                _position = new(_position.X + _velocity.X * deltaTime, _position.Y + _velocity.Y * deltaTime);
                _position.Y += gravity * 0.5f * deltaTime;
                _collision.X = _position.X;
                _collision.Y = _position.Y;

            }
            else if (_isHookAttached)
            {
                // Should pull player here
                Vector2 direction = Utils.GetVector2Direction(player.GetPlayerCenterPosition(), _position);

                float distance = Raymath.Vector2Distance(_position, player.GetPosition());

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
            _pressingHookKey = input.Hook;

            if (_isHookReleased)
            {
                foreach (var collision in map.GetCollisions())
                {
                    if (Raylib.CheckCollisionRecs(_collision, collision))
                    {
                        _isHookAttached = true;
                    }
                }
            }
        }

        public void Draw(Player player)
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
}
