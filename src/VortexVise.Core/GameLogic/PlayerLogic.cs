﻿using System.Drawing;
using System.Numerics;
using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;
using VortexVise.Core.States;
using VortexVise.Core.Utilities;

namespace VortexVise.Core.GameLogic
{
    /// <summary>
    /// Handle player logic
    /// </summary>
    public static class PlayerLogic
    {
        static public void Init(IAssetService assetService)
        {
            GameAssets.Gameplay.HookTexture = assetService.LoadTexture("Resources/Sprites/GFX/hook.png");
        }
        static public void Unload(IAssetService assetService)
        {
            assetService.UnloadTexture(GameAssets.Gameplay.HookTexture);
        }
        public static void CopyLastPlayerState(PlayerState currentPlayerState, PlayerState lastPlayerState)
        {
            currentPlayerState.Position = lastPlayerState.Position;
            currentPlayerState.Direction = lastPlayerState.Direction;
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
            currentPlayerState.Stats = lastPlayerState.Stats;
            currentPlayerState.LastPlayerHitId = lastPlayerState.LastPlayerHitId;
            currentPlayerState.JetPackFuel = lastPlayerState.JetPackFuel;
        }

        public static void AddPlayerTimers(PlayerState currentPlayerState, float deltaTime)
        {
            if (currentPlayerState.TimeSinceJump > 0) currentPlayerState.TimeSinceJump += deltaTime;
            if (currentPlayerState.DamagedTimer > 0) currentPlayerState.DamagedTimer -= deltaTime;
        }
        public static void HandlePlayerDeath(PlayerState currentPlayerState, float deltaTime, GameState currentGameState, GameState lastGameState)
        {
            if (currentPlayerState.HeathPoints <= 0 && !currentPlayerState.IsDead)
            {
                GameAssets.Sounds.Death.Play();
                currentPlayerState.IsDead = true;
                currentPlayerState.WeaponStates.Clear();
                currentPlayerState.HookState.IsHookReleased = false;
                currentGameState.Animations.Add(new() { Animation = GameAssets.Animations.Blood, Position = currentPlayerState.Position });
                currentGameState.KillFeedStates.Add(new KillFeedState(currentPlayerState.LastPlayerHitId, currentPlayerState.Id));

                // Add stats
                currentPlayerState.Stats.Deaths++;
                if (currentPlayerState.LastPlayerHitId == currentPlayerState.Id || currentPlayerState.LastPlayerHitId == Guid.Empty)
                {
                    //currentPlayerState.Stats.Kills--;
                    GameAssets.Sounds.Death.Play(pitch: 0.8f);
                }
                else
                {
                    var p = lastGameState.PlayerStates.FirstOrDefault(x => x.Id == currentPlayerState.LastPlayerHitId);
                    if (p != null)
                    {
                        p.Stats.Kills++;
                        if (IsPlayerLocal(p.Id))
                        {
                            currentGameState.Animations.Add(new() { Animation = GameAssets.Animations.KillConfirmation, Position = new(p.Position.X - 16, p.Position.Y - 64) });
                            GameAssets.Sounds.Kill.Play();
                        }
                    }
                }
                currentPlayerState.LastPlayerHitId = Guid.Empty;
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
                    currentPlayerState.JetPackFuel = GameMatch.DefaultJetPackFuel;
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
                    currentPlayerState.SetVelocityX(Utils.Lerp(currentPlayerState.Velocity.X, GameMatch.PlayerMaxSpeed, 1f - (float)Math.Exp(-5f * deltaTime)));
            }
            else if (currentPlayerState.Input.Left && !currentPlayerState.Input.Right)
            {
                currentPlayerState.AddVelocity(new(-(GameMatch.PlayerAcceleration * deltaTime), 0));
                if (currentPlayerState.Velocity.X < GameMatch.PlayerMaxSpeed * -1)
                    currentPlayerState.SetVelocityX(Utils.Lerp(currentPlayerState.Velocity.X, GameMatch.PlayerMaxSpeed * -1, 1f - (float)Math.Exp(-5f * deltaTime)));
            }
            else
            {
                float desaceleration = currentPlayerState.IsTouchingTheGround || currentPlayerState.Velocity.Y == 0f ? 10f : 0.5f;
                currentPlayerState.SetVelocityX(Utils.Lerp(currentPlayerState.Velocity.X, 0, 1f - (float)Math.Exp(-desaceleration * deltaTime)));
            }
        }

