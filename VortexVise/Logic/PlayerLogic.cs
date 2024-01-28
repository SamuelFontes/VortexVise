using Raylib_cs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using VortexVise.States;
using VortexVise.Utilities;

namespace VortexVise.GameObjects;

public static class PlayerLogic
{
    static private Texture2D _texture;
    static private readonly float _maxMoveSpeed = 350;
    static private readonly float _acceleration = 750;
    static private Camera2D _camera;
    static private Vector2 _spawnPoint;

    static public void Init()
    {
        _texture = Raylib.LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
        _spawnPoint = new Vector2(Raylib.GetScreenWidth() / 2.0f, Raylib.GetScreenHeight() / 2.0f); // TODO: Get from map

        _camera = new Camera2D(_spawnPoint, new(0,0), 0, 1);
    }
    static public int ProcessDirection(float deltaTime, InputState input, PlayerState lastState)
    {
        var direction = lastState.Direction;
        if (input.Right)
        {
            direction = -1;
        }
        else if (input.Left)
        {
            direction = 1;
        }
        return direction;
    }

    static public (Vector2,bool) ProcessVelocity(float deltaTime, InputState input, PlayerState lastState, float gravity)
    {
        var velocity = lastState.Velocity;
        bool isTouchingTheGround = lastState.IsTouchingTheGround;
        if (input.Right)
        {
            velocity.X += _acceleration * deltaTime;
            if (velocity.X > _maxMoveSpeed)// && gravitationalForce == 0) // TODO: fix when player is in the air it should increase the max velocity
                velocity.X = _maxMoveSpeed;
        }
        else if (input.Left)
        {
            velocity.X -= _acceleration * deltaTime;
            if (velocity.X < _maxMoveSpeed * -1)// && gravitationalForce == 0)
                velocity.X = _maxMoveSpeed * -1;
        }
        else
        {
            float desaceleration = isTouchingTheGround || velocity.Y == 0f ? 10f : 0.5f;
            velocity.X = Raymath.Lerp(velocity.X, 0, 1f - (float)Math.Exp(-desaceleration * deltaTime));
        }


        if (input.Jump && isTouchingTheGround)
        {
            isTouchingTheGround = false;
            velocity.Y = -400;
        }

        float maxGravity = 500;
        if (!isTouchingTheGround)
        {
            velocity.Y += gravity * deltaTime;
            if (velocity.Y >= maxGravity)
                velocity.Y = maxGravity;
        }

        return (velocity,isTouchingTheGround);
    }

