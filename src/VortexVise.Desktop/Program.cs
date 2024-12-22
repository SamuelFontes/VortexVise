using VortexVise.Core.Enums;
using VortexVise.Desktop.GameGlobals;
using VortexVise.Desktop.Scenes;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;
using Steamworks;
using VortexVise.Desktop.Services;
using VortexVise.Core.Interfaces;

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
Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);                                               // Make game window resizeble
//Raylib.InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, "Vortex Vise");                  // Create game window
Raylib.InitWindow(960, 540, "Vortex Vise");                  // Create game window
//Raylib.ToggleBorderlessWindowed();                                                                      // Make game fullscreen
Raylib.InitAudioDevice();                                                                               // Initialize audio device
Raylib.HideCursor();                                                                                    // Hide windows cursor
Raylib.SetTargetFPS(GameSettings.TargetFPS);                                                            // Set game target FPS
Raylib.SetExitKey(0);                                                                                   // Disable escape closing the game

// Initialize services
InputService inputService = new InputService();

// Load Assets
var assetService = new AssetService();
var rendererService = new RendererService();
var collisionService = new CollisionService();
GameAssets.InitializeAssets(assetService);                                                                          // Load global data 
SceneManager sceneManager = new SceneManager(inputService);

// Set Window Icon
Image icon = Raylib.LoadImage("Resources/Skins/afatso.png");                                            // Load icon at runtime
Raylib.SetWindowIcon(icon);                                                                             // Set icon
Raylib.UnloadImage(icon);                                                                               // Unload icon from memory

// Initiate music
GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance);      // Play main menu song

// Setup and init first screen
sceneManager.CurrentScene = GameScene.MENU;

// Main Game Loop
//--------------------------------------------------------------------------------------
while (!(Raylib.WindowShouldClose() || GameCore.GameShouldClose))
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
    sceneManager.UpdateScene(sceneManager,collisionService);


    // Update user interface
    //----------------------------------------------------------------------------------
    GameUserInterface.UpdateUserInterface();


    // DRAW
    //----------------------------------------------------------------------------------

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Raylib.BLACK);

    // Draw scene (gameplay or menu)
    sceneManager.DrawScene(rendererService);

    // Draw full screen rectangle in front of everything when changing screens
    if (sceneManager.OnTransition) sceneManager.DrawTransition();

    // Draw UI on top
    GameUserInterface.DrawUserInterface();

    Raylib.EndDrawing();
}

// Fade screen to black when exit
sceneManager.TransitionToNewScene(GameScene.UNKNOWN);
while (!sceneManager.TransitionFadeOut)
{
    sceneManager.UpdateTransition();
}

// De-Initialization
//--------------------------------------------------------------------------------------
GameAssets.UnloadAssets(assetService);
Raylib.CloseAudioDevice();     // Close audio context
Raylib.CloseWindow();          // Close window and OpenGL context