        public static void ProcessPlayerJump(PlayerState currentPlayerState, float deltaTime)
        {
            if (currentPlayerState.Input.Jump && currentPlayerState.IsTouchingTheGround)
            {
                if (IsPlayerLocal(currentPlayerState.Id))
                    GameAssets.Sounds.Jump.Play(volume: 0.5f, overrideIfPlaying: false);
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

        public static void ProcessPlayerJetPack(PlayerState currentPlayerState, GameState currentGameState, float deltaTime)
        {
            if (currentPlayerState.Input.JetPack && currentPlayerState.JetPackFuel > 0)
            {
                if (IsPlayerLocal(currentPlayerState.Id)) GameAssets.Sounds.JetPack.Play(pitch: 1f, volume: 0.5f, overrideIfPlaying: false);
                currentGameState.Animations.Add(new() { Animation = GameAssets.Animations.Fire, Position = new(currentPlayerState.Position.X + new Random().Next(8, 16), currentPlayerState.Position.Y + 24) });

                if (currentPlayerState.Velocity.Y > 0) currentPlayerState.SetVelocityY(0);
                currentPlayerState.AddVelocity(new(0, -(GameMatch.PlayerJumpForce * 4 * deltaTime)));
                currentPlayerState.JetPackFuel -= deltaTime * 2;
                if (currentPlayerState.JetPackFuel <= 0) currentPlayerState.JetPackFuel = GameMatch.DefaultJetPackFuel * -1;
            }
            else
            {
                if (currentPlayerState.IsTouchingTheGround || currentPlayerState.HookState.IsHookAttached) currentPlayerState.JetPackFuel += deltaTime * 5;
                else currentPlayerState.JetPackFuel += deltaTime * 0.5f;
                if (currentPlayerState.JetPackFuel > GameMatch.DefaultJetPackFuel) currentPlayerState.JetPackFuel = GameMatch.DefaultJetPackFuel;
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

        public static void ApplyCollisions(PlayerState currentPlayerState, PlayerState lastPlayerState, float deltaTime, ICollisionService collisionService)
        {
            var wasTouchingTheGround = currentPlayerState.IsTouchingTheGround;
            currentPlayerState.IsTouchingTheGround = false;

            Vector2 mapSize = MapLogic.GetMapSize();

            // Apply map border collisions
            // ------------------------------------------------------------------------------
            if (currentPlayerState.Position.Y <= 0)
            {
                // TODO: if there is the invert world effect, this should be lethal
                currentPlayerState.Position = new Vector2(currentPlayerState.Position.X, 0);
                currentPlayerState.Velocity = new Vector2(currentPlayerState.Velocity.X, 0);

            }
            else if (currentPlayerState.Position.Y > mapSize.Y)
            {
                currentPlayerState.HeathPoints = -1;
                return;
            }
            else if (currentPlayerState.Collision.X <= 0)
            {
                currentPlayerState.Position = new(0 - (currentPlayerState.Collision.X - currentPlayerState.Position.X), currentPlayerState.Position.Y);
                currentPlayerState.Velocity = new Vector2(0, currentPlayerState.Velocity.Y);
            }
            else if (currentPlayerState.Collision.X + currentPlayerState.Collision.Width >= mapSize.X)
            {
                currentPlayerState.Position = new(mapSize.X - currentPlayerState.Collision.Width - 8, currentPlayerState.Position.Y);
                currentPlayerState.Velocity = new Vector2(0, currentPlayerState.Velocity.Y);
            }

            List<Rectangle> playerCollisions = [];
            var differenceX = currentPlayerState.Collision.X - lastPlayerState.Collision.X;
            var differenceY = currentPlayerState.Collision.Y - lastPlayerState.Collision.Y;
            playerCollisions.Add(new Rectangle((int)(lastPlayerState.Collision.X + differenceX * 0.2f), (int)(lastPlayerState.Collision.Y + differenceY * 0.2f), 16, 16));
            playerCollisions.Add(new Rectangle((int)(lastPlayerState.Collision.X + differenceX * 0.4f), (int)(lastPlayerState.Collision.Y + differenceY * 0.4f), 16, 16));
            playerCollisions.Add(new Rectangle((int)(lastPlayerState.Collision.X + differenceX * 0.6f), (int)(lastPlayerState.Collision.Y + differenceY * 0.6f), 16, 16));
            playerCollisions.Add(new Rectangle((int)(lastPlayerState.Collision.X + differenceX * 0.8f), (int)(lastPlayerState.Collision.Y + differenceY * 0.8f), 16, 16));

            playerCollisions.Add(currentPlayerState.Collision);

            bool colided = false;
            // Apply map collisions
            // -----------------------------------------
            foreach (var playerCollision in playerCollisions)
            {
                if (colided)
                {
                    break;
                }
                foreach (var collision in MapLogic.GetCollisions())
                {
                    if (collisionService.DetectCollision(playerCollision, collision))
                    {

                        // This means the player is inside the thing 
                        var collisionOverlap = collisionService.GetCollision(playerCollision, collision);

                        Vector2 colliderCenter = new(collision.X + collision.Width * 0.5f, collision.Y + collision.Height * 0.5f);

                        if (collisionOverlap.Height < collisionOverlap.Width)
                        {
                            if (collisionOverlap.Y == collision.Y)
                            {
                                // Feet collision
                                currentPlayerState.Position = new(currentPlayerState.Position.X, collision.Y - 32 + 8);
                                currentPlayerState.SetVelocityY(0);
                                currentPlayerState.IsTouchingTheGround = true;
                                currentPlayerState.TimeSinceJump = 0;
                                colided = true;
                            }
                            else
                            {
                                // Head collision
                                currentPlayerState.Position = new Vector2(currentPlayerState.Position.X, playerCollision.Y - 8 + collisionOverlap.Height);
                                currentPlayerState.SetVelocityY(0.01f);
                                colided = true;
                            }
                        }
                        else
                        {

                            if (collisionOverlap.X > colliderCenter.X)
                            {
                                currentPlayerState.SetVelocityX(0);
                                // Right side of collision block on map
                                currentPlayerState.Position += new Vector2(collisionOverlap.Width, 0);
                                colided = true;
                            }
                            else
                            {
                                currentPlayerState.SetVelocityX(0);
                                // Left collision
                                currentPlayerState.Position -= new Vector2(collisionOverlap.Width, 0);
                                colided = true;
                            }
                        }

                    }
                }
            }

            // Check if player is still on the ground
            if (wasTouchingTheGround && !currentPlayerState.IsTouchingTheGround)
            {
                var feetColided = false;
                Rectangle playerFeet = new Rectangle(currentPlayerState.Collision.X, currentPlayerState.Collision.Y + currentPlayerState.Collision.Height, currentPlayerState.Collision.Width, 1);
                foreach (var collision in MapLogic.GetCollisions()) if (collisionService.DetectCollision(collision, playerFeet)) feetColided = true;
                if (feetColided)
                {
                    currentPlayerState.IsTouchingTheGround = true;
                }
                else
                {
                    currentPlayerState.TimeSinceJump += deltaTime;
                    currentPlayerState.CanDash = true;
                }
            }

            return;
        }

        public static Vector2 GetPlayerCenterPosition(Vector2 playerPosition)
        {
            Vector2 position = new((int)playerPosition.X, (int)playerPosition.Y);
            position.X += 16;
            position.Y += 16;
            return position;
        }
        public static void MakePlayerDashOrDoubleJump(PlayerState currentPlayerState, bool isDoubleJump)
        {
            var verticalForce = -GameMatch.PlayerJumpForce * 0.2f;
            if (currentPlayerState.Input.Up) verticalForce *= 2;
            if (IsPlayerLocal(currentPlayerState.Id))
                GameAssets.Sounds.Dash.Play(volume: 0.8f);
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


        public static void ProcessPlayerPickUpItem(GameState currentState, PlayerState currentPlayerState, ICollisionService collisionService)
        {
            if (currentPlayerState.Input.GrabDrop)
            {
                Guid? idToRemove = null;
                foreach (var drop in currentState.WeaponDrops)
                {
                    if (collisionService.DetectCollision(drop.Collision, currentPlayerState.Collision))
                    {
                        idToRemove = drop.Id;
                        //foreach (var w in currentPlayerState.WeaponStates) w.IsEquipped = false;
                        //Heal
                        if (drop.WeaponState.Weapon.WeaponType == WeaponType.Heal)
                        {
                            currentPlayerState.HeathPoints += drop.WeaponState.Weapon.Damage;
                            if (currentPlayerState.HeathPoints > GameMatch.DefaultPlayerHeathPoints) currentPlayerState.HeathPoints = GameMatch.DefaultPlayerHeathPoints;
                            currentState.WeaponDrops.RemoveAll(x => idToRemove == x.Id);
                            if (IsPlayerLocal(currentPlayerState.Id))
                                GameAssets.Sounds.Drop.Play(pitch: 0.5f);
                            return;
                        }

                        currentPlayerState.WeaponStates.Clear(); // TODO: REMOVE THIS, the player should be able to have more weapons
                        drop.WeaponState.IsEquipped = true;
                        currentPlayerState.WeaponStates.Add(drop.WeaponState);
                        if (IsPlayerLocal(currentPlayerState.Id))
                            GameAssets.Sounds.WeaponClick.Play();
                        break;
                    }
                }
                if (idToRemove != null)
                    currentState.WeaponDrops.RemoveAll(x => idToRemove == x.Id);
            }
        }

        public static bool IsPlayerLocal(Guid playerId)
        {
            bool isPlayerLocal = false;
            if (GameCore.PlayerOneProfile.Gamepad != GamepadSlot.Disconnected && GameCore.PlayerOneProfile.Id == playerId) isPlayerLocal = true;
            if (GameCore.PlayerTwoProfile.Gamepad != GamepadSlot.Disconnected && GameCore.PlayerTwoProfile.Id == playerId) isPlayerLocal = true;
            if (GameCore.PlayerThreeProfile.Gamepad != GamepadSlot.Disconnected && GameCore.PlayerThreeProfile.Id == playerId) isPlayerLocal = true;
            if (GameCore.PlayerFourProfile.Gamepad != GamepadSlot.Disconnected && GameCore.PlayerFourProfile.Id == playerId) isPlayerLocal = true;
            return isPlayerLocal;
        }

    }
}
