using Raylib_cs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Models;
using VortexVise.Networking;
using VortexVise.Utilities;

namespace VortexVise.GameObjects;

public class Player
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Hook Hook { get; private set; } = new Hook();

    private Vector2 _position = new Vector2() { X = 0, Y = 0 };
    private Vector2 _velocity = new Vector2() { X = 0, Y = 0 };
    private int _direction = 1;
    private Texture2D _texture;
    private float _maxMoveSpeed = 350;
    private float _acceleration = 750;
    private Camera2D _camera;
    private bool _hasCamera = false;
    private bool _isTouchingTheGround = false;
    private Rectangle _collisionBox;
    private List<Rectangle> _playerCollisions = new List<Rectangle>();

    public Player(bool hasCamera, Map map)
    {
        _texture = Raylib.LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
        var spawnPoint = new Vector2(Raylib.GetScreenWidth() / 2.0f, Raylib.GetScreenHeight() / 2.0f);
        _position = spawnPoint;

        if (hasCamera)
        {
            _hasCamera = hasCamera;
            _camera = new Camera2D(spawnPoint, spawnPoint, 0, 1);
        }
    }

    public Vector2 ProcessVelocity(float deltaTime, Input input)
    {
        var velocity = _velocity;
        if (input.Right)
        {
            velocity.X += _acceleration * deltaTime;
            if (velocity.X > _maxMoveSpeed)// && gravitationalForce == 0)
                velocity.X = _maxMoveSpeed;
            _direction = -1;
        }
        else if (input.Left)
        {
            velocity.X -= _acceleration * deltaTime;
            if (velocity.X < _maxMoveSpeed * -1)// && gravitationalForce == 0)
                velocity.X = _maxMoveSpeed * -1;
            _direction = 1;
        }
        else
        {
            float desaceleration = _isTouchingTheGround || velocity.Y == 0f ? 10f : 0.5f;
            velocity.X = Raymath.Lerp(velocity.X, 0, 1f - (float)Math.Exp(-desaceleration * deltaTime));
        }


        if (input.Jump && _isTouchingTheGround)
        {
            _isTouchingTheGround = false;
            velocity.Y = -400;
        }

        return velocity;
    }

    public Input GetInput()
    {
        // TODO: Implement gamepad and stuff
        Input input = new();
        if (Raylib.IsKeyDown(KeyboardKey.A))
            input.Left = true;
        if (Raylib.IsKeyDown(KeyboardKey.D))
            input.Right = true;
        if (Raylib.IsKeyDown(KeyboardKey.Space) || Raylib.IsKeyDown(KeyboardKey.K))
            input.Jump = true;
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
            input.CancelHook = true;
        if (Raylib.IsMouseButtonDown(MouseButton.Right) || Raylib.IsKeyDown(KeyboardKey.J))
            input.Hook = true;
        if (Raylib.IsKeyDown(KeyboardKey.W))
            input.Up = true;
        if (Raylib.IsKeyDown(KeyboardKey.S))
            input.Down = true;
        return input;
    }

    public Vector2 ProcessPosition(float gravity, float deltaTime, Vector2 velocity)
    {
        var position = _position;
        if (velocity.X != 0)
            position.X += velocity.X * deltaTime;

        float maxGravity = 500;
        if (!_isTouchingTheGround)
        {
            velocity.Y += gravity * deltaTime;
            if (velocity.Y >= maxGravity)
                velocity.Y = maxGravity;
        }
        position.Y += velocity.Y * deltaTime;
        return position;
    }

    public Vector2 GetPosition()
    {
        return _position;
    }

    public Vector2 GetVelocity()
    {
        return _velocity;
    }

    public float GetX()
    {
        return _position.X;
    }

    public float GetY()
    {
        return _position.Y;
    }

    public float GetGravitationalForce()
    {
        return _velocity.Y;
    }

    public float GetMoveSpeed()
    {
        return _velocity.X;
    }

    public void ApplyCollisions(Map map)
    {
        Vector2 collisionOffset = new(20, 12);
        Rectangle endingCollision = new(_position.X + collisionOffset.X, _position.Y + collisionOffset.Y, 25, 40);
        _isTouchingTheGround = false;

        Vector2 mapSize = map.GetMapSize();
        // Apply ouside map collisions
        if (endingCollision.Y <= 0)
        {
            _position.Y = 11.9f;
            _velocity.Y = 0;
        }
        else if (endingCollision.Y > mapSize.Y)
        {
            // TODO: Kill the player
            _position = new(map.GetMapSize().X / 2, map.GetMapSize().Y / 2);
            _velocity.Y = 0;
            _velocity.X = 0;

        }
        if (endingCollision.X <= 0)
        {
            _position.X = 0 - (endingCollision.X - _position.X);
            _velocity.X = 0;
        }
        else if (endingCollision.X + endingCollision.Width >= mapSize.X)
        {
            _position.X = mapSize.X - endingCollision.Width - collisionOffset.X;
            _velocity.X = 0;
        }

        // This will interpolate the collisions when the player is fast, otherwise he will go through stuff easily
        // WARNING: This solution only works if the player never goes in the minus coordinates, why? because at least for now he can't, if this changes please redo this collision interpolation crap
        _playerCollisions.Clear();
        float interpolationAmount = 2;
        for (float i = interpolationAmount; i > 0; i -= 0.2f)
        {
            Rectangle interpolatedCollision = endingCollision;
            if (_collisionBox.X < endingCollision.X && endingCollision.X - _collisionBox.X >= _collisionBox.Width * i)
            {
                interpolatedCollision.X += _collisionBox.Width * i;
            }
            else if (_collisionBox.X > endingCollision.X && _collisionBox.X - endingCollision.X >= _collisionBox.Width * i)
            {
                interpolatedCollision.X -= _collisionBox.Width * i;
            }

            if (_collisionBox.Y < endingCollision.Y && endingCollision.Y - _collisionBox.Y >= _collisionBox.Height * i)
            {
                interpolatedCollision.Y += _collisionBox.Height * i;
            }
            else if (_collisionBox.Y > endingCollision.Y && _collisionBox.Y - endingCollision.Y >= _collisionBox.Height * i)
            {
                interpolatedCollision.Y -= _collisionBox.Height * i;
            }
            if (interpolatedCollision.X != endingCollision.X || interpolatedCollision.Y != endingCollision.Y)
                _playerCollisions.Add(interpolatedCollision);
        }


        _playerCollisions.Add(endingCollision);

        // Apply map collisions
        foreach (var playerCollision in _playerCollisions)
        {
            bool colided = false;
            foreach (var collision in map.GetCollisions())
            {
                if (Raylib.CheckCollisionRecs(playerCollision, collision))
                {

                    // This means the player is inside the thing 
                    var collisionOverlap = Raylib.GetCollisionRec(playerCollision, collision);

                    if (_position.Y == collision.Y - _texture.Height + collisionOffset.Y)
                        _isTouchingTheGround = true;

                    Vector2 colliderCenter = new(collision.X + collision.Width * 0.5f, collision.Y + collision.Height * 0.5f);

                    if (collisionOverlap.Height < collisionOverlap.Width)
                    {
                        if (collisionOverlap.Y < colliderCenter.Y)
                        {
                            // Feet collision
                            _position.Y = collision.Y - _texture.Height + collisionOffset.Y;
                            _collisionBox = playerCollision;
                            _collisionBox.Y = (collision.Y - playerCollision.Height);
                            _velocity.Y = 0;
                            _isTouchingTheGround = true;
                            colided = true;
                            continue;
                        }
                        else
                        {
                            // Head collision
                            //_position.Y = collision.Y + collision.height + collisionOffset.Y;
                            //playerCollision.Y = collision.Y + collision.height;
                            _position.Y += collisionOverlap.Height;
                            _collisionBox = playerCollision;
                            _collisionBox.Y += collisionOverlap.Height;
                            _velocity.Y = 0.01f;
                            colided = true;
                            continue;
                        }
                    }
                    else
                    {

                        if (collisionOverlap.X > colliderCenter.X)
                        {
                            _velocity.X = 0;
                            // Right side of collision block on map
                            _position.X += collisionOverlap.Width;
                            _collisionBox = playerCollision;
                            _collisionBox.X += collisionOverlap.Width;
                            colided = true;
                            continue;
                        }
                        else
                        {
                            _velocity.X = 0;
                            // Left collision
                            _position.X -= collisionOverlap.Width;
                            _collisionBox = playerCollision;
                            _collisionBox.X -= collisionOverlap.Width;
                            colided = true;
                            continue;
                        }
                    }


                }
            }

            if (colided)
                return;
        }
        _collisionBox = endingCollision;
    }

    public void ProcessCamera(Map map)
    {
        if (_hasCamera)
        {
            Vector2 target = new(_position.X, _position.Y);

            // Make it stay inside the map
            if (target.X - Raylib.GetScreenWidth() / 2 <= 0)
                target.X = Raylib.GetScreenWidth() / 2;
            else if (target.X + Raylib.GetScreenWidth() / 2 >= map.GetMapSize().X)
                target.X = map.GetMapSize().X - Raylib.GetScreenWidth() / 2;

            if (target.Y - Raylib.GetScreenHeight() / 2 <= 0)
                target.Y = Raylib.GetScreenHeight() / 2;
            else if (target.Y + Raylib.GetScreenHeight() / 2 >= map.GetMapSize().Y)
                target.Y = map.GetMapSize().Y - Raylib.GetScreenHeight() / 2;

            // Make camera smooth
            // FIXME: fix camera jerkness when almost hitting the target
            _camera.Target.X = Raymath.Lerp(_camera.Target.X, target.X, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));
            _camera.Target.Y = Raymath.Lerp(_camera.Target.Y, target.Y, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));

            Raylib.BeginMode2D(_camera);
        }
    }

    public Vector2 GetPlayerCenterPosition()
    {

        Vector2 position = new(_position.X, _position.Y);
        position.X += _texture.Width / 2;
        position.Y += _texture.Height / 2;
        return position;
    }

    public bool IsLookingRight()
    {
        return _direction == -1;
    }

    public void AddVelocity(Vector2 velocity, float deltaTime)
    {
        _velocity.X += velocity.X * deltaTime;
        _velocity.Y += velocity.Y * deltaTime;
    }

    public void Draw()
    {
        Rectangle sourceRec = new(0.0f, 0.0f, (float)_texture.Width * _direction, (float)_texture.Height);

        Rectangle destRec = new(0, 0, (float)_texture.Width, (float)_texture.Height);

        Raylib.DrawTexturePro(_texture, sourceRec, destRec, new Vector2(_position.X * -1, _position.Y * -1), 0, Color.White);


        if (Utils.Debug())
        {
            Raylib.DrawRectangleRec(_collisionBox, Color.Green); // Debug
            int color = 0;
            foreach (var playerCollision in _playerCollisions)
            {
                Raylib.DrawRectangleRec(playerCollision, new(color, 100, 100, 255)); // Collision Interpolation
                color += 40;

            }
        }
    }
    public void ApplyState(PlayerState state)
    {
        _position = state.Position;
        _velocity = state.Velocity;
        Hook.ApplyState(state.HookState);
    }


}
