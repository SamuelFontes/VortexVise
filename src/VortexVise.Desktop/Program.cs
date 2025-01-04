using VortexVise.Core.Enums;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Scenes;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;
using Steamworks;
using VortexVise.Desktop.Services;
using VortexVise.Core.Interfaces;
using VortexVise.Core.GameContext;
using System.Net;

// ...

try
{
    SteamClient.Init(480);
    foreach (var friend in SteamFriends.GetFriends())
    {
        Console.WriteLine($"{friend.Id}: {friend.Name}");
        Console.WriteLine($"{friend.IsOnline} / {friend.SteamLevel}");

    }
}
catch (System.Exception e)
{
    // Couldn't init for some reason (steam is closed etc)
}
SteamClient.Shutdown();

// Initialization
//---------------------------------------------------------
var context = new Context(new AssetService(), new CollisionService(), new InputService(), new RendererService(), new SoundService());
Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);                                               // Make game window resizeble
context.GameCore.GameScreenWidth = 960;
context.GameCore.GameScreenHeight = 540;
Raylib.InitWindow(context.GameCore.GameScreenWidth, context.GameCore.GameScreenHeight, "Vortex Vise");                  // Create game window
//Raylib.ToggleBorderlessWindowed();                                                                      // Make game fullscreen
Raylib.InitAudioDevice();                                                                               // Initialize audio device
Raylib.HideCursor();                                                                                    // Hide windows cursor
Raylib.SetTargetFPS(GameSettings.TargetFPS);                                                            // Set game target FPS
Raylib.SetExitKey(0);                                                                                   // Disable escape closing the game

// Load Assets
GameAssets.InitializeAssets(context.AssetService);                                                                          // Load global data 
SceneManager sceneManager = new SceneManager(context.InputService,context.GameCore, context.RendererService);

// Set Window Icon
Image icon = Raylib.LoadImage("Resources/Skins/afatso.png");                                            // Load icon at runtime
Raylib.SetWindowIcon(icon);                                                                             // Set icon
Raylib.UnloadImage(icon);                                                                               // Unload icon from memory

// Initiate music
GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance,context.GameCore);      // Play main menu song

// Setup and init first screen
sceneManager.CurrentScene = GameScene.MENU;

// Main Game Loop
//--------------------------------------------------------------------------------------
while (!(Raylib.WindowShouldClose() || context.GameCore.GameShouldClose))
{
    // UPDATE
    //----------------------------------------------------------------------------------

    // Read PC Keys
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
    {
        if (GameSettings.BorderlessFullScreen)
            Raylib.ToggleBorderlessWindowed();
        else
            Raylib.ToggleFullscreen();
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F7)) Utils.SwitchDebug();

    // Update music
    if (GameAssets.MusicAndAmbience.IsMusicPlaying) Raylib.UpdateMusicStream(GameAssets.MusicAndAmbience.Music);       // NOTE: Music keeps playing between screens

    // Update game
    sceneManager.UpdateScene(sceneManager,context.CollisionService,context.GameCore, context.RendererService);


    // Update user interface
    //----------------------------------------------------------------------------------
    GameUserInterface.UpdateUserInterface(context.GameCore);


    // DRAW
    //----------------------------------------------------------------------------------

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Raylib.BLACK);

    // Draw scene (gameplay or menu)
    sceneManager.DrawScene(context.RendererService,context.GameCore);

    // Draw full screen rectangle in front of everything when changing screens
    if (sceneManager.OnTransition) sceneManager.DrawTransition(context.GameCore);

    // Draw UI on top
    GameUserInterface.DrawUserInterface(context.RendererService,context.GameCore);

    Raylib.EndDrawing();
}

// Fade screen to black when exit
sceneManager.TransitionToNewScene(GameScene.UNKNOWN);
while (!sceneManager.TransitionFadeOut)
{
    sceneManager.UpdateTransition(context.GameCore, context.RendererService);
}

// De-Initialization
//--------------------------------------------------------------------------------------
GameAssets.UnloadAssets(context.AssetService);
Raylib.CloseAudioDevice();     // Close audio context
Raylib.CloseWindow();          // Close window and OpenGL context
