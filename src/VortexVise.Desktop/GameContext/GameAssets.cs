﻿
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using VortexVise.Core.Enums;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Models;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.Logic;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.Scenes;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.GameContext;

/// <summary>
/// 
/// GameAssets
///
/// This will hold all the global game assets. 
/// It's responsible for loading and unloading all assets to memory.
/// </summary>
public static class GameAssets
{
    /// <summary>
    /// Initialize all global assets when the game starts.
    /// </summary>
    public static void InitializeAssets(IAssetService assetService)
    {
        // Misc
        //---------------------------------------------------------
        Misc.Font = Raylib.LoadFont("Resources/Common/DeltaBlock.ttf");
        HUD.LoadHud(assetService);

        // Sounds
        //---------------------------------------------------------
        Sounds.Init();

        // Music And Ambience
        //---------------------------------------------------------

        // Gameplay
        //---------------------------------------------------------
        Gameplay.LoadWeapons(assetService);
        Gameplay.LoadSkins(assetService);

        // Animation
        //---------------------------------------------------------
        Animations.LoadAnimations(assetService);

        // Misc
        //---------------------------------------------------------
        GameUserInterface.InitUserInterface();

        Gameplay.LoadMaps(assetService);
    }

    /// <summary>
    /// Unload all game assets before game closes.
    /// </summary>
    public static void UnloadAssets(IAssetService assetService)
    {
        // Misc
        //---------------------------------------------------------
        Raylib.UnloadFont(Misc.Font);
        HUD.Unload(assetService);

        // Sounds
        //---------------------------------------------------------
        Sounds.Unload();

        // Music And Ambience
        //---------------------------------------------------------
        if (MusicAndAmbience.IsMusicPlaying) Raylib.UnloadMusicStream(MusicAndAmbience.Music);

        // Gameplay
        //---------------------------------------------------------
        foreach (var map in Gameplay.Maps) assetService.UnloadTexture(map.Texture);
        foreach (var skin in Gameplay.Skins) assetService.UnloadTexture(skin.Texture);
        foreach (var w in Gameplay.Weapons)
        {
            assetService.UnloadTexture(w.Texture);
            assetService.UnloadTexture(w.ProjectileTexture);
        }

        // Animation
        //---------------------------------------------------------
        Animations.UnloadAnimations(assetService);

        // Misc
        //---------------------------------------------------------
        GameUserInterface.UnloadUserInterface();
    }

    /// <summary>
    /// Miscellaneous assets
    /// </summary>
    public static class Misc
    {
        public static Font Font;                                                    // Global font
    }

    /// <summary>
    /// Gameplay related assets.
    /// </summary>
    public static class Gameplay
    {
        public static Texture HookTexture;
        public static List<Map> Maps { get; set; } = [];
        public static List<Weapon> Weapons { get; set; } = [];
        public static List<Skin> Skins { get; set; } = [];

        public static void LoadSkins(IAssetService assetService)
        {
            string[] skins = Directory.GetFiles("Resources/Skins", "*.png", SearchOption.TopDirectoryOnly);
            foreach (string skin in skins)
            {
                var s = new Skin
                {
                    Id = Utils.GetFileChecksum(skin),
                    TextureLocation = skin,
                    Texture = assetService.LoadTexture(skin)
                };
                Skins.Add(s);
            }
            if (Skins.Count == 0) throw new Exception("Couldn't load any player skin");
        }

