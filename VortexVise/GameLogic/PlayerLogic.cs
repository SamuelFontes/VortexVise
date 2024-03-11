using Raylib_cs;
using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.States;
using VortexVise.Utilities;

namespace VortexVise.Logic;

public static class PlayerLogic
{
    static public Vector2 SpawnPoint;
    static private Texture2D _texture;
    static private readonly float _maxMoveSpeed = 350;
    static public readonly float _jumpForce = 450;
    static private readonly float _maxGravity = 1000;
    static private readonly float _acceleration = 750;
    static private Camera2D _camera;
    static private Vector2 _collisionOffset = new(10, 6);

    static public void Init(bool isServer)
    {
        if (!isServer)
        {
            _texture = Raylib.LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
            HookLogic._texture = Raylib.LoadTexture("Resources/Sprites/GFX/hook.png");
        }
        else
        {
            _texture = new Texture2D() { Width = 16, Height = 16 }; // Player will always be this size
            HookLogic._texture = new Texture2D() { Width = 8, Height = 8 }; // if there are diffent kinds of hook change it here

        }
        SpawnPoint = new Vector2(MapLogic._mapTexture.Width * 0.5f, MapLogic._mapTexture.Height * 0.5f);
        var cameraView = new Vector2(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f);

        _camera = new Camera2D(cameraView, new Vector2(0, 0), 0, 1);
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

    static public (Vector2, bool) ProcessVelocity(float deltaTime, InputState input, PlayerState lastState, float gravity)
    {
        var velocity = lastState.Velocity;
        bool isTouchingTheGround = lastState.IsTouchingTheGround;
        if (input.Right)
        {
            velocity.X += _acceleration * deltaTime;
            if (velocity.X > _maxMoveSpeed)
                velocity.X = Raymath.Lerp(velocity.X, _maxMoveSpeed, 1f - (float)Math.Exp(-5f * deltaTime));
        }
        else if (input.Left)
        {
            velocity.X -= _acceleration * deltaTime;
            if (velocity.X < _maxMoveSpeed * -1)
                velocity.X = Raymath.Lerp(velocity.X, _maxMoveSpeed * -1, 1f - (float)Math.Exp(-5f * deltaTime));
        }
        else
        {
            float desaceleration = isTouchingTheGround || velocity.Y == 0f ? 10f : 0.5f;
            velocity.X = Raymath.Lerp(velocity.X, 0, 1f - (float)Math.Exp(-desaceleration * deltaTime));
        }


        if (input.Jump && isTouchingTheGround)
        {
            GameAudio.PlaySound(GameAudio.Jump, volume: 0.5f);
            isTouchingTheGround = false;
            velocity.Y = -_jumpForce;
        }

        float maxGravity = _maxGravity;
        if (!isTouchingTheGround)
        {
            velocity.Y += gravity * deltaTime;
            if (velocity.Y >= maxGravity)
                velocity.Y = maxGravity;
        }

        return (velocity, isTouchingTheGround);
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
        if (Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsKeyPressed(KeyboardKey.K))
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

    public static Rectangle GetPlayerCollision(Vector2 position)
    {
        return new(position.X + _collisionOffset.X, position.Y + _collisionOffset.Y, 12, 20);
    }

    public static (Vector2, Vector2, Rectangle, bool) ApplyCollisions(Vector2 currentPlayerPosition, Vector2 currentPlayerVelocity, Rectangle lastPlayerCollision)
    {
        // TODO: Refactor this please this makes my brain melt
        Vector2 newPosition = currentPlayerPosition;
        Vector2 newVelocity = currentPlayerVelocity;
        Rectangle newCollision = lastPlayerCollision;
        var isTouchingTheGround = false;
        Rectangle endingCollision = GetPlayerCollision(currentPlayerPosition);

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
            newPosition = new(MapLogic.GetMapSize().X * 0.5f, MapLogic.GetMapSize().Y * 0.5f);
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
            newPosition.X = mapSize.X - endingCollision.Width - _collisionOffset.X;
            newVelocity.X = 0;
        }

        // This will interpolate the collisions when the player is fast, otherwise he will go through stuff easily
        // WARNING: This solution only works if the player never goes in the minus coordinates, why? because at least for now he can't, if this changes please redo this collision interpolation crap
        var playerCollisions = new List<Rectangle>();
        float interpolationAmount = 2f;
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

                    if (newPosition.Y == collision.Y - _texture.Height + _collisionOffset.Y)
                        isTouchingTheGround = true;

                    Vector2 colliderCenter = new(collision.X + collision.Width * 0.5f, collision.Y + collision.Height * 0.5f);

                    if (collisionOverlap.Height < collisionOverlap.Width)
                    {
                        if (collisionOverlap.Y == collision.Y)
                        {
                            // Feet collision
                            newPosition.Y = collision.Y - _texture.Height + _collisionOffset.Y;
                            newCollision = playerCollision;
                            newCollision.Y = collision.Y - playerCollision.Height;
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
                return (newPosition, newVelocity, newCollision, isTouchingTheGround);
            }
        }
        newCollision = endingCollision;
        return (newPosition, newVelocity, newCollision, isTouchingTheGround);
    }

    public static void ProcessCamera(Vector2 targetPosition)
    {
        Vector2 target = new(targetPosition.X, targetPosition.Y);

        // Make it stay inside the map
        if (target.X - GameCore.GameScreenWidth * 0.5f <= 0)
            target.X = GameCore.GameScreenWidth * 0.5f;
        else if (target.X + GameCore.GameScreenWidth * 0.5f >= MapLogic.GetMapSize().X)
            target.X = MapLogic.GetMapSize().X - GameCore.GameScreenWidth * 0.5f;

        if (target.Y - GameCore.GameScreenHeight * 0.5f <= 0)
            target.Y = GameCore.GameScreenHeight * 0.5f;
        else if (target.Y + GameCore.GameScreenHeight * 0.5f >= MapLogic.GetMapSize().Y)
            target.Y = MapLogic.GetMapSize().Y - GameCore.GameScreenHeight * 0.5f;

        // Make camera smooth
        // FIXME: fix camera jerkness when almost hitting the target
        _camera.Target.X = Raymath.Lerp(_camera.Target.X, target.X, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));
        _camera.Target.Y = Raymath.Lerp(_camera.Target.Y, target.Y, 1 - (float)Math.Exp(-3 * Raylib.GetFrameTime()));

        Raylib.BeginMode2D(_camera);
    }

    public static Vector2 GetPlayerCenterPosition(Vector2 playerPosition)
    {

        Vector2 position = new(playerPosition.X, playerPosition.Y);
        position.X += _texture.Width * 0.5f;
        position.Y += _texture.Height * 0.5f;
        return position;
    }



    public static void DrawState(PlayerState playerState)
    {
        Rectangle sourceRec = new(0.0f, 0.0f, (float)_texture.Width * playerState.Direction, _texture.Height);

        Rectangle destRec = new(playerState.Position.X + _texture.Width * 0.5f, playerState.Position.Y + _texture.Height * 0.5f, _texture.Width, _texture.Height);

        var rotation = playerState.Animation.GetAnimationRotation(playerState.Velocity, playerState.Input);
        if (rotation != 0) destRec.Y -= 2f; // this adds a little bump to the walking animation

        Raylib.DrawTexturePro(_texture, sourceRec, destRec, new Vector2(_texture.Width * 0.5f, _texture.Height * 0.5f), rotation, Color.White); // Draw Player 


        if (Utils.Debug())
        {
            Raylib.DrawRectangleRec(playerState.Collision, Color.Green); // Debug
        }
    }
}
