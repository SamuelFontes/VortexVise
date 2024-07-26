using VortexVise.Enums;
using VortexVise.GameGlobals;
using VortexVise.Scenes;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

// Initialization
//---------------------------------------------------------
Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);                                               // Make game window resizeble
Raylib.InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, "Vortex Vise");                  // Create game window
Raylib.ToggleBorderlessWindowed();                                                                      // Make game fullscreen
Raylib.InitAudioDevice();                                                                               // Initialize audio device
Raylib.HideCursor();                                                                                    // Hide windows cursor
Raylib.SetTargetFPS(GameSettings.TargetFPS);                                                            // Set game target FPS
Raylib.SetExitKey(0);                                                                                   // Disable escape closing the game

// Load Assets
GameAssets.InitializeAssets();                                                                          // Load global data 

// Set Window Icon
Image icon = Raylib.LoadImage("Resources/Skins/afatso.png");                                            // Load icon at runtime
Raylib.SetWindowIcon(icon);                                                                             // Set icon
Raylib.UnloadImage(icon);                                                                               // Unload icon from memory

// Initiate music
GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance);      // Play main menu song

// Setup and init first screen
SceneManager.CurrentScene = GameScene.MENU;
MenuScene.InitMenuScene();

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
    SceneManager.UpdateScene();


    // Update user interface
    //----------------------------------------------------------------------------------
    GameUserInterface.UpdateUserInterface();


    // DRAW
    //----------------------------------------------------------------------------------

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Raylib.BLACK);

    // Draw scene (gameplay or menu)
    SceneManager.DrawScene();

    // Draw full screen rectangle in front of everything when changing screens
    if (SceneManager.OnTransition) SceneManager.DrawTransition();

    // Draw UI on top
    GameUserInterface.DrawUserInterface();

    Raylib.EndDrawing();
}

// Fade screen to black when exit
SceneManager.TransitionToNewScene(GameScene.UNKNOWN);
while (!SceneManager.TransitionFadeOut)
{
    SceneManager.UpdateTransition();
}

// De-Initialization
//--------------------------------------------------------------------------------------
GameAssets.UnloadAssets();
Raylib.CloseAudioDevice();     // Close audio context
Raylib.CloseWindow();          // Close window and OpenGL context
