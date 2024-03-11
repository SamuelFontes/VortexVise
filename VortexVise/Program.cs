﻿/*******************************************************************************************
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
Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
Raylib.InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, "Vortex Vise");
Raylib.SetWindowMinSize(GameCore.GameScreenWidth, GameCore.GameScreenHeight);
RenderTexture2D gameRendering = Raylib.LoadRenderTexture(GameCore.GameScreenWidth, GameCore.GameScreenHeight); // Game will be rendered to this texture
Raylib.InitAudioDevice();      // Initialize audio device
Raylib.HideCursor();

// Load global data (assets that must be available in all scenes, i.e. font)
GameAudio.InitAudio();
Font font = Raylib.LoadFont("Resources/Common/moltorspunch.ttf");
Music music = Raylib.LoadMusicStream("Resources/Audio/Music/ambient.ogg");
Sound fxClick = Raylib.LoadSound("Resources/Audio/FX/click.wav");
Sound fxSelection = Raylib.LoadSound("Resources/Audio/FX/selection.wav");

// Initiate music
Raylib.SetMusicVolume(music, 1.0f);
Raylib.PlayMusicStream(music);

// Setup and init first screen
GameSceneManager.CurrentScene = GameScene.GAMEPLAY;
GameplayScene.InitGameplayScene();
GameUserInterface.InitUserInterface();

// Main Game Loop
//--------------------------------------------------------------------------------------
while (!(Raylib.WindowShouldClose() || GameCore.GameShouldClose))
{
    // Read PC Keys
    //----------------------------------------------------------------------------------
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
    {
        if (GameOptions.BorderlessFullScreen)
            Raylib.ToggleBorderlessWindowed();
        else
        {
            Raylib.ToggleFullscreen();
        }
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_F7)) Utils.SwitchDebug();

    // Update music
    //----------------------------------------------------------------------------------
    Raylib.UpdateMusicStream(music);       // NOTE: Music keeps playing between screens

    // Update scene
    //----------------------------------------------------------------------------------
    if (!GameSceneManager.TransitionFadeOut)
    {

        // Update
        //----------------------------------------------------------------------------------
        switch (GameSceneManager.CurrentScene)
        {
            case GameScene.GAMEPLAY:
                {
                    GameplayScene.UpdateGameplayScene();

                    //if (FinishGameplayScreen() == 1) TransitionToScreen(ENDING);
                    //else if (FinishGameplayScreen() == 2) TransitionToScreen(TITLE);

                }
                break;
            default: break;
        }
    }
    else GameSceneManager.UpdateTransition();    // Update transition (fade-in, fade-out)
    // Draw
    //----------------------------------------------------------------------------------

    // Update user interface
    //----------------------------------------------------------------------------------
    GameUserInterface.UpdateUserInterface();


    // Setup scalling
    float MIN(float a, float b) { return ((a) < (b) ? (a) : (b)); }
    GameCore.GameScreenScale = MIN((float)Raylib.GetScreenWidth() / GameCore.GameScreenWidth, (float)Raylib.GetScreenHeight() / GameCore.GameScreenHeight); // TODO: This should be calculated only on screen size change
    TextureFilter screenFiltering;
    if (GameOptions.IntegerScalling && GameCore.GameScreenScale == (int)GameCore.GameScreenScale)
    {
        screenFiltering = TextureFilter.TEXTURE_FILTER_POINT;
    }
    else
    {
        screenFiltering = TextureFilter.TEXTURE_FILTER_BILINEAR;
    }
    Raylib.SetTextureFilter(gameRendering.texture, screenFiltering);  // Texture scale filter to use

    Raylib.BeginTextureMode(gameRendering);


    Raylib.ClearBackground(WHITE);

    // Deal with resolution
    //----------------------------------------------------------------------------------

    switch (GameSceneManager.CurrentScene)
    {
        case GameScene.GAMEPLAY: GameplayScene.DrawGameplayScene(); break;
        default: break;
    }

    // Draw full screen rectangle in front of everything
    if (GameSceneManager.OnTransition) GameSceneManager.DrawTransition();

    GameUserInterface.DrawUserInterface();
    Raylib.DrawText(Utils.DebugText, 32, 32, 32, WHITE);
    Raylib.DrawFPS(500, 32);

    Raylib.EndTextureMode();
    Raylib.BeginDrawing();
    Raylib.ClearBackground(BLACK);     // Clear screen background

    // Draw render texture to screen, properly scaled
    if (GameOptions.MaintainAspectRatio)
    {
        Raylib.DrawTexturePro(gameRendering.texture, new(0.0f, 0.0f, gameRendering.texture.width, -gameRendering.texture.height), new(
            (Raylib.GetScreenWidth() - (GameCore.GameScreenWidth * GameCore.GameScreenScale)) * 0.5f, (Raylib.GetScreenHeight() - ((float)GameCore.GameScreenHeight * GameCore.GameScreenScale)) * 0.5f, (float)GameCore.GameScreenWidth * GameCore.GameScreenScale, (float)GameCore.GameScreenHeight * GameCore.GameScreenScale), new Vector2(0, 0), 0.0f, WHITE);
    }
    else
    {

        Raylib.DrawTexturePro(gameRendering.texture, new Rectangle(0.0f, 0.0f, (float)gameRendering.texture.width, (float)-gameRendering.texture.height), new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), new Vector2(0, 0), 0.0f, WHITE);

    }


    Raylib.EndDrawing();
    //----------------------------------------------------------------------------------
}

// Fade screen to black when exit
//--------------------------------------------------------------------------------------
GameSceneManager.TransitionToNewScene(GameScene.UNKNOWN);

// De-Initialization
//--------------------------------------------------------------------------------------
// Unload current screen data before closing
switch (GameSceneManager.CurrentScene)
{
    //case GameScene.LOGO: UnloadLogoScreen(); break;
    //case GameScene.TITLE: UnloadTitleScreen(); break;
    case GameScene.GAMEPLAY: GameplayScene.UnloadGameplayScene(); break;
    //case GameScene.ENDING: UnloadEndingScreen(); break;
    //case GameScene.OPTIONS: UnloadOptionsScreen(); break;
    default: break;
}
GameUserInterface.UnloadUserInterface();

// Unload global data loaded
Raylib.UnloadFont(font);
GameAudio.UnloadAudio();
//UnloadMusicStream(music);
Raylib.UnloadSound(fxClick);
Raylib.UnloadSound(fxSelection);

Raylib.CloseAudioDevice();     // Close audio context
Raylib.CloseWindow();          // Close window and OpenGL context

