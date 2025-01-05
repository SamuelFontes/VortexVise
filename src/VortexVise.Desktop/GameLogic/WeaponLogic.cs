#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'. Honestly this will only run once so we don't care about performance
using System.Numerics;
using VortexVise.Core.Enums;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Core.States;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.States;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Logic;

/// <summary>
/// Handle logic for things that helps us kill other things.
/// </summary>
public static class WeaponLogic
{
    public static float WeaponSpawnTimer { get; set; } = 0;
    public static float WeaponRotation { get; set; } = 0;

    public static void CopyLastState(GameState currentGameState, GameState lastGameState)
    {
        foreach (var dropState in lastGameState.WeaponDrops)
        {
            currentGameState.WeaponDrops.Add(new WeaponDropState(dropState.WeaponState, dropState.DropTimer, dropState.Position, dropState.Velocity));
        }
    }
    public static void SpawnWeapons(GameState currentGameState, float deltaTime, GameCore gameCore)
    {
        WeaponSpawnTimer += deltaTime;
        if (WeaponSpawnTimer > GameMatch.WeaponSpawnDelay)
        {
            WeaponSpawnTimer = 0;
            var weapon = GameAssets.Gameplay.Weapons.OrderBy(x => Guid.NewGuid()).First();

            Vector2 spawnPoint = GameMatch.CurrentMap.ItemSpawnPoints.OrderBy(x => Guid.NewGuid()).First().Deserialize();
            if (spawnPoint.X == 0 && spawnPoint.Y == 0) return;
            // Remove old weapons if there is another in same place
            var existingWeapon = currentGameState.WeaponDrops.FirstOrDefault(x => x.Position == spawnPoint);
            if (existingWeapon != null) currentGameState.WeaponDrops.Remove(existingWeapon);

            var weaponDrop = new WeaponDropState(new WeaponState(weapon, weapon.Ammo, weapon.Ammo, false, weapon.ReloadDelay, 0), spawnPoint);
            currentGameState.WeaponDrops.Add(weaponDrop);
            GameAssets.Sounds.WeaponDrop.Play();
        }
    }

    public static void UpdateWeaponDrops(GameState currentGameState, float deltaTime)
    {
        foreach (var drop in currentGameState.WeaponDrops)
        {
            drop.DropTimer += deltaTime;
        }

        // create animation
        WeaponRotation -= deltaTime * 100;
    }

    public static void ProcessPlayerShooting(PlayerState currentPlayerState, GameState gameState, float deltaTime, GameCore gameCore)
    {
        if (currentPlayerState.WeaponStates.Count <= 0) return; // FIXME: change this when player can grab more weapons

        var ws = currentPlayerState.WeaponStates[0];
        if (ws.ReloadTimer < ws.Weapon.ReloadDelay) ws.ReloadTimer += deltaTime;
        if (ws.ReloadTimer > ws.Weapon.ReloadDelay) ws.ReloadTimer = ws.Weapon.ReloadDelay;
        if (currentPlayerState.Input.FireWeapon)
        {

            if (ws.ReloadTimer < ws.Weapon.ReloadDelay) return;

            switch (ws.Weapon.WeaponType)
            {
                case WeaponType.MeleeBlunt:
                    {
                        var p = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
                        p.X -= 16 * currentPlayerState.Direction;
                        var hitbox = new DamageHitBoxState(currentPlayerState.Id, new((p.X - 16), p.Y - 20), new(0, 0, 32, 40), ws.Weapon, 0.2f, currentPlayerState.Direction, new(0, 0), false, currentPlayerState.WeaponStates[0]);

                        gameState.DamageHitBoxes.Add(hitbox);
                        GameAssets.Sounds.Dash.Play(pitch: 0.5f);
                        break;
                    }
                case WeaponType.MeleeCut:
                    {
                        var p = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
                        p.X -= 16 * currentPlayerState.Direction;
                        var hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 20), new(0, 0, 32, 40), ws.Weapon, 0.2f, currentPlayerState.Direction, new(0, 0), false, currentPlayerState.WeaponStates[0]);

                        gameState.DamageHitBoxes.Add(hitbox);
                        GameAssets.Sounds.Dash.Play(pitch: 1.5f);
                        break;
                    }
                case WeaponType.Shotgun:
                    {
                        var p = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
                        p.X -= 16 * currentPlayerState.Direction;

                        var hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 16), new(0, 0, 16, 16), ws.Weapon, 0.2f, currentPlayerState.Direction, new(1000 * currentPlayerState.Direction * -1, 0), true, currentPlayerState.WeaponStates[0]);
                        gameState.DamageHitBoxes.Add(hitbox);

