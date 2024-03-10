/*******************************************************************************************
*
*   Vortex Vise
*
*   A nice game about killing things
*
********************************************************************************************/

using Raylib_cs;
using System.Numerics;
using VortexVise;

// Initialization
//---------------------------------------------------------
Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
Raylib.InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, "Vortex Vise");
Raylib.SetWindowMinSize(GameCore.GameScreenWidth, GameCore.GameScreenHeight);
RenderTexture2D gameRendering = Raylib.LoadRenderTexture(GameCore.GameScreenWidth, GameCore.GameScreenHeight); // Game will be rendered to this texture
Raylib.InitAudioDevice();      // Initialize audio device
Raylib.HideCursor();

// Load global data (assets that must be available in all screens, i.e. font)
Font font = Raylib.LoadFont("Resources/Common/moltorspunch.ttf");
Music music = Raylib.LoadMusicStream("Resources/Audio/Music/ambient.ogg");
Sound fxClick = Raylib.LoadSound("Resources/Audio/FX/click.wav");
Sound fxSelection = Raylib.LoadSound("Resources/Audio/FX/selection.wav");

Raylib.SetMusicVolume(music, 1.0f);
Raylib.PlayMusicStream(music);

// Setup and init first screen
GameCore.CurrentScene = GameScene.GAMEPLAY;
GameplayScene.InitGameplayScene();
UserInterface.InitUserInterface();

// Main Game Loop
//--------------------------------------------------------------------------------------
while (!(Raylib.WindowShouldClose() || GameCore.GameShouldClose))
{
    // Read PC Keys
    //----------------------------------------------------------------------------------
    if (Raylib.IsKeyPressed(KeyboardKey.F11)) Raylib.ToggleBorderlessWindowed();
    if (Raylib.IsKeyPressed(KeyboardKey.F7)) Utils.SwitchDebug();

    // Update user interface
    //----------------------------------------------------------------------------------
    UserInterface.UnloadUserInterface();
    Raylib.UpdateMusicStream(music);       // NOTE: Music keeps playing between screens

    // Update scene
    //----------------------------------------------------------------------------------
    if (!GameCore.TransitionFadeOut)
    {

        // Update
        //----------------------------------------------------------------------------------
        switch (GameCore.CurrentScene)
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
    else GameCore.UpdateTransition();    // Update transition (fade-in, fade-out)
    // Draw
    //----------------------------------------------------------------------------------

    float MIN(float a, float b)
    {
        return ((a) < (b) ? (a) : (b));
    }
    // Setup scalling
    GameCore.GameScreenScale = MIN((float)Raylib.GetScreenWidth() / GameCore.GameScreenWidth, (float)Raylib.GetScreenHeight() / GameCore.GameScreenHeight); // TODO: This should be calculated only on screen size change
    /*    if (integerScalling)
        {
            scale = (int)scale;
            screenFiltering = TEXTURE_FILTER_POINT;
        }
        else
        {
            screenFiltering = TEXTURE_FILTER_BILINEAR;
        }
        SetTextureFilter(target.texture, screenFiltering);  // Texture scale filter to use
    */
    Raylib.BeginTextureMode(gameRendering);


    Raylib.ClearBackground(Color.RayWhite);

    // Deal with resolution
    //----------------------------------------------------------------------------------

    switch (GameCore.CurrentScene)
    {
        case GameScene.GAMEPLAY: GameplayScene.DrawGameplayScene(); break;
        default: break;
    }

    // Draw full screen rectangle in front of everything
    if (GameCore.OnTransition) GameCore.DrawTransition();

    UserInterface.DrawUserInterface();

    Raylib.EndTextureMode();
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);     // Clear screen background

    // Draw render texture to screen, properly scaled
    /*    if (maintainAspectRation)
        {
            DrawTexturePro(target.texture, (Rectangle){ 0.0f, 0.0f, (float)target.texture.width, (float)-target.texture.height },
                               (Rectangle){
                (GetScreenWidth() - ((float)gameScreenWidth * scale)) * 0.5f, (GetScreenHeight() - ((float)gameScreenHeight * scale)) * 0.5f,
                               (float)gameScreenWidth * scale, (float)gameScreenHeight * scale }, (Vector2){ 0, 0 }, 0.0f, WHITE);
        }
        else
        {
    */
    Raylib.DrawTexturePro(gameRendering.Texture, new Rectangle(0.0f, 0.0f, (float)gameRendering.Texture.Width, (float)-gameRendering.Texture.Height), new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), new Vector2(0, 0), 0.0f, Color.White);

    //}

    Raylib.EndDrawing();
    //----------------------------------------------------------------------------------
}

// Fade screen to black when exit
//--------------------------------------------------------------------------------------
GameCore.TransitionToNewScene(GameScene.UNKNOWN);

// De-Initialization
//--------------------------------------------------------------------------------------
// Unload current screen data before closing
switch (GameCore.CurrentScene)
{
    //case GameScene.LOGO: UnloadLogoScreen(); break;
    //case GameScene.TITLE: UnloadTitleScreen(); break;
    case GameScene.GAMEPLAY: GameplayScene.UnloadGameplayScene(); break;
    //case GameScene.ENDING: UnloadEndingScreen(); break;
    //case GameScene.OPTIONS: UnloadOptionsScreen(); break;
    default: break;
}
UserInterface.UnloadUserInterface();

// Unload global data loaded
Raylib.UnloadFont(font);
//UnloadMusicStream(music);
Raylib.UnloadSound(fxClick);
Raylib.UnloadSound(fxSelection);

Raylib.CloseAudioDevice();     // Close audio context
Raylib.CloseWindow();          // Close window and OpenGL context