    static public InputState GetInput()
    {
        // TODO: Implement gamepad and stuff
        InputState input = new();
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

    public static Vector2 ProcessPosition(float deltaTime, PlayerState currentPlayerState, Vector2 lastPosition)
    {
        var position = lastPosition;
        if (currentPlayerState.Velocity.X != 0)
            position.X += currentPlayerState.Velocity.X * deltaTime;

        position.Y += currentPlayerState.Velocity.Y * deltaTime;
        return position;
    }

    public static (Vector2,Vector2,Rectangle,bool) ApplyCollisions(Vector2 currentPlayerPosition, Vector2 currentPlayerVelocity, Rectangle lastPlayerCollision)
    {
        // TODO: Refactor this please this makes my brain melt
        Vector2 collisionOffset = new(20, 12);
        Vector2 newPosition = currentPlayerPosition;
        Vector2 newVelocity = currentPlayerVelocity;
        Rectangle newCollision = lastPlayerCollision;
        var isTouchingTheGround = false;
        Rectangle endingCollision = new(currentPlayerPosition.X + collisionOffset.X, currentPlayerPosition.Y + collisionOffset.Y, 25, 40);

        Vector2 mapSize = MapLogic.GetMapSize();
        // Apply ouside map collisions
        if (endingCollision.Y <= 0)
        {
            newPosition.Y = 11.9f;
            newPosition.Y = 0;
        }
        else if (endingCollision.Y > mapSize.Y)
        {
            // TODO: Kill the player
            newPosition = new(MapLogic.GetMapSize().X / 2, MapLogic.GetMapSize().Y / 2);
            newVelocity.Y = 0;
            newVelocity.X = 0;

        }
        if (endingCollision.X <= 0)
        {
            newPosition.X = 0 - (endingCollision.X - newPosition.X);
            newVelocity.X = 0;
        }
        else if (endingCollision.X + endingCollision.Width >= mapSize.X)
        {
            newPosition.X = mapSize.X - endingCollision.Width - collisionOffset.X;
            newVelocity.X = 0;
        }

        // This will interpolate the collisions when the player is fast, otherwise he will go through stuff easily
        // WARNING: This solution only works if the player never goes in the minus coordinates, why? because at least for now he can't, if this changes please redo this collision interpolation crap
        var playerCollisions = new List<Rectangle>();
        float interpolationAmount = 2;
        for (float i = interpolationAmount; i > 0; i -= 0.2f)
        {
            Rectangle interpolatedCollision = endingCollision;
            if (newCollision.X < endingCollision.X && endingCollision.X - newCollision.X >= newCollision.Width * i)
            {
                interpolatedCollision.X += newCollision.Width * i;
            }
            else if (newCollision.X > endingCollision.X && newCollision.X - endingCollision.X >= newCollision.Width * i)
            {
                interpolatedCollision.X -= newCollision.Width * i;
            }

            if (newCollision.Y < endingCollision.Y && endingCollision.Y - newCollision.Y >= newCollision.Height * i)
            {
                interpolatedCollision.Y += newCollision.Height * i;
            }
            else if (newCollision.Y > endingCollision.Y && newCollision.Y - endingCollision.Y >= newCollision.Height * i)
            {
                interpolatedCollision.Y -= newCollision.Height * i;
            }
            if (interpolatedCollision.X != endingCollision.X || interpolatedCollision.Y != endingCollision.Y)
                playerCollisions.Add(interpolatedCollision);
        }


        playerCollisions.Add(endingCollision);

        // Apply map collisions
        foreach (var playerCollision in playerCollisions)
        {
            bool colided = false;
            foreach (var collision in MapLogic.GetCollisions())
            {
                if (Raylib.CheckCollisionRecs(playerCollision, collision))
                {

                    // This means the player is inside the thing 
                    var collisionOverlap = Raylib.GetCollisionRec(playerCollision, collision);

                    if (newPosition.Y == collision.Y - _texture.Height + collisionOffset.Y)
                        isTouchingTheGround = true;

                    Vector2 colliderCenter = new(collision.X + collision.Width * 0.5f, collision.Y + collision.Height * 0.5f);

                    if (collisionOverlap.Height < collisionOverlap.Width)
                    {
                        if (collisionOverlap.Y < colliderCenter.Y)
                        {
                            // Feet collision
                            newPosition.Y = collision.Y - _texture.Height + collisionOffset.Y;
                            newCollision = playerCollision;
                            newCollision.Y = (collision.Y - playerCollision.Height);
                            newVelocity.Y = 0;
                            isTouchingTheGround = true;
                            colided = true;
                            continue;
                        }
                        else
                        {
                            // Head collision
                            //newPosition.Y = collision.Y + collision.height + collisionOffset.Y;
                            //playerCollision.Y = collision.Y + collision.height;
                            newPosition.Y += collisionOverlap.Height;
                            newCollision = playerCollision;
                            newCollision.Y += collisionOverlap.Height;
                            newVelocity.Y = 0.01f;
                            colided = true;
                            continue;
                        }
                    }
                    else
                    {

                        if (collisionOverlap.X > colliderCenter.X)
                        {
                            newVelocity.X = 0;
                            // Right side of collision block on map
                            newPosition.X += collisionOverlap.Width;
                            newCollision = playerCollision;
                            newCollision.X += collisionOverlap.Width;
                            colided = true;
                            continue;
                        }
                        else
                        {
                            newVelocity.X = 0;
                            // Left collision
                            newPosition.X -= collisionOverlap.Width;
                            newCollision = playerCollision;
                            newCollision.X -= collisionOverlap.Width;
                            colided = true;
                            continue;
                        }
                    }


                }
            }

            if (colided)
            {
                return (newPosition,newVelocity,newCollision,isTouchingTheGround);
            }
        }
        newCollision = endingCollision;
        return (newPosition,newVelocity,newCollision,isTouchingTheGround);
    }

    public static void ProcessCamera(Vector2 targetPosition)
    {
        Vector2 target = new(targetPosition.X, targetPosition.Y);

        // Make it stay inside the map
        if (target.X - Raylib.GetScreenWidth() / 2 <= 0)
            target.X = Raylib.GetScreenWidth() / 2;
        else if (target.X + Raylib.GetScreenWidth() / 2 >= MapLogic.GetMapSize().X)
            target.X = MapLogic.GetMapSize().X - Raylib.GetScreenWidth() / 2;

        if (target.Y - Raylib.GetScreenHeight() / 2 <= 0)
            target.Y = Raylib.GetScreenHeight() / 2;
        else if (target.Y + Raylib.GetScreenHeight() / 2 >= MapLogic.GetMapSize().Y)
            target.Y = MapLogic.GetMapSize().Y - Raylib.GetScreenHeight() / 2;

        // Make camera smooth
        // FIXME: fix camera jerkness when almost hitting the target
        _camera.Target.X = Raymath.Lerp(_camera.Target.X, target.X, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));
        _camera.Target.Y = Raymath.Lerp(_camera.Target.Y, target.Y, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));

        Raylib.BeginMode2D(_camera);
    }

    public static Vector2 GetPlayerCenterPosition(Vector2 playerPosition)
    {

        Vector2 position = new(playerPosition.X, playerPosition.Y);
        position.X += _texture.Width / 2;
        position.Y += _texture.Height / 2;
        return position;
    }


    public static Vector2 AddVelocity(Vector2 velocity, float deltaTime)
    {
        var newVelocity = velocity;
        newVelocity.X += velocity.X * deltaTime;
        newVelocity.Y += velocity.Y * deltaTime;
        return newVelocity;
    }

    public static void DrawState(PlayerState playerState)
    {
        Rectangle sourceRec = new(0.0f, 0.0f, (float)_texture.Width * playerState.Direction, (float)_texture.Height);

        Rectangle destRec = new(0, 0, (float)_texture.Width, (float)_texture.Height);

        Raylib.DrawTexturePro(_texture, sourceRec, destRec, new Vector2(playerState.Position.X * -1, playerState.Position.Y * -1), 0, Color.White); // This is inverted for some reason


        if (Utils.Debug())
        {
            Raylib.DrawRectangleRec(playerState.Collision, Color.Green); // Debug
        }
    }
}
