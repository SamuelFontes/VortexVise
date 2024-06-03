/*******************************************************************************************
*
*   Vortex Vise
*
*   A nice game about killing things
*
********************************************************************************************/

using System.Numerics;
using VortexVise.Enums;
using VortexVise.GameGlobals;
using VortexVise.Scenes;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;


// Initialization
//---------------------------------------------------------
Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);                                               // Make game window resizeble
//var monitorHeight = Raylib.GetMonitorHeight(0);
//var monitorWidth = Raylib.GetMonitorWidth(0);
/*if (false)// Turn on to compile to steam deck
{
    GameCore.GameScreenHeight = 400;
    GameCore.GameScreenWidth = 640;
    GameCore.MenuFontSize = 20;
}
*/
Raylib.InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, "Vortex Vise");                  // Create game window
Raylib.SetWindowMinSize(GameCore.GameScreenWidth, GameCore.GameScreenHeight);                           // Set minimal window size
Raylib.InitAudioDevice();                                                                               // Initialize audio device
Raylib.HideCursor();                                                                                    // Hide windows cursor
Raylib.SetTargetFPS(GameSettings.TargetFPS);                                                            // Set game target FPS
Raylib.SetExitKey(0);                                                                                   // Disable escape closing the game
GameAssets.InitializeAssets();                                                                          // Load global data 
GameCore.GameRendering = Raylib.LoadRenderTexture(GameCore.GameScreenWidth, GameCore.GameScreenHeight); // Game will be rendered to this texture
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
    // Read PC Keys
    //----------------------------------------------------------------------------------
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
    {
        if (GameSettings.BorderlessFullScreen)
            Raylib.ToggleBorderlessWindowed();
        else
            Raylib.ToggleFullscreen();
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F7)) Utils.SwitchDebug();

    // Update music
    //----------------------------------------------------------------------------------
    if (GameAssets.MusicAndAmbience.IsMusicPlaying) Raylib.UpdateMusicStream(GameAssets.MusicAndAmbience.Music);       // NOTE: Music keeps playing between screens

    // Update game
    //----------------------------------------------------------------------------------
    SceneManager.UpdateScene();


    // Update user interface
    //----------------------------------------------------------------------------------
    GameUserInterface.UpdateUserInterface();


    // Deal with resolution
    //----------------------------------------------------------------------------------
    // Setup scalling
    GameCore.GameScreenScale = Utils.MIN((float)Raylib.GetScreenWidth() / GameCore.GameScreenWidth, (float)Raylib.GetScreenHeight() / GameCore.GameScreenHeight); // TODO: This should be calculated only on screen size change
    TextureFilter screenFiltering;
    if (GameSettings.IntegerScalling && GameCore.GameScreenScale == (int)GameCore.GameScreenScale)
    {
        screenFiltering = TextureFilter.TEXTURE_FILTER_POINT;
    }
    else
    {
        screenFiltering = TextureFilter.TEXTURE_FILTER_BILINEAR;
    }
    Raylib.SetTextureFilter(GameCore.GameRendering.texture, screenFiltering);  // Texture scale filter to use

    Raylib.BeginTextureMode(GameCore.GameRendering);


    Raylib.ClearBackground(Raylib.BLACK);

    // Draw Game
    //----------------------------------------------------------------------------------
    SceneManager.DrawScene();

    // Draw full screen rectangle in front of everything
    if (SceneManager.OnTransition) SceneManager.DrawTransition();

    GameUserInterface.DrawUserInterface();

    Raylib.EndTextureMode();
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Raylib.BLACK);     // Clear screen background

    // Draw render texture to screen, properly scaled
    if (GameSettings.MaintainAspectRatio)
    {
        Raylib.DrawTexturePro(GameCore.GameRendering.texture, new(0.0f, 0.0f, GameCore.GameRendering.texture.width, -GameCore.GameRendering.texture.height), new(
            (Raylib.GetScreenWidth() - (GameCore.GameScreenWidth * GameCore.GameScreenScale)) * 0.5f, (Raylib.GetScreenHeight() - ((float)GameCore.GameScreenHeight * GameCore.GameScreenScale)) * 0.5f, (float)GameCore.GameScreenWidth * GameCore.GameScreenScale, (float)GameCore.GameScreenHeight * GameCore.GameScreenScale), new Vector2(0, 0), 0.0f, Raylib.WHITE);
    }
    else
    {

        Raylib.DrawTexturePro(GameCore.GameRendering.texture, new Rectangle(0.0f, 0.0f, (float)GameCore.GameRendering.texture.width, (float)-GameCore.GameRendering.texture.height), new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), new Vector2(0, 0), 0.0f, Raylib.WHITE);

    }


    Raylib.EndDrawing();
    //----------------------------------------------------------------------------------
}

// Fade screen to black when exit
//--------------------------------------------------------------------------------------
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

