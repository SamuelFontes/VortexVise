using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VortexVise.GameGlobals;
using VortexVise.Models;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

public static class WeaponLogic
{
    static string weaponLocation = "Resources/Weapons";
    public static List<Weapon> Weapons { get; set; } = new List<Weapon>();
    public static float WeaponSpawnTimer { get; set; } = 0;
    public static float WeaponRotation { get; set; } = 0;
    public static bool AnimationCicle { get; set; } = false;
    public static void Init()
    {
        // Read weapons from Resources/Weapons
        string[] weaponFiles = Directory.GetFiles(weaponLocation, "*.ini", SearchOption.TopDirectoryOnly);
        string[] pngFiles = Directory.GetFiles(weaponLocation, "*.png", SearchOption.TopDirectoryOnly);
        var id = 0;
        foreach (var file in weaponFiles)
        {
            string fileContent = File.ReadAllText(file);
            try
            {

                var matchesWeapons = Regex.Matches(fileContent, @"\[WEAPON\][\s\S]*?(?=(\[WEAPON\]|$))");
                foreach (Match match in matchesWeapons)
                {
                    var weapon = new Weapon();

                    // Name
                    weapon.Name = Regex.Match(match.Value, @"(?<=NAME\s*=\s*?)[\s\S]+?(?=\s\s)").Value.Trim();

                    // Texture
                    weapon.TextureLocation = Regex.Match(match.Value, @"(?<=TEXTURE_LOCATION\s*=)[\S]+?(?=\s\s)").Value.Trim();
                    if (string.IsNullOrEmpty(weapon.TextureLocation)) throw new Exception("Can't find the texture location");
                    weapon.TextureLocation = weaponLocation + "/" + weapon.TextureLocation;

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



                    // Load the texture
                    weapon.Texture = Raylib.LoadTexture(weapon.TextureLocation); // TODO: Create a way of not loding replicated textures

                    // Define Id and add to list
                    weapon.Id = id;
                    Weapons.Add(weapon);
                    id++;
                    Console.WriteLine($"WEAPON \"{weapon.Name}\" ADDED");
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading weapon {file}: {ex.Message}");
            }
        }
        if (Weapons.Count == 0) throw new Exception("Can't find any weapon");
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
        if (WeaponSpawnTimer > MatchSettings.WeaponSpawnDelay && currentGameState.WeaponDrops.Count < 4)
        {
            WeaponSpawnTimer = 0;
            var weapon = Weapons.OrderBy(x => Guid.NewGuid()).First();
            var spawnPoint = MapLogic.CurrentMap.ItemSpawnPoints.OrderBy(x => Guid.NewGuid()).First();
            // Remove old weapons if there is anoter in same place
            var existingWeapon = currentGameState.WeaponDrops.FirstOrDefault(x => x.Position == spawnPoint);
            if (existingWeapon != null) currentGameState.WeaponDrops.Remove(existingWeapon);

            var weaponDrop = new WeaponDropState();
            weaponDrop.WeaponState = new WeaponState(weapon, 10, 10, false, weapon.ReloadDelay, 0);
            weaponDrop.Position = spawnPoint;
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
        currentGameState.WeaponDrops.RemoveAll(x => x.DropTimer > 60);

        // create animation
        WeaponRotation -= deltaTime * 100;
    }

    public static void DrawWeaponDrops(GameState currentGameState)
    {
        foreach (var drop in currentGameState.WeaponDrops)
        {
            Rectangle sourceRec = new(0.0f, 0.0f, drop.WeaponState.Weapon.Texture.width, drop.WeaponState.Weapon.Texture.height);

            Rectangle destRec = new(drop.Position.X + drop.WeaponState.Weapon.Texture.width * 0.5f, drop.Position.Y + drop.WeaponState.Weapon.Texture.height * 0.5f, drop.WeaponState.Weapon.Texture.width, drop.WeaponState.Weapon.Texture.height);

            Raylib.DrawTexturePro(drop.WeaponState.Weapon.Texture, sourceRec, destRec, new(drop.WeaponState.Weapon.Texture.width * 0.5f, drop.WeaponState.Weapon.Texture.height * 0.5f), (int)WeaponRotation, Raylib.WHITE);

            if (Utils.Debug()) Raylib.DrawRectangleRec(drop.Collision, Raylib.PURPLE); // Debug
        }
    }

    public static void Unload()
    {
        foreach (var w in Weapons) Raylib.UnloadTexture(w.Texture);

    }

}