        /// <summary>
        /// Loads only map hash to memory
        /// </summary>
        public static void LoadMapsHash()
        {
            string mapLocation = "Resources/Maps";
            string[] mapFiles = Directory.GetFiles(mapLocation, "*.json", SearchOption.TopDirectoryOnly);
            foreach (var file in mapFiles)
            {
                try
                {
                    var map = JsonSerializer.Deserialize(File.ReadAllText(file), SourceGenerationContext.Default.Map);
                    if (map == null) throw new Exception("Can't read map file");
                    map.Id = Utils.GetFileChecksum(file);
                    Maps.Add(map);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
        public static void LoadMaps(IAssetService assetService)
        {
            string mapLocation = "Resources/Maps";
            // Get all files from the Resources/Maps folder to read list of avaliable game levels aka maps
            string[] mapFiles = Directory.GetFiles(mapLocation, "*.json", SearchOption.TopDirectoryOnly);
            string[] pngFiles = Directory.GetFiles(mapLocation, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var file in mapFiles)
            {
                string fileContent = File.ReadAllText(file);
                try
                {
                    var map = JsonSerializer.Deserialize(File.ReadAllText(file), SourceGenerationContext.Default.Map);
                    if (map == null) throw new Exception("Can't read map file");
                    if (map.Name == null || map.Name.Length == 0) throw new Exception("Can't read map NAME");
                    if (map.Collisions.Count == 0) throw new Exception("Can't read map COLLISIONS");
                    if (map.PlayerSpawnPoints.Count == 0) throw new Exception("Can't read map PlayerSpawnPoints");
                    if (map.EnemySpawnPoints.Count == 0) throw new Exception("Can't read map EnemySpawnPoints");
                    if (map.ItemSpawnPoints.Count == 0) throw new Exception("Can't read map ItemSpawnPoints");
                    if (map.GameModes.Count == 0) throw new Exception("Can't read map GAME_MODES");
                    //if (!pngFiles.Contains(map.TextureLocation)) throw new Exception($"Can't find image file {map.TextureLocation}");
                    map.MapLocation = file;
                    map.Id = Utils.GetFileChecksum(file);
                    map.Texture = assetService.LoadTexture(map.TextureLocation);
                    Maps.Add(map);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error reading map {file}: {ex.Message}");
                }
            }
            if (Maps.Count == 0) throw new Exception("Can't find any map");
            MapLogic.LoadRandomMap();
        }

        public static void LoadWeapons(IAssetService assetService)
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
                        var weapon = new Weapon();
                        // Name
                        weapon.Name = Regex.Match(match.Value, @"(?<=NAME\s*=\s*?)[\s\S]+?(\n|\s\s)").Value.Trim();

                        // Texture
                        weapon.TextureLocation = Regex.Match(match.Value, @"(?<=TEXTURE_LOCATION\s*=)[\S]+?(\s\s|\r\n|\n)").Value.Trim();
                        weapon.ProjectileTextureLocation = Regex.Match(match.Value, @"(?<=PROJECTILE_TEXTURE_LOCATION\s*=)[\S]+?(\s\s|\r\n|\n)").Value.Trim();

                        if (string.IsNullOrEmpty(weapon.TextureLocation)) throw new Exception("Can't find the texture location");
                        weapon.TextureLocation = weaponLocation + "/" + weapon.TextureLocation;
                        if (weapon.ProjectileTextureLocation != string.Empty) weapon.ProjectileTextureLocation = weaponLocation + "/" + weapon.ProjectileTextureLocation;

                        // Weapon Type
                        string weaponType = Regex.Match(match.Value, @"(?<=TYPE\s*=)(PISTOL|SMG|SHOTGUN|MELEE_BLUNT|MELEE_CUT|GRANADE|MINE|BAZOKA|HEAL)(\n|\s\s)").Value.Trim();
                        if (string.IsNullOrEmpty(weaponType)) throw new Exception("Can't read Weapon Type");
                        switch (weaponType)
                        {
                            case ("PISTOL"): weapon.WeaponType = WeaponType.Pistol; break;
                            case ("SMG"): weapon.WeaponType = WeaponType.SMG; break;
                            case ("SHOTGUN"): weapon.WeaponType = WeaponType.Shotgun; break;
                            case ("MELEE_BLUNT"): weapon.WeaponType = WeaponType.MeleeBlunt; break;
                            case ("MELEE_CUT"): weapon.WeaponType = WeaponType.MeleeCut; break;
                            case ("GRANADE"): weapon.WeaponType = WeaponType.Granade; break;
                            case ("MINE"): weapon.WeaponType = WeaponType.Mine; break;
                            case ("BAZOKA"): weapon.WeaponType = WeaponType.Bazoka; break;
                            case ("HEAL"): weapon.WeaponType = WeaponType.Heal; break;
                        }

                        // Get reloadDelay
                        if (weapon.WeaponType != WeaponType.Heal)
                            weapon.ReloadDelay = float.Parse(Regex.Match(match.Value, @"(?<=RELOAD_TIME\s*=)[\d\.]*(\n|\s\s)").Value, CultureInfo.InvariantCulture);

                        // Color
                        var color = Regex.Match(match.Value, @"(?<=COLOR=)\d+,\d+,\d+,\d+").Value;
                        if (string.IsNullOrEmpty(color)) weapon.Color = System.Drawing.Color.White;
                        else
                        {
                            string[] rgba = color.Split(',');
                            weapon.Color = new Color(Convert.ToInt32(rgba[0]), Convert.ToInt32(rgba[1]), Convert.ToInt32(rgba[2]), Convert.ToInt32(rgba[3])).ToDrawingColor();
                        }

                        // Damage
                        weapon.Damage = Convert.ToInt32(Regex.Match(match.Value, @"(?<=DAMAGE=)\d+").Value);

                        // Target Knockback
                        if (match.Value.Contains("TARGET_KNOCKBACK")) weapon.Knockback = Convert.ToInt32(Regex.Match(match.Value, @"(?<=TARGET_KNOCKBACK=)\d+").Value);

                        // Target Effect
                        string effect = Regex.Match(match.Value, @"(?<=TARGET_EFFECT\s*=)(COLD|WET|FIRE|ELETRICITY|FREZED|CONFUSION|DIZZY|GET_ROTATED|BLEEDING|POISON|HEAL)(\n|\s\s)").Value.Trim();
                        if (!string.IsNullOrEmpty(effect))
                        {
                            switch (effect)
                            {
                                case ("COLD"): weapon.Effect = StatusEffects.Cold; break;
                                case ("WET"): weapon.Effect = StatusEffects.Wet; break;
                                case ("FIRE"): weapon.Effect = StatusEffects.Fire; break;
                                case ("ELETRICITY"): weapon.Effect = StatusEffects.Eletricity; break;
                                case ("FREEZED"): weapon.Effect = StatusEffects.Freezed; break;
                                case ("CONFUSION"): weapon.Effect = StatusEffects.Confusion; break;
                                case ("DIZZY"): weapon.Effect = StatusEffects.Dizzy; break;
                                case ("GET_ROTATED"): weapon.Effect = StatusEffects.GetRotatedIdiot; break;
                                case ("BLEEDING"): weapon.Effect = StatusEffects.Bleeding; break;
                                case ("POISON"): weapon.Effect = StatusEffects.Poison; break;
                                case ("HEAL"): weapon.Effect = StatusEffects.Heal; break;
                            }
                            weapon.EffectAmount = Convert.ToInt32(Regex.Match(match.Value, @"(?<=TARGET_EFFECT_AMOUNT=)\d+").Value);
                        }

                        // Self Knockback
                        if (match.Value.Contains("SELF_KNOCKBACK")) weapon.SelfKnockback = Convert.ToInt32(Regex.Match(match.Value, @"(?<=SELF_KNOCKBACK=)\d+").Value);

                        // Self Effect
                        effect = Regex.Match(match.Value, @"(?<=SELF_EFFECT\s*=)(COLD|WET|FIRE|ELETRICITY|FREZED|CONFUSION|DIZZY|GET_ROTATED|BLEEDING|POISON|HEAL)(\n|\s\s)").Value.Trim();
                        if (!string.IsNullOrEmpty(effect))
                        {
                            switch (effect)
                            {
                                case ("COLD"): weapon.SelfEffect = StatusEffects.Cold; break;
                                case ("WET"): weapon.SelfEffect = StatusEffects.Wet; break;
                                case ("FIRE"): weapon.SelfEffect = StatusEffects.Fire; break;
                                case ("ELETRICITY"): weapon.SelfEffect = StatusEffects.Eletricity; break;
                                case ("FREEZED"): weapon.SelfEffect = StatusEffects.Freezed; break;
                                case ("CONFUSION"): weapon.SelfEffect = StatusEffects.Confusion; break;
                                case ("DIZZY"): weapon.SelfEffect = StatusEffects.Dizzy; break;
                                case ("GET_ROTATED"): weapon.SelfEffect = StatusEffects.GetRotatedIdiot; break;
                                case ("BLEEDING"): weapon.SelfEffect = StatusEffects.Bleeding; break;
                                case ("POISON"): weapon.SelfEffect = StatusEffects.Poison; break;
                                case ("HEAL"): weapon.SelfEffect = StatusEffects.Heal; break;
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
                        weapon.Texture = assetService.LoadTexture(weapon.TextureLocation); // TODO: Create a way of not loading replicated textures
                        if (weapon.ProjectileTextureLocation != string.Empty) weapon.ProjectileTexture = assetService.LoadTexture(weapon.ProjectileTextureLocation);

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
            if (GameAssets.Gameplay.Weapons.Count == 0) throw new Exception("Can't find any weapon");
        }

    }

    /// <summary>
    /// Music and ambience sounds.
    /// </summary>
    public static class MusicAndAmbience
    {
        public static Music Music;                                                  // Background music
        public static Music Ambience;                                               // Background sounds

        public static bool IsMusicPlaying = false;
        public static bool IsAmbiencePlaying = false;

        // Music list
        //---------------------------------------------------------
        public static string MusicAssetPixelatedDiscordance = "Resources/Audio/Music/PixelatedDiscordance.mp3";
        public static string MusicAssetNotGonnaLeoThis = "Resources/Audio/Music/NotGonnaLeoThis.mp3";

        public static void PlayMusic(string music, GameCore gameCore)
        {
            if (IsMusicPlaying) StopMusic(gameCore);
            Music = Raylib.LoadMusicStream(music);
            Raylib.PlayMusicStream(Music);
            IsMusicPlaying = true;
            Raylib.SetMusicVolume(Music, 0.8f);
        }

        public static void StopMusic(GameCore gameCore)
        {
            if (!IsMusicPlaying) return;
            Sounds.PlaySound(Sounds.VinylScratch,gameCore, pitch: 0.7f);
            Raylib.StopMusicStream(Music);
            Raylib.UnloadMusicStream(Music);
            IsMusicPlaying = false;
        }

        public static void PlayCustomMusic(string musicName, GameCore gameCore)
        {
            if (IsMusicPlaying) StopMusic(gameCore);
            Music = Raylib.LoadMusicStream($"Resources/Audio/Music/{musicName}.mp3");
            Raylib.PlayMusicStream(Music);
            IsMusicPlaying = true;
            Raylib.SetMusicVolume(Music, 0.8f);
        }

        // Ambience Sounds
        //---------------------------------------------------------
        public static void PlayAmbience(string ambience)
        {
            if (IsAmbiencePlaying) Raylib.UnloadMusicStream(Ambience);
            Ambience = Raylib.LoadMusicStream(ambience);
        }

    }

    /// <summary>
    /// Sound effects.
    /// </summary>
    public static class Sounds
    {
        public static Sound HookShoot;
        public static Sound HookHit;
        public static Sound Jump;
        public static Sound Dash;
        public static Sound Click;
        public static Sound Selection;
        public static Sound WeaponDrop;
        public static Sound WeaponClick;
        public static Sound Death;
        public static Sound Shotgun;
        public static Sound Drop;
        public static Sound HitMarker;
        public static Sound Explosion;
        public static Sound VinylScratch;
        public static Sound Kill;
        public static Sound JetPack;
        public static void Init()
        {
            HookShoot = Raylib.LoadSound("Resources/Audio/FX/hook_fire.wav");
            HookHit = Raylib.LoadSound("Resources/Audio/FX/hook_hit.wav");
            Jump = Raylib.LoadSound("Resources/Audio/FX/jump.wav");
            Dash = Raylib.LoadSound("Resources/Audio/FX/dash.wav");
            Click = Raylib.LoadSound("Resources/Audio/FX/click.wav");
            Selection = Raylib.LoadSound("Resources/Audio/FX/selection.wav");
            WeaponDrop = Raylib.LoadSound("Resources/Audio/FX/metal_drop.wav");
            WeaponClick = Raylib.LoadSound("Resources/Audio/FX/weapon_click.wav");
            Death = Raylib.LoadSound("Resources/Audio/FX/death3.ogg");
            Shotgun = Raylib.LoadSound("Resources/Audio/FX/shotgun.ogg");
            Drop = Raylib.LoadSound("Resources/Audio/FX/Drop.ogg");
            HitMarker = Raylib.LoadSound("Resources/Audio/FX/hitmarker.ogg");
            Explosion = Raylib.LoadSound("Resources/Audio/FX/explosion.ogg");
            VinylScratch = Raylib.LoadSound("Resources/Audio/FX/vinyl_scratch.ogg");
            Kill = Raylib.LoadSound("Resources/Audio/FX/kill.ogg");
            JetPack = Raylib.LoadSound("Resources/Audio/FX/jetpack.ogg");
        }
        public static void Unload()
        {
            Raylib.UnloadSound(HookShoot);
            Raylib.UnloadSound(HookHit);
            Raylib.UnloadSound(Jump);
            Raylib.UnloadSound(Dash);
            Raylib.UnloadSound(Click);
            Raylib.UnloadSound(Selection);
            Raylib.UnloadSound(WeaponDrop);
            Raylib.UnloadSound(WeaponClick);
            Raylib.UnloadSound(Death);
            Raylib.UnloadSound(Shotgun);
            Raylib.UnloadSound(Drop);
            Raylib.UnloadSound(HitMarker);
            Raylib.UnloadSound(Explosion);
            Raylib.UnloadSound(VinylScratch);
            Raylib.UnloadSound(Kill);
            Raylib.UnloadSound(JetPack);
        }
        public static void PlaySound(Sound sound, GameCore gameCore, float pan = 0.5f, float pitch = 1f, float volume = 1f, bool overrideIfPlaying = true)
        {
            if (gameCore.IsServer) return; // Audio don't play on the server
            if (!overrideIfPlaying && Raylib.IsSoundPlaying(sound)) return;

            volume *= GameSettings.VolumeSounds;
            Raylib.SetSoundPan(sound, pan);
            Raylib.SetSoundPitch(sound, pitch);
            Raylib.SetSoundVolume(sound, volume);
            Raylib.PlaySound(sound);
        }
    }

    public static class Animations
    {
        public static Animation Blood;
        public static Animation Explosion;
        public static Animation HitMarker;
        public static Animation KillConfirmation;
        public static Animation Fire;
        public static void LoadAnimations(IAssetService assetService)
        {
            Blood = new Animation(assetService,"Resources/Sprites/GFX/death.png", 32, 5, 1, System.Drawing.Color.White, 0.05f); ;
            Explosion = new Animation(assetService,"Resources/Sprites/GFX/explosion.png", 32, 5, 4, System.Drawing.Color.White, 0.05f); ;
            HitMarker = new Animation(assetService,"Resources/Sprites/GFX/hitmarker.png", 32, 1, 1, System.Drawing.Color.White, 0.2f); ;
            KillConfirmation = new Animation(assetService,"Resources/Common/kill_confirmed.png", 64, 5, 1, System.Drawing.Color.White, 0.1f); ;
            Fire = new Animation(assetService,"Resources/Common/fire.png", 8, 4, 1, System.Drawing.Color.White, 0.02f);
        }
        public static void UnloadAnimations(IAssetService assetService)
        {
            assetService.UnloadTexture(Blood.Texture);
            assetService.UnloadTexture(Explosion.Texture);
            assetService.UnloadTexture(HitMarker.Texture);
            assetService.UnloadTexture(KillConfirmation.Texture);
            assetService.UnloadTexture(Fire.Texture);
        }
    }

    public static class HUD
    {

        public static ITextureAsset Arrow;
        public static ITextureAsset WideBarGreen;
        public static ITextureAsset WideBarRed;
        public static ITextureAsset WideBarEmpty;
        public static ITextureAsset BulletCounter;
        public static ITextureAsset HudBorder;
        public static ITextureAsset Kill;
        public static ITextureAsset Death;
        public static ITextureAsset ThinBarOrange;
        public static ITextureAsset ThinBarBlue;
        public static ITextureAsset ThinBarEmpty;
        public static ITextureAsset SelectionSquare;
        public static ITextureAsset KillFeedBackground;
        public static void LoadHud(IAssetService assetService)
        {
            WideBarGreen = assetService.LoadTexture("Resources/Common/wide_bar_green.png");
            WideBarRed = assetService.LoadTexture("Resources/Common/wide_bar_red.png");
            WideBarEmpty = assetService.LoadTexture("Resources/Common/wide_bar_empty.png");
            Arrow = assetService.LoadTexture("Resources/Common/arrow.png");
            BulletCounter = assetService.LoadTexture("Resources/Common/bullet.png");
            HudBorder = assetService.LoadTexture("Resources/Common/hud_border.png");
            Kill = assetService.LoadTexture("Resources/Common/kill.png");
            Death = assetService.LoadTexture("Resources/Common/death.png");
            ThinBarOrange = assetService.LoadTexture("Resources/Common/thin_bar_orange.png");
            ThinBarBlue = assetService.LoadTexture("Resources/Common/thin_bar_blue.png");
            ThinBarEmpty = assetService.LoadTexture("Resources/Common/thin_bar_empty.png");
            SelectionSquare = assetService.LoadTexture("Resources/Common/selection_square.png");
            KillFeedBackground = assetService.LoadTexture("Resources/Common/kill_feed_background.png");
        }
        public static void Unload(IAssetService assetService)
        {
            assetService.UnloadTexture(WideBarGreen);
            assetService.UnloadTexture(WideBarRed);
            assetService.UnloadTexture(WideBarEmpty);
            assetService.UnloadTexture(Arrow);
            assetService.UnloadTexture(BulletCounter);
            assetService.UnloadTexture(HudBorder);
            assetService.UnloadTexture(Kill);
            assetService.UnloadTexture(Death);
            assetService.UnloadTexture(ThinBarOrange);
            assetService.UnloadTexture(ThinBarBlue);
            assetService.UnloadTexture(ThinBarEmpty);
            assetService.UnloadTexture(SelectionSquare);
            assetService.UnloadTexture(KillFeedBackground);
        }
    }

}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
