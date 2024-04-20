
using VortexVise.Logic;
using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

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
    public static void InitializeAssets()
    {
        // Misc
        //---------------------------------------------------------
        Misc.Font = Raylib.LoadFont("Resources/Common/DeltaBlock.ttf");
        HUD.Load();

        // Sounds
        //---------------------------------------------------------
        Sounds.Init();

        // Music And Ambience
        //---------------------------------------------------------

        // Gameplay
        //---------------------------------------------------------
        MapLogic.Init();
        WeaponLogic.Init();
        // Load skins
        string[] skins = Directory.GetFiles("Resources/Skins", "*.png", SearchOption.TopDirectoryOnly);
        var counter = 0;
        foreach (string skin in skins)
        {
            var s = new Skin();
            s.Id = counter++;
            s.TextureLocation = skin;
            s.Texture = Raylib.LoadTexture(skin);
            Gameplay.Skins.Add(s);
        }
        if (Gameplay.Skins.Count == 0) throw new Exception("Couldn't load any player skin");

        // Animation
        //---------------------------------------------------------
        Animations.LoadAnimations();

    }

    /// <summary>
    /// Unload all game assets before game closes.
    /// </summary>
    public static void UnloadAssets()
    {
        // Misc
        //---------------------------------------------------------
        Raylib.UnloadFont(Misc.Font);
        HUD.Unload();

        // Sounds
        //---------------------------------------------------------
        Sounds.Unload();

        // Music And Ambience
        //---------------------------------------------------------
        if (MusicAndAmbience.IsMusicPlaying) Raylib.UnloadMusicStream(MusicAndAmbience.Music);

        // Gameplay
        //---------------------------------------------------------
        MapLogic.Unload();
        foreach (var skin in Gameplay.Skins) Raylib.UnloadTexture(skin.Texture);

        // Animation
        //---------------------------------------------------------
        Animations.UnloadAnimations();
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
        public static List<Map> Maps { get; set; } = [];
        public static Texture CurrentMapTexture; // This is the whole map baked into an image
        public static Texture HookTexture;
        public static List<Weapon> Weapons { get; set; } = [];
        public static List<Skin> Skins { get; set; } = new List<Skin>();
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

        public static void PlayMusic(string music)
        {
            if (IsMusicPlaying) StopMusic();
            Music = Raylib.LoadMusicStream(music);
            Raylib.PlayMusicStream(Music);
            IsMusicPlaying = true;
            Raylib.SetMusicVolume(Music, 0.8f);
        }

        public static void StopMusic()
        {
            if (!IsMusicPlaying) return;
            Sounds.PlaySound(Sounds.VinylScratch, pitch: 0.7f);
            Raylib.StopMusicStream(Music);
            Raylib.UnloadMusicStream(Music);
            IsMusicPlaying = false;
        }

        public static void PlayCustomMusic(string musicName)
        {
            if (IsMusicPlaying) StopMusic();
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
        }
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
        public static void PlaySound(Sound sound, float pan = 0.5f, float pitch = 1f, float volume = 1f)
        {
            if (GameCore.IsServer) return; // Audio don't play on the server

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
        public static void LoadAnimations()
        {
            Blood = new Animation("Resources/Sprites/GFX/death.png", 32, 5, 1, Raylib.WHITE, 0.05f); ;
            Explosion = new Animation("Resources/Sprites/GFX/explosion.png", 32, 5, 4, Raylib.WHITE, 0.05f); ;
            HitMarker = new Animation("Resources/Sprites/GFX/hitmarker.png", 32, 1, 1, Raylib.WHITE, 0.2f); ;
        }
        public static void UnloadAnimations()
        {
            Raylib.UnloadTexture(Blood.Texture);
            Raylib.UnloadTexture(Explosion.Texture);
            Raylib.UnloadTexture(HitMarker.Texture);
        }
    }

    public static class HUD
    {
        public static void Load()
        {
            WeaponOn = Raylib.LoadTexture("resources/Common/hud_weapon_on.png");
            WeaponOff = Raylib.LoadTexture("resources/Common/hud_weapon_off.png");
            Arrow = Raylib.LoadTexture("Resources/Common/arrow.png");
            BulletCounter = Raylib.LoadTexture("Resources/Common/bullet_counter.png");
            HudBorder = Raylib.LoadTexture("Resources/Common/hud_border.png");
        }
        public static void Unload()
        {
            Raylib.UnloadTexture(WeaponOn);
            Raylib.UnloadTexture(WeaponOff);
            Raylib.UnloadTexture(Arrow);
            Raylib.UnloadTexture(BulletCounter);
            Raylib.UnloadTexture(HudBorder);
        }

        public static Texture Arrow;
        public static Texture WeaponOn;
        public static Texture WeaponOff;
        public static Texture BulletCounter;
        public static Texture HudBorder;
    }
}
