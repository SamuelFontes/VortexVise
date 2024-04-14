using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

/// <summary>
/// Handle player logic
/// </summary>
public static class PlayerLogic
{
    static public void Init()
    {
        GameAssets.Gameplay.HookTexture = Raylib.LoadTexture("Resources/Sprites/GFX/hook.png");
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
        currentPlayerState.Animation = lastPlayerState.Animation;
        currentPlayerState.WeaponStates = lastPlayerState.WeaponStates;
        currentPlayerState.HeathPoints = lastPlayerState.HeathPoints;
        currentPlayerState.IsDead = lastPlayerState.IsDead;
        currentPlayerState.SpawnTimer = lastPlayerState.SpawnTimer;
        currentPlayerState.IsBot = lastPlayerState.IsBot;
        currentPlayerState.DamagedTimer = lastPlayerState.DamagedTimer;
    }

    public static void AddPlayerTimers(PlayerState currentPlayerState, float deltaTime)
    {
        if (currentPlayerState.TimeSinceJump > 0) currentPlayerState.TimeSinceJump += deltaTime;
        if (currentPlayerState.DamagedTimer > 0) currentPlayerState.DamagedTimer -= deltaTime;
    }
    public static void HandlePlayerDeath(PlayerState currentPlayerState, float deltaTime, GameState currentGameState)
    {
        if (currentPlayerState.HeathPoints <= 0 && !currentPlayerState.IsDead)
        {
            GameAssets.Sounds.PlaySound(GameAssets.Sounds.Death);
            currentPlayerState.IsDead = true;
            currentPlayerState.WeaponStates.Clear();
            currentPlayerState.HookState.IsHookReleased = false;
            currentGameState.Animations.Add(new() { Animation = GameAssets.Animations.Blood, Position = currentPlayerState.Position });
        }

        if (currentPlayerState.IsDead)
        {
            currentPlayerState.SpawnTimer += deltaTime;
            if (currentPlayerState.SpawnTimer > GameMatch.PlayerSpawnDelay)
            {
                currentPlayerState.IsDead = false;
                currentPlayerState.SpawnTimer = 0;
                currentPlayerState.Position = GameMatch.PlayerSpawnPoint;
                currentPlayerState.Velocity = new(0, 0);
                currentPlayerState.HeathPoints = GameMatch.DefaultPlayerHeathPoints;
            }
        }
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
            currentPlayerState.AddVelocity(new(GameMatch.PlayerAcceleration * deltaTime, 0));
            if (currentPlayerState.Velocity.X > GameMatch.PlayerMaxSpeed)
                currentPlayerState.SetVelocityX(RayMath.Lerp(currentPlayerState.Velocity.X, GameMatch.PlayerMaxSpeed, 1f - (float)Math.Exp(-5f * deltaTime)));
        }
        else if (currentPlayerState.Input.Left && !currentPlayerState.Input.Right)
        {
            currentPlayerState.AddVelocity(new(-(GameMatch.PlayerAcceleration * deltaTime), 0));
            if (currentPlayerState.Velocity.X < GameMatch.PlayerMaxSpeed * -1)
                currentPlayerState.SetVelocityX(RayMath.Lerp(currentPlayerState.Velocity.X, GameMatch.PlayerMaxSpeed * -1, 1f - (float)Math.Exp(-5f * deltaTime)));
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
            if (PlayerLogic.IsPlayerLocal(currentPlayerState.Id))
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Jump, volume: 0.5f);
            currentPlayerState.IsTouchingTheGround = false;
            currentPlayerState.SetVelocityY(-GameMatch.PlayerJumpForce);
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
        float maxGravity = GameMatch.PlayerMaxGravity;
        if (!currentPlayerState.IsTouchingTheGround)
        {
            currentPlayerState.AddVelocity(new(0, gravity * deltaTime));
            if (currentPlayerState.Velocity.Y >= maxGravity)
                currentPlayerState.SetVelocityY(maxGravity);
        }
    }

    public static void ApplyPlayerVelocity(PlayerState currentPlayerState, float deltaTime)
    {
        currentPlayerState.Position += new Vector2(currentPlayerState.Velocity.X * deltaTime, currentPlayerState.Velocity.Y * deltaTime);
    }

    public static Rectangle GetPlayerCollision(Vector2 position)
    {
        return new(position.X + GameMatch.PlayerCollisionOffset.X, position.Y + GameMatch.PlayerCollisionOffset.Y, 12, 20);
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
            currentPlayerState.HeathPoints = -1;
            return;
        }
        else if (endingCollision.X <= 0)
        {
            currentPlayerState.Position = new(0 - (endingCollision.X - currentPlayerState.Position.X), currentPlayerState.Position.Y);
            currentPlayerState.Velocity = new Vector2(0, currentPlayerState.Velocity.Y);
        }
        else if (endingCollision.X + endingCollision.Width >= mapSize.X)
        {
            currentPlayerState.Position = new(mapSize.X - endingCollision.Width - GameMatch.PlayerCollisionOffset.X, currentPlayerState.Position.Y);
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

                    if (currentPlayerState.Position.Y == collision.Y - currentPlayerState.Skin.Texture.height + GameMatch.PlayerCollisionOffset.Y)
                        currentPlayerState.IsTouchingTheGround = true;

                    Vector2 colliderCenter = new(collision.X + collision.Width * 0.5f, collision.Y + collision.Height * 0.5f);

                    if (collisionOverlap.Height < collisionOverlap.Width)
                    {
                        if (collisionOverlap.Y == collision.Y)
                        {
                            // Feet collision
                            currentPlayerState.Position = new(currentPlayerState.Position.X, collision.Y - currentPlayerState.Skin.Texture.height + GameMatch.PlayerCollisionOffset.Y);
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
        position.X += 32 * 0.5f;
        position.Y += 32 * 0.5f;
        return position;
    }
    public static void MakePlayerDashOrDoubleJump(PlayerState currentPlayerState, bool isDoubleJump)
    {
        var verticalForce = -GameMatch.PlayerJumpForce * 0.2f;
        if (currentPlayerState.Input.Up) verticalForce *= 2;
        if (PlayerLogic.IsPlayerLocal(currentPlayerState.Id))
            GameAssets.Sounds.PlaySound(GameAssets.Sounds.Dash, volume: 0.8f);
        if (isDoubleJump)
        {
            if (currentPlayerState.Velocity.Y < -GameMatch.PlayerJumpForce)
            {
                currentPlayerState.Velocity = new(currentPlayerState.Velocity.X, verticalForce + currentPlayerState.Velocity.Y);
            }
            else
            {
                currentPlayerState.Velocity = new(currentPlayerState.Velocity.X, -GameMatch.PlayerJumpForce);
            }
        }
        else
        {
            if (currentPlayerState.IsLookingRight())
                currentPlayerState.AddVelocity(new(GameMatch.PlayerJumpForce, verticalForce));
            else
                currentPlayerState.AddVelocity(new(-GameMatch.PlayerJumpForce, verticalForce));

            if (currentPlayerState.Velocity.Y > 0)
                currentPlayerState.SetVelocityY(verticalForce);
        }

        currentPlayerState.CanDash = false;
        // Triggers dash animation
        currentPlayerState.Animation.IsDashing = true;
        currentPlayerState.Animation.Rotation = 0;
        currentPlayerState.Animation.IsDashFacingRight = currentPlayerState.IsLookingRight();
    }


    public static void ProcessPlayerPickUpItem(GameState currentState, PlayerState currentPlayerState)
    {
        if (currentPlayerState.Input.GrabDrop)
        {
            Guid? idToRemove = null;
            foreach (var drop in currentState.WeaponDrops)
            {
                if (Raylib.CheckCollisionRecs(drop.Collision, currentPlayerState.Collision)) // TODO: if holding max weapons trade with current
                {
                    //foreach (var w in currentPlayerState.WeaponStates) w.IsEquipped = false;
                    currentPlayerState.WeaponStates.Clear(); // TODO: REMOVE THIS, the player should be able to have more weapons
                    drop.WeaponState.IsEquipped = true;
                    currentPlayerState.WeaponStates.Add(drop.WeaponState);
                    idToRemove = drop.Id;
                    if (PlayerLogic.IsPlayerLocal(currentPlayerState.Id))
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.WeaponClick);
                    break;
                }
            }
            if (idToRemove != null)
                currentState.WeaponDrops.RemoveAll(x => idToRemove == x.Id);
        }
    }

    public static bool IsPlayerLocal(int playerId)
    {
        bool isPlayerLocal = false;
        if (GameCore.PlayerOneProfile.Gamepad != -9 && GameCore.PlayerOneProfile.Id == playerId) isPlayerLocal = true;
        if (GameCore.PlayerTwoProfile.Gamepad != -9 && GameCore.PlayerTwoProfile.Id == playerId) isPlayerLocal = true;
        if (GameCore.PlayerThreeProfile.Gamepad != -9 && GameCore.PlayerThreeProfile.Id == playerId) isPlayerLocal = true;
        if (GameCore.PlayerFourProfile.Gamepad != -9 && GameCore.PlayerFourProfile.Id == playerId) isPlayerLocal = true;
        return isPlayerLocal;
    }

}
