#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'. Honestly this will only run once so we don't care about performance
using System.Numerics;
using System.Text.RegularExpressions;
using VortexVise.GameGlobals;
using VortexVise.Models;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

/// <summary>
/// Handle logic for things that helps us kill other things.
/// </summary>
public static class WeaponLogic
{
    public static float WeaponSpawnTimer { get; set; } = 0;
    public static float WeaponRotation { get; set; } = 0;
    public static void Init()
    {
        string weaponLocation = "Resources/Weapons";
        // Read weapons from Resources/Weapons
        string[] weaponFiles = Directory.GetFiles(weaponLocation, "*.ini", SearchOption.TopDirectoryOnly);
        //string[] pngFiles = Directory.GetFiles(weaponLocation, "*.png", SearchOption.TopDirectoryOnly);
        var id = 0;
        foreach (var file in weaponFiles)
        {
            string fileContent = File.ReadAllText(file);
            try
            {

                var matchesWeapons = Regex.Matches(fileContent, @"\[WEAPON\][\s\S]*?(?=(\[WEAPON\]|$))");
                foreach (Match match in matchesWeapons.Cast<Match>())
                {
                    var weapon = new Weapon
                    {
                        // Name
                        Name = Regex.Match(match.Value, @"(?<=NAME\s*=\s*?)[\s\S]+?(?=\s\s)").Value.Trim(),

                        // Texture
                        TextureLocation = Regex.Match(match.Value, @"(?<=TEXTURE_LOCATION\s*=)[\S]+?(?=\s\s)").Value.Trim(),
                        ProjectileTextureLocation = Regex.Match(match.Value, @"(?<=PROJECTILE_TEXTURE_LOCATION\s*=)[\S]+?(?=\s\s)").Value
                    };
                    if (string.IsNullOrEmpty(weapon.TextureLocation)) throw new Exception("Can't find the texture location");
                    weapon.TextureLocation = weaponLocation + "/" + weapon.TextureLocation;
                    if (weapon.ProjectileTextureLocation != string.Empty) weapon.ProjectileTextureLocation = weaponLocation + "/" + weapon.ProjectileTextureLocation;

                    // Weapon Type
                    string weaponType = Regex.Match(match.Value, @"(?<=TYPE\s*=)(PISTOL|SMG|SHOTGUN|MELEE_BLUNT|MELEE_CUT|GRANADE|MINE|BAZOKA)(?=\s\s)").Value.Trim();
                    if (string.IsNullOrEmpty(weaponType)) throw new Exception("Can't read map COLLISIONS");
                    switch (weaponType)
                    {
                        case ("PISTOL"): weapon.WeaponType = Enums.WeaponType.Pistol; break;
                        case ("SMG"): weapon.WeaponType = Enums.WeaponType.SMG; break;
                        case ("SHOTGUN"): weapon.WeaponType = Enums.WeaponType.Shotgun; break;
                        case ("MELEE_BLUNT"): weapon.WeaponType = Enums.WeaponType.MeleeBlunt; break;
                        case ("MELEE_CUT"): weapon.WeaponType = Enums.WeaponType.MeleeCut; break;
                        case ("GRANADE"): weapon.WeaponType = Enums.WeaponType.Granade; break;
                        case ("MINE"): weapon.WeaponType = Enums.WeaponType.Mine; break;
                        case ("BAZOKA"): weapon.WeaponType = Enums.WeaponType.Bazoka; break;
                    }

                    // Get reloadDelay
                    weapon.ReloadDelay = float.Parse(Regex.Match(match.Value, @"(?<=RELOAD_TIME\s*=)[\d\.]*(?=\s\s)").Value);

                    // Color
                    var color = Regex.Match(match.Value, @"(?<=COLOR=)\d+,\d+,\d+,\d+").Value;
                    if (string.IsNullOrEmpty(color)) weapon.Color = Raylib.WHITE;
                    else
                    {
                        string[] rgba = color.Split(',');
                        weapon.Color = new Color(Convert.ToInt32(rgba[0]), Convert.ToInt32(rgba[1]), Convert.ToInt32(rgba[2]), Convert.ToInt32(rgba[3]));
                    }

                    // Damage
                    weapon.Damage = Convert.ToInt32(Regex.Match(match.Value, @"(?<=DAMAGE=)\d+").Value);

                    // Target Knockback
                    if (match.Value.Contains("TARGET_KNOCKBACK")) weapon.Knockback = Convert.ToInt32(Regex.Match(match.Value, @"(?<=TARGET_KNOCKBACK=)\d+").Value);

                    // Target Effect
                    string effect = Regex.Match(match.Value, @"(?<=TARGET_EFFECT\s*=)(COLD|WET|FIRE|ELETRICITY|FREZED|CONFUSION|DIZZY|GET_ROTATED|BLEEDING|POISON|HEAL)(?=\s\s)").Value.Trim();
                    if (!string.IsNullOrEmpty(effect))
                    {
                        switch (effect)
                        {
                            case ("COLD"): weapon.Effect = Enums.StatusEffects.Cold; break;
                            case ("WET"): weapon.Effect = Enums.StatusEffects.Wet; break;
                            case ("FIRE"): weapon.Effect = Enums.StatusEffects.Fire; break;
                            case ("ELETRICITY"): weapon.Effect = Enums.StatusEffects.Eletricity; break;
                            case ("FREEZED"): weapon.Effect = Enums.StatusEffects.Freezed; break;
                            case ("CONFUSION"): weapon.Effect = Enums.StatusEffects.Confusion; break;
                            case ("DIZZY"): weapon.Effect = Enums.StatusEffects.Dizzy; break;
                            case ("GET_ROTATED"): weapon.Effect = Enums.StatusEffects.GetRotatedIdiot; break;
                            case ("BLEEDING"): weapon.Effect = Enums.StatusEffects.Bleeding; break;
                            case ("POISON"): weapon.Effect = Enums.StatusEffects.Poison; break;
                            case ("HEAL"): weapon.Effect = Enums.StatusEffects.Heal; break;
                        }
                        weapon.EffectAmount = Convert.ToInt32(Regex.Match(match.Value, @"(?<=TARGET_EFFECT_AMOUNT=)\d+").Value);
                    }

                    // Self Knockback
                    if (match.Value.Contains("SELF_KNOCKBACK")) weapon.SelfKnockback = Convert.ToInt32(Regex.Match(match.Value, @"(?<=SELF_KNOCKBACK=)\d+").Value);

                    // Self Effect
                    effect = Regex.Match(match.Value, @"(?<=SELF_EFFECT\s*=)(COLD|WET|FIRE|ELETRICITY|FREZED|CONFUSION|DIZZY|GET_ROTATED|BLEEDING|POISON|HEAL)(?=\s\s)").Value.Trim();
                    if (!string.IsNullOrEmpty(effect))
                    {
                        switch (effect)
                        {
                            case ("COLD"): weapon.SelfEffect = Enums.StatusEffects.Cold; break;
                            case ("WET"): weapon.SelfEffect = Enums.StatusEffects.Wet; break;
                            case ("FIRE"): weapon.SelfEffect = Enums.StatusEffects.Fire; break;
                            case ("ELETRICITY"): weapon.SelfEffect = Enums.StatusEffects.Eletricity; break;
                            case ("FREEZED"): weapon.SelfEffect = Enums.StatusEffects.Freezed; break;
                            case ("CONFUSION"): weapon.SelfEffect = Enums.StatusEffects.Confusion; break;
                            case ("DIZZY"): weapon.SelfEffect = Enums.StatusEffects.Dizzy; break;
                            case ("GET_ROTATED"): weapon.SelfEffect = Enums.StatusEffects.GetRotatedIdiot; break;
                            case ("BLEEDING"): weapon.SelfEffect = Enums.StatusEffects.Bleeding; break;
                            case ("POISON"): weapon.SelfEffect = Enums.StatusEffects.Poison; break;
                            case ("HEAL"): weapon.SelfEffect = Enums.StatusEffects.Heal; break;
                        }
                        weapon.SelfEffectAmount = Convert.ToInt32(Regex.Match(match.Value, @"(?<=SELF_EFFECT_AMOUNT=)\d+").Value);
                        weapon.SelfEffectPercentageChance = Convert.ToInt32(Regex.Match(match.Value, @"(?<=SELF_EFFECT_CHANCE=)\d+").Value);
                    }

                    // Ammo
                    if (match.Value.Contains("AMMO"))
                        weapon.Ammo = Convert.ToInt32(Regex.Match(match.Value, @"(?<=AMMO=)\d+").Value);
                    else
                        weapon.Ammo = 1; // Defaults to 1 use if there is no ammo info



                    // Load the texture
                    weapon.Texture = Raylib.LoadTexture(weapon.TextureLocation); // TODO: Create a way of not loding replicated textures
                    if (weapon.ProjectileTextureLocation != string.Empty) weapon.ProjectileTexture = Raylib.LoadTexture(weapon.ProjectileTextureLocation);

                    // Define Id and add to list
                    weapon.Id = id;
                    GameAssets.Gameplay.Weapons.Add(weapon);
                    id++;
                    Console.WriteLine($"WEAPON \"{weapon.Name}\" ADDED");
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading weapon {file}: {ex.Message}");
            }
        }
        if (GameAssets.Gameplay.Weapons.Count == 0) throw new Exception("Can't find any weapon");
    }

    public static void CopyLastState(GameState currentGameState, GameState lastGameState)
    {
        foreach (var dropState in lastGameState.WeaponDrops)
        {
            currentGameState.WeaponDrops.Add(new WeaponDropState(dropState.WeaponState, dropState.DropTimer, dropState.Position, dropState.Velocity));
        }
    }
    public static void SpawnWeapons(GameState currentGameState, float deltaTime)
    {
        WeaponSpawnTimer += deltaTime;
        if (WeaponSpawnTimer > GameMatch.WeaponSpawnDelay)
        {
            WeaponSpawnTimer = 0;
            var weapon = GameAssets.Gameplay.Weapons.OrderBy(x => Guid.NewGuid()).First();

            Vector2 spawnPoint = GameMatch.CurrentMap.ItemSpawnPoints.Where(x => !currentGameState.WeaponDrops.Select(d => d.Position).Contains(x)).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (spawnPoint.X == 0 && spawnPoint.Y == 0) return;
            // Remove old weapons if there is anoter in same place
            var existingWeapon = currentGameState.WeaponDrops.FirstOrDefault(x => x.Position == spawnPoint);
            if (existingWeapon != null) currentGameState.WeaponDrops.Remove(existingWeapon);

            var weaponDrop = new WeaponDropState(new WeaponState(weapon, weapon.Ammo, weapon.Ammo, false, weapon.ReloadDelay, 0), spawnPoint);
            currentGameState.WeaponDrops.Add(weaponDrop);
            GameAssets.Sounds.PlaySound(GameAssets.Sounds.WeaponDrop);
        }
    }

    public static void UpdateWeaponDrops(GameState currentGameState, float deltaTime)
    {
        foreach (var drop in currentGameState.WeaponDrops)
        {
            drop.DropTimer += deltaTime;
        }
        //currentGameState.WeaponDrops.RemoveAll(x => x.DropTimer > 60);

        // create animation
        WeaponRotation -= deltaTime * 100;
    }

    public static void ProcessPlayerShooting(PlayerState currentPlayerState, GameState gameState, float deltaTime)
    {
        if (currentPlayerState.WeaponStates.Count <= 0) return; // FIXME: change this when player can grab more weapons

        var ws = currentPlayerState.WeaponStates[0];
        ws.ReloadTimer += deltaTime;
        if (currentPlayerState.Input.FireWeapon)
        {

            if (ws.ReloadTimer < ws.Weapon.ReloadDelay) return;

            switch (ws.Weapon.WeaponType)
            {
                case Enums.WeaponType.MeleeBlunt:
                {
                    var p = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
                    p.X -= 16 * currentPlayerState.Direction;
                    var hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 20, 32, 40), ws.Weapon, 0.2f, currentPlayerState.Direction, new(0, 0), false, currentPlayerState.WeaponStates[0]);

                    gameState.DamageHitBoxes.Add(hitbox);
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Dash, pitch: 0.5f);
                    break;
                }
                case Enums.WeaponType.MeleeCut:
                {
                    var p = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
                    p.X -= 16 * currentPlayerState.Direction;
                    var hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 20, 32, 40), ws.Weapon, 0.2f, currentPlayerState.Direction, new(0, 0), false, currentPlayerState.WeaponStates[0]);

