using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

public static class PlayerLogic
{
    static public Vector2 SpawnPoint;
    static private Texture _texture;
    static private readonly float _maxMoveSpeed = 350;
    static public readonly float _jumpForce = 450;
    static private readonly float _maxGravity = 1000;
    static private readonly float _acceleration = 750;
    static private Vector2 _collisionOffset = new(10, 6);

    static public void Init(bool isServer)
    {
        if (!isServer)
        {
            _texture = Raylib.LoadTexture("Resources/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
            PlayerHookLogic._texture = Raylib.LoadTexture("Resources/Sprites/GFX/hook.png");
        }
        else
        {
            _texture = new Texture() { width = 16, height = 16 }; // Player will always be this size
            PlayerHookLogic._texture = new Texture() { width = 8, height = 8 }; // if there are diffent kinds of hook change it here

        }
        SpawnPoint = new Vector2(MapLogic.MapTexture.width * 0.5f, MapLogic.MapTexture.height * 0.5f);
    }
    public static void CopyLastPlayerState(PlayerState currentPlayerState, PlayerState lastPlayerState)
    {
        currentPlayerState.Position = lastPlayerState.Position;
        currentPlayerState.Direction = lastPlayerState.Direction;
        currentPlayerState.Collision = lastPlayerState.Collision;
        currentPlayerState.Velocity = lastPlayerState.Velocity;
        currentPlayerState.IsTouchingTheGround = lastPlayerState.IsTouchingTheGround;
        currentPlayerState.HookState = lastPlayerState.HookState;
        currentPlayerState.CanDash = lastPlayerState.CanDash;
        currentPlayerState.TimeSinceJump = lastPlayerState.TimeSinceJump;
    }

    public static void AddPlayerTimers(PlayerState currentPlayerState, float deltaTime)
    {
        Utils.DebugText = currentPlayerState.TimeSinceJump.ToString();
        if (currentPlayerState.TimeSinceJump > 0) currentPlayerState.TimeSinceJump += deltaTime;
    }

    static public void SetPlayerDirection(PlayerState playerState)
    {
        if (playerState.Input.Right)
            playerState.Direction = -1;
        else if (playerState.Input.Left)
            playerState.Direction = 1;
    }

    static public void ProcessPlayerMovement(PlayerState currentPlayerState, float deltaTime)
    {
        if (currentPlayerState.Input.Right && !currentPlayerState.Input.Left)
        {
            currentPlayerState.AddVelocity(new(_acceleration * deltaTime, 0));
            if (currentPlayerState.Velocity.X > _maxMoveSpeed)
                currentPlayerState.SetVelocityX(RayMath.Lerp(currentPlayerState.Velocity.X, _maxMoveSpeed, 1f - (float)Math.Exp(-5f * deltaTime)));
        }
        else if (currentPlayerState.Input.Left && !currentPlayerState.Input.Right)
        {
            currentPlayerState.AddVelocity(new(-(_acceleration * deltaTime), 0));
            if (currentPlayerState.Velocity.X < _maxMoveSpeed * -1)
                currentPlayerState.SetVelocityX(RayMath.Lerp(currentPlayerState.Velocity.X, _maxMoveSpeed * -1, 1f - (float)Math.Exp(-5f * deltaTime)));
        }
        else
        {
            float desaceleration = currentPlayerState.IsTouchingTheGround || currentPlayerState.Velocity.Y == 0f ? 10f : 0.5f;
            currentPlayerState.SetVelocityX(RayMath.Lerp(currentPlayerState.Velocity.X, 0, 1f - (float)Math.Exp(-desaceleration * deltaTime)));
        }
    }

    public static void ProcessPlayerJump(PlayerState currentPlayerState, float deltaTime)
    {
        if (currentPlayerState.Input.Jump && currentPlayerState.IsTouchingTheGround)
        {
            GameSounds.PlaySound(GameSounds.Jump, volume: 0.5f);
            currentPlayerState.IsTouchingTheGround = false;
            currentPlayerState.SetVelocityY(-_jumpForce);
            currentPlayerState.CanDash = true;
            currentPlayerState.TimeSinceJump += deltaTime;
        }
        else if (currentPlayerState.Input.CancelHook && currentPlayerState.CanDash && currentPlayerState.TimeSinceJump > 0.1f)
        {
            MakePlayerDashOrDoubleJump(currentPlayerState, true);
        }

    }

    public static void ApplyPlayerGravity(PlayerState currentPlayerState, float deltaTime, float gravity)
    {
        float maxGravity = _maxGravity;
        if (!currentPlayerState.IsTouchingTheGround)
        {
            currentPlayerState.AddVelocity(new(0, gravity * deltaTime));
            if (currentPlayerState.Velocity.Y >= maxGravity)
                currentPlayerState.SetVelocityY(maxGravity);
        }
    }

    static public InputState GetInput(int gamepad)
    {
        // Receives gamepad; -1 is mouse and keyboard, 0 to 3 is gamepad slots
        InputState input = new();
        if (gamepad == -1)
        {
            // Mouse and keyboard
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                input.Left = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                input.Right = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W) || Raylib.IsKeyDown(KeyboardKey.KEY_UP))
                input.Up = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_S) || Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
                input.Down = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT))
                input.UILeft = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT))
                input.UIRight = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                input.UIUp = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                input.UIDown = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) || Raylib.IsKeyDown(KeyboardKey.KEY_K))
                input.Jump = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Raylib.IsKeyPressed(KeyboardKey.KEY_K))
                input.CancelHook = true;
            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT) || Raylib.IsKeyDown(KeyboardKey.KEY_J))
                input.Hook = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Raylib.IsKeyPressed(KeyboardKey.KEY_J) || Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
                input.Confirm = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                input.Back = true;
        }
        else
        {
            // Gamepad
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -0.5f)
                input.Left = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -0.5f)
                input.UILeft = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > 0.5f)
                input.Right = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > 0.5f)
                input.UIRight = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -0.5f)
                input.Up = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -0.5f)
                input.UIUp = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > 0.5f)
                input.Down = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > 0.5f)
                input.UIDown = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                input.Jump = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                input.CancelHook = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_LEFT))
                input.Hook = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                input.Confirm = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT))
                input.Back = true;

        }
        return input;
    }

    public static void ApplyPlayerVelocity(PlayerState currentPlayerState, float deltaTime)
    {
        currentPlayerState.Position += new Vector2(currentPlayerState.Velocity.X * deltaTime, currentPlayerState.Velocity.Y * deltaTime);
    }

    public static Rectangle GetPlayerCollision(Vector2 position)
    {
        return new(position.X + _collisionOffset.X, position.Y + _collisionOffset.Y, 12, 20);
    }

    public static void ApplyCollisions(PlayerState currentPlayerState, float deltaTime)
    {
        var wasTouchingTheGround = currentPlayerState.IsTouchingTheGround;
        currentPlayerState.IsTouchingTheGround = false;
        Rectangle endingCollision = GetPlayerCollision(currentPlayerState.Position);

        Vector2 mapSize = MapLogic.GetMapSize();
        // Apply ouside map collisions
        if (endingCollision.Y <= 0)
        {
            // TODO: if there is the invert world effect, this should be lethal
            currentPlayerState.Position = new Vector2(currentPlayerState.Position.X, 0);
            currentPlayerState.Velocity = new Vector2(currentPlayerState.Velocity.X, 0);

        }
        else if (endingCollision.Y > mapSize.Y)
        {
            // TODO: Kill the player
            currentPlayerState.Position = new(MapLogic.GetMapSize().X * 0.5f, MapLogic.GetMapSize().Y * 0.5f); // TODO: get spawnpoint
            currentPlayerState.Velocity = new(0, 0);
        }
        if (endingCollision.X <= 0)
        {
            currentPlayerState.Position = new(0 - (endingCollision.X - currentPlayerState.Position.X), currentPlayerState.Position.Y);
            currentPlayerState.Velocity = new Vector2(0, currentPlayerState.Velocity.Y);
        }
        else if (endingCollision.X + endingCollision.Width >= mapSize.X)
        {
            currentPlayerState.Position = new(mapSize.X - endingCollision.Width - _collisionOffset.X, currentPlayerState.Position.Y);
            currentPlayerState.Velocity = new Vector2(0, currentPlayerState.Velocity.Y);
        }

        // This will interpolate the collisions when the player is fast, otherwise he will go through stuff easily
        var playerCollisions = new List<Rectangle>();
        float interpolationAmount = 2f;
        for (float i = interpolationAmount; i > 0; i -= 0.2f)
        {
            Rectangle interpolatedCollision = endingCollision;
            if (currentPlayerState.Collision.X < endingCollision.X && endingCollision.X - currentPlayerState.Collision.X >= currentPlayerState.Collision.Width * i)
            {
                interpolatedCollision.X += currentPlayerState.Collision.Width * i;
            }
            else if (currentPlayerState.Collision.X > endingCollision.X && currentPlayerState.Collision.X - endingCollision.X >= currentPlayerState.Collision.Width * i)
            {
                interpolatedCollision.X -= currentPlayerState.Collision.Width * i;
            }

            if (currentPlayerState.Collision.Y < endingCollision.Y && endingCollision.Y - currentPlayerState.Collision.Y >= currentPlayerState.Collision.Height * i)
            {
                interpolatedCollision.Y += currentPlayerState.Collision.Height * i;
            }
            else if (currentPlayerState.Collision.Y > endingCollision.Y && currentPlayerState.Collision.Y - endingCollision.Y >= currentPlayerState.Collision.Height * i)
            {
                interpolatedCollision.Y -= currentPlayerState.Collision.Height * i;
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

                    if (currentPlayerState.Position.Y == collision.Y - _texture.height + _collisionOffset.Y)
                        currentPlayerState.IsTouchingTheGround = true;

                    Vector2 colliderCenter = new(collision.X + collision.Width * 0.5f, collision.Y + collision.Height * 0.5f);

                    if (collisionOverlap.Height < collisionOverlap.Width)
                    {
                        if (collisionOverlap.Y == collision.Y)
                        {
                            // Feet collision
                            currentPlayerState.Position = new(currentPlayerState.Position.X, collision.Y - _texture.height + _collisionOffset.Y);
                            currentPlayerState.Collision = playerCollision;
                            currentPlayerState.Collision = new(currentPlayerState.Collision.X, collision.Y - playerCollision.Height, currentPlayerState.Collision.Width, currentPlayerState.Collision.Height);
                            currentPlayerState.SetVelocityY(0);
                            currentPlayerState.IsTouchingTheGround = true;
                            currentPlayerState.TimeSinceJump = 0;
                            colided = true;
                            continue;
                        }
                        else
                        {
                            // Head collision
                            currentPlayerState.Position += new Vector2(0, collisionOverlap.Height);
                            currentPlayerState.Collision = playerCollision;
                            currentPlayerState.Collision = new Rectangle(currentPlayerState.Collision.X, currentPlayerState.Collision.Y + collisionOverlap.Height, currentPlayerState.Collision.Width, collisionOverlap.Height);
                            currentPlayerState.SetVelocityY(0.01f);
                            colided = true;
                            continue;
                        }
                    }
                    else
                    {

                        if (collisionOverlap.X > colliderCenter.X)
                        {
                            currentPlayerState.SetVelocityX(0);
                            // Right side of collision block on map
                            currentPlayerState.Position += new Vector2(collisionOverlap.Width, 0);
                            currentPlayerState.Collision = playerCollision;
                            currentPlayerState.Collision = new Rectangle(currentPlayerState.Collision.X + collisionOverlap.Width, currentPlayerState.Collision.Y, currentPlayerState.Collision.Width, currentPlayerState.Collision.height);
                            colided = true;
                            continue;
                        }
                        else
                        {
                            currentPlayerState.SetVelocityX(0);
                            // Left collision
                            currentPlayerState.Position -= new Vector2(collisionOverlap.Width, 0);
                            currentPlayerState.Collision = playerCollision;
                            currentPlayerState.Collision = new(currentPlayerState.Collision.X - collisionOverlap.Width, currentPlayerState.Collision.Y, currentPlayerState.Collision.Width, currentPlayerState.Collision.Height);
                            colided = true;
                            continue;
                        }
                    }


                }
            }

            if (wasTouchingTheGround && !currentPlayerState.IsTouchingTheGround)
            {
                currentPlayerState.TimeSinceJump += deltaTime;
                currentPlayerState.CanDash = true;
            }

            if (colided)
            {
                return;
            }
        }
        if (wasTouchingTheGround && !currentPlayerState.IsTouchingTheGround)
        {
            currentPlayerState.TimeSinceJump += deltaTime;
            currentPlayerState.CanDash = true;
        }
        currentPlayerState.Collision = endingCollision;
        return;
    }

    public static Vector2 GetPlayerCenterPosition(Vector2 playerPosition)
    {

        Vector2 position = new(playerPosition.X, playerPosition.Y);
        position.X += _texture.width * 0.5f;
        position.Y += _texture.height * 0.5f;
        return position;
    }
    public static void MakePlayerDashOrDoubleJump(PlayerState currentPlayerState, bool isDoubleJump)
    {
        var verticalForce = -_jumpForce * 0.2f;
        if (currentPlayerState.Input.Up) verticalForce *= 2;
        GameSounds.PlaySound(GameSounds.Dash, volume: 0.8f);
        if (isDoubleJump)
        {
            if (currentPlayerState.Velocity.Y < -_jumpForce)
            {
                currentPlayerState.Velocity = new(currentPlayerState.Velocity.X, verticalForce + currentPlayerState.Velocity.Y);
            }
            else
            {
                currentPlayerState.Velocity = new(currentPlayerState.Velocity.X, -_jumpForce);
            }
        }
        else
        {
            if (currentPlayerState.IsLookingRight())
                currentPlayerState.AddVelocity(new(PlayerLogic._jumpForce, verticalForce));
            else
                currentPlayerState.AddVelocity(new(-PlayerLogic._jumpForce, verticalForce));

            if (currentPlayerState.Velocity.Y > 0)
                currentPlayerState.SetVelocityY(verticalForce);
        }

        currentPlayerState.CanDash = false;
    }


    public static void DrawState(PlayerState playerState)
    {
        Rectangle sourceRec = new(0.0f, 0.0f, (float)_texture.width * playerState.Direction, _texture.height);

        Rectangle destRec = new(playerState.Position.X + _texture.width * 0.5f, playerState.Position.Y + _texture.height * 0.5f, _texture.width, _texture.height);

        var rotation = playerState.Animation.GetAnimationRotation();
        if (rotation != 0) destRec.Y -= 2f; // this adds a little bump to the walking animation

        Raylib.DrawTexturePro(_texture, sourceRec, destRec, new Vector2(_texture.width * 0.5f, _texture.height * 0.5f), rotation, Raylib.WHITE); // Draw Player 


        if (Utils.Debug())
        {
            Raylib.DrawRectangleRec(playerState.Collision, Raylib.GREEN); // Debug
        }
    }
}