                        hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 16), new(0, 0, 16, 16), ws.Weapon, 0.2f, currentPlayerState.Direction, new(1000 * currentPlayerState.Direction * -1, 50), true, currentPlayerState.WeaponStates[0]);
                        gameState.DamageHitBoxes.Add(hitbox);

                        hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 16), new(0, 0, 16, 16), ws.Weapon, 0.2f, currentPlayerState.Direction, new(1000 * currentPlayerState.Direction * -1, -50), true, currentPlayerState.WeaponStates[0]);
                        gameState.DamageHitBoxes.Add(hitbox);

                        GameAssets.Sounds.Shotgun.Play(pitch: 1.5f);
                        GameAssets.Sounds.WeaponClick.Play(pitch: 0.5f);
                        break;
                    }
                case WeaponType.Granade:
                    {
                        var p = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
                        p.X -= 16 * currentPlayerState.Direction;

                        Vector2 velocity = new(GameMatch.GranadeForce * currentPlayerState.Direction * -1, -GameMatch.GranadeForce);
                        if (currentPlayerState.Input.Left && currentPlayerState.Input.Down) // ↙ 
                            velocity = new(-GameMatch.GranadeForce, GameMatch.GranadeForce);
                        else if (currentPlayerState.Input.Right && currentPlayerState.Input.Down) // ↘
                            velocity = new(GameMatch.GranadeForce, GameMatch.GranadeForce);
                        else if (currentPlayerState.Input.Down) // ↓
                            velocity = new(0, GameMatch.GranadeForce);
                        else if (currentPlayerState.Input.Left && currentPlayerState.Input.Up) // ↖
                            velocity = new(-GameMatch.GranadeForce * 0.5f, -GameMatch.GranadeForce);
                        else if (currentPlayerState.Input.Right && currentPlayerState.Input.Up) // ↗
                            velocity = new(GameMatch.GranadeForce * 0.5f, -GameMatch.GranadeForce);
                        else if (currentPlayerState.Input.Left) // ↖
                            velocity = new(-GameMatch.GranadeForce, -GameMatch.GranadeForce);
                        else if (currentPlayerState.Input.Right) // ↗
                            velocity = new(GameMatch.GranadeForce, -GameMatch.GranadeForce);
                        else if (currentPlayerState.Input.Up) // ↑
                            velocity = new(0, -GameMatch.GranadeForce);
                        velocity = Utils.OnlyAddVelocity(velocity, currentPlayerState.Velocity, 2);

                        var hitbox = new DamageHitBoxState(currentPlayerState.Id, new((int)p.X - 16, (int)p.Y - 20), new(0, 0, 16, 16), ws.Weapon, 0.2f, currentPlayerState.Direction, velocity, true, currentPlayerState.WeaponStates[0]);
                        gameState.DamageHitBoxes.Add(hitbox);

                        GameAssets.Sounds.Dash.Play(pitch: 0.4f);
                        break;
                    }
            }
            ws.ReloadTimer = 0;

            // Apply self knockback
            currentPlayerState.Velocity = new(currentPlayerState.Velocity.X + currentPlayerState.Direction * ws.Weapon.SelfKnockback, currentPlayerState.Velocity.Y);

            // Reduce ammo if not melee
            if (!(ws.Weapon.WeaponType == WeaponType.MeleeBlunt || ws.Weapon.WeaponType == WeaponType.MeleeCut))
            {
                ws.CurrentAmmo--;
            }

        }
    }

    public static void ProcessHitBoxes(ICollisionService collisionService, GameState currentGameState, GameState lastGameState, float deltaTime, float gravity, GameCore gameCore)
    {
        foreach (var hitbox in currentGameState.DamageHitBoxes)
        {
            if (!hitbox.ShouldColide) hitbox.HitBoxTimer -= deltaTime;

            hitbox.Position = new((hitbox.Position.X + hitbox.Velocity.X * deltaTime), (hitbox.Position.Y + hitbox.Velocity.Y * deltaTime));
            hitbox.HitBox = new System.Drawing.Rectangle((int)hitbox.Position.X,
                (int)hitbox.Position.Y,
                hitbox.HitBox.Width,
                hitbox.HitBox.Height);

            // Check projectile collision with map
            if (hitbox.ShouldColide)
            {
                foreach (var collision in GameMatch.CurrentMap.Collisions)
                {
                    if (collisionService.CheckCollisionRecs(collision, hitbox.HitBox))
                    {
                        hitbox.ShouldDisappear = true;
                        GameAssets.Sounds.HookHit.Play(pitch: 2f);
                        if (hitbox.Weapon.WeaponType == WeaponType.Granade)
                        {
                            hitbox.Explode(currentGameState, gameCore);
                        }
                        break;
                    }
                }
            }
            // Check if projectile is outside the map
            if ((hitbox.HitBox.X + hitbox.HitBox.Width <= 0) || (hitbox.HitBox.Y > MapLogic.GetMapSize().Y) || /*(hitbox.HitBox.Y + hitbox.HitBox.Height <= 0) ||*/ (hitbox.HitBox.X > MapLogic.GetMapSize().X))
                hitbox.ShouldDisappear = true;

            // Melee should follow the player that is using the weapon
            if (hitbox.Weapon.WeaponType == WeaponType.MeleeBlunt || hitbox.Weapon.WeaponType == WeaponType.MeleeCut)
            {
                var player = lastGameState.PlayerStates.FirstOrDefault(p => p.Id == hitbox.PlayerId);
                if (player != null)
                {
                    var p = PlayerLogic.GetPlayerCenterPosition(player.Position);
                    p.X -= 16 * player.Direction;
                    hitbox.Position = new(p.X - 16, p.Y - 20);
                    hitbox.HitBox = new System.Drawing.Rectangle((int)hitbox.Position.X, (int)hitbox.Position.Y, 32, 40); // FIXME: we should get this from some other place because if it changes it should change everywhere
                }
            }
            if (hitbox.Weapon.WeaponType == WeaponType.Granade) // If is granade apply gravity
            {
                hitbox.Velocity = new(hitbox.Velocity.X, hitbox.Velocity.Y + gravity * 0.5f * deltaTime);

            }


        }
        currentGameState.DamageHitBoxes.RemoveAll(x => x.HitBoxTimer <= 0 || x.ShouldDisappear);
    }

    public static void ApplyHitBoxesDamage(ICollisionService collisionService, GameState gameState, PlayerState currentPlayerState, GameCore gameCore)
    {
        var hitboxes = gameState.DamageHitBoxes.Where(x => x.PlayerId != currentPlayerState.Id || (x.IsExplosion && !currentPlayerState.IsBot));// Adding friendly fire for bots is not a good idea
        foreach (var hitbox in hitboxes)
        {
            if (collisionService.CheckCollisionRecs(currentPlayerState.Collision.ToDrawingRectangle(), hitbox.HitBox))
            {
                // Dude was hit by projectile
                GameAssets.Sounds.HookHit.Play(pitch: 0.5f);
                currentPlayerState.HeathPoints -= hitbox.Weapon.Damage;
                hitbox.ShouldDisappear = true;
                currentPlayerState.DamagedTimer = 0.2f;
                currentPlayerState.Velocity = new(currentPlayerState.Velocity.X - hitbox.Direction * hitbox.Weapon.Knockback, currentPlayerState.Velocity.Y);

                // Set player id for owner to get the kill
                currentPlayerState.LastPlayerHitId = hitbox.PlayerId;

                // Show HitMaker if player is local 
                if (PlayerLogic.IsPlayerLocal(hitbox.PlayerId, gameCore))
                {
                    var player = gameState.PlayerStates.FirstOrDefault(x => x.Id == hitbox.PlayerId);
                    if (player == null) break;
                    gameState.Animations.Add(new() { Animation = GameAssets.Animations.HitMarker, Position = player.Position });
                    GameAssets.Sounds.HitMarker.Play();
                }

                if (hitbox.Weapon.WeaponType == WeaponType.Granade) hitbox.Explode(gameState, gameCore);

                // If is melee weapon it should spend ammo when hitting someone
                if (hitbox.Weapon.WeaponType == WeaponType.MeleeBlunt || hitbox.Weapon.WeaponType == WeaponType.MeleeCut)
                {
                    hitbox.WeaponState.CurrentAmmo--;
                }
            }

        }
    }
    public static void BreakPlayerWeapon(PlayerState currentPlayerState, GameCore gameCore)
    {
        foreach (var weapon in currentPlayerState.WeaponStates)
        {
            if (weapon.CurrentAmmo <= 0)
                if (PlayerLogic.IsPlayerLocal(currentPlayerState.Id, gameCore) && weapon.Weapon.WeaponType != WeaponType.Granade && weapon.Weapon.WeaponType != WeaponType.Mine)
                    GameAssets.Sounds.Drop.Play();
        }
        currentPlayerState.WeaponStates.RemoveAll(x => x.CurrentAmmo <= 0);
    }

}
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