                    gameState.DamageHitBoxes.Add(hitbox);
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Dash, pitch: 1.5f);
                    break;
                }
                case Enums.WeaponType.Shotgun:
                {
                    var p = PlayerLogic.GetPlayerCenterPosition(currentPlayerState.Position);
                    p.X -= 16 * currentPlayerState.Direction;

                    var hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 16, 16, 16), ws.Weapon, 0.2f, currentPlayerState.Direction, new(1000 * currentPlayerState.Direction * -1, 0), true, currentPlayerState.WeaponStates[0]);
                    gameState.DamageHitBoxes.Add(hitbox);

                    hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 16, 16, 16), ws.Weapon, 0.2f, currentPlayerState.Direction, new(1000 * currentPlayerState.Direction * -1, 50), true, currentPlayerState.WeaponStates[0]);
                    gameState.DamageHitBoxes.Add(hitbox);

                    hitbox = new DamageHitBoxState(currentPlayerState.Id, new(p.X - 16, p.Y - 16, 16, 16), ws.Weapon, 0.2f, currentPlayerState.Direction, new(1000 * currentPlayerState.Direction * -1, -50), true, currentPlayerState.WeaponStates[0]);
                    gameState.DamageHitBoxes.Add(hitbox);

                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Shotgun, pitch: 1.5f);
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.WeaponClick, pitch: 0.5f);
                    break;
                }
            }
            ws.ReloadTimer = 0;

            // Apply self knockback
            currentPlayerState.Velocity = new(currentPlayerState.Velocity.X + currentPlayerState.Direction * ws.Weapon.SelfKnockback, currentPlayerState.Velocity.Y);

            // Reduce ammo if not melee
            if (!(ws.Weapon.WeaponType == Enums.WeaponType.MeleeBlunt || ws.Weapon.WeaponType == Enums.WeaponType.MeleeCut))
            {
                ws.CurrentAmmo--;
            }

        }
    }

    public static void ProcessHitBoxes(GameState currentGameState, GameState lastGameState, float deltaTime)
    {
        foreach (var hitbox in currentGameState.DamageHitBoxes)
        {
            if (!hitbox.ShouldColide) hitbox.HitBoxTimer -= deltaTime;

            hitbox.HitBox = new Rectangle(hitbox.HitBox.X + hitbox.Velocity.X * deltaTime, hitbox.HitBox.Y + hitbox.Velocity.Y * deltaTime, hitbox.HitBox.Width, hitbox.HitBox.Height);

            // Check projectile collision with map
            if (hitbox.ShouldColide)
            {
                foreach (var collision in GameMatch.CurrentMap.Collisions)
                {
                    if (Raylib.CheckCollisionRecs(collision, hitbox.HitBox))
                    {
                        hitbox.ShouldDisappear = true;
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.HookHit, pitch: 2f);
                    }
                }
            }
            // Check if projectile is outside the map
            if ((hitbox.HitBox.X + hitbox.HitBox.Width <= 0) || (hitbox.HitBox.Y > MapLogic.GetMapSize().Y) || (hitbox.HitBox.Y + hitbox.HitBox.Height <= 0) || (hitbox.HitBox.X > MapLogic.GetMapSize().X))
                hitbox.ShouldDisappear = true;

            // Melee should follow the player that is using the weapon
            if (hitbox.Weapon.WeaponType == Enums.WeaponType.MeleeBlunt || hitbox.Weapon.WeaponType == Enums.WeaponType.MeleeCut)
            {
                var player = lastGameState.PlayerStates.FirstOrDefault(p => p.Id == hitbox.PlayerId);
                if (player != null)
                {
                    var p = PlayerLogic.GetPlayerCenterPosition(player.Position);
                    p.X -= 16 * player.Direction;
                    hitbox.HitBox = new(p.X - 16, p.Y - 20, 32, 40); // FIXME: we should get this from some other place because if it changes it should change everywhere
                }
            }

        }
        currentGameState.DamageHitBoxes.RemoveAll(x => x.HitBoxTimer <= 0 || x.ShouldDisappear);
    }

    public static void ApplyHitBoxesDamage(GameState gameState, PlayerState currentPlayerState)
    {
        var hitboxes = gameState.DamageHitBoxes.Where(x => x.PlayerId != currentPlayerState.Id);
        foreach (var hitbox in hitboxes)
        {
            if (Raylib.CheckCollisionRecs(currentPlayerState.Collision, hitbox.HitBox))
            {
                // Dude was hit by projectile
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.HookHit, pitch: 0.5f);
                currentPlayerState.DamagedTimer = 0.2f;
                currentPlayerState.Velocity = new(currentPlayerState.Velocity.X - hitbox.Direction * hitbox.Weapon.Knockback, currentPlayerState.Velocity.Y);
                currentPlayerState.HeathPoints -= hitbox.Weapon.Damage;
                hitbox.ShouldDisappear = true;

                // If is melee weapon it should spend ammo when hitting someone
                if (hitbox.Weapon.WeaponType == Enums.WeaponType.MeleeBlunt || hitbox.Weapon.WeaponType == Enums.WeaponType.MeleeCut)
                {
                    hitbox.WeaponState.CurrentAmmo--;
                }
            }

        }
    }
    public static void BreakPlayerWeapon(PlayerState currentPlayerState)
    {
        foreach (var weapon in currentPlayerState.WeaponStates)
        {
            if (weapon.CurrentAmmo <= 0)
                if (PlayerLogic.IsPlayerLocal(currentPlayerState.Id))
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Drop);
        }
        currentPlayerState.WeaponStates.RemoveAll(x => x.CurrentAmmo <= 0);
    }

    public static void Unload()
    {
        foreach (var w in GameAssets.Gameplay.Weapons)
        {
            Raylib.UnloadTexture(w.Texture);
            Raylib.UnloadTexture(w.ProjectileTexture);
        }
    }

}
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
