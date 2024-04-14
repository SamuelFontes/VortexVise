
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

        // Sounds
        //---------------------------------------------------------
        Sounds.HookShoot = Raylib.LoadSound("Resources/Audio/FX/hook_fire.wav");
        Sounds.HookHit = Raylib.LoadSound("Resources/Audio/FX/hook_hit.wav");
        Sounds.Jump = Raylib.LoadSound("Resources/Audio/FX/jump.wav");
        Sounds.Dash = Raylib.LoadSound("Resources/Audio/FX/dash.wav");
        Sounds.Click = Raylib.LoadSound("Resources/Audio/FX/click.wav");
        Sounds.Selection = Raylib.LoadSound("Resources/Audio/FX/selection.wav");
        Sounds.WeaponDrop = Raylib.LoadSound("Resources/Audio/FX/metal_drop.wav");
        Sounds.WeaponClick = Raylib.LoadSound("Resources/Audio/FX/weapon_click.wav");
        Sounds.Death = Raylib.LoadSound("Resources/Audio/FX/death3.ogg");
        Sounds.Shotgun = Raylib.LoadSound("Resources/Audio/FX/shotgun.ogg");
        Sounds.Drop = Raylib.LoadSound("Resources/Audio/FX/Drop.ogg");

        // Music And Ambience
        //---------------------------------------------------------

        // Gameplay
        //---------------------------------------------------------
        MapLogic.Init();
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

    }

    /// <summary>
    /// Unload all game assets before game closes.
    /// </summary>
    public static void UnloadAssets()
    {
        // Misc
        //---------------------------------------------------------
        Raylib.UnloadFont(Misc.Font);

        // Sounds
        //---------------------------------------------------------
        Raylib.UnloadSound(Sounds.HookShoot);
        Raylib.UnloadSound(Sounds.HookHit);
        Raylib.UnloadSound(Sounds.Jump);
        Raylib.UnloadSound(Sounds.Dash);
        Raylib.UnloadSound(Sounds.Click);
        Raylib.UnloadSound(Sounds.Selection);
        Raylib.UnloadSound(Sounds.WeaponDrop);
        Raylib.UnloadSound(Sounds.WeaponClick);
        Raylib.UnloadSound(Sounds.Death);
        Raylib.UnloadSound(Sounds.Shotgun);
        Raylib.UnloadSound(Sounds.Drop);

        // Music And Ambience
        //---------------------------------------------------------
        if (MusicAndAmbience.IsMusicPlaying) Raylib.UnloadMusicStream(MusicAndAmbience.Music);

        // Gameplay
        //---------------------------------------------------------
        MapLogic.Unload();
        foreach (var skin in Gameplay.Skins) Raylib.UnloadTexture(skin.Texture);
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
            if (IsMusicPlaying) Raylib.UnloadMusicStream(Music);
            Music = Raylib.LoadMusicStream(music);
            Raylib.PlayMusicStream(Music);
        }

        public static void PlayCustomMusic(string musicName)
        {
            if (IsMusicPlaying) Raylib.UnloadMusicStream(Music);
            Music = Raylib.LoadMusicStream($"Resources/Audio/Music/{musicName}.mp3");
            Raylib.PlayMusicStream(Music);
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
        public static Animation Explosion = new Animation("Resources/Sprites/GFX/explosion.png", 32, 5);
    }
}
