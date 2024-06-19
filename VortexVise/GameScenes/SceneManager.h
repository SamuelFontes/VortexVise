using VortexVise.Enums;
using VortexVise.GameGlobals;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

/// <summary>
/// This should handle the scene transitions and define what is the current scene on the game.
/// </summary>
static internal class SceneManager
{
    public static float TransitionAlpha { get; set; } = 0.0f;                   // Transition Alpha
    public static bool OnTransition { get; set; } = false;                      // Is scene transition happening
    public static bool TransitionFadeOut { get; set; } = false;                 // Is scene fading out
    public static GameScene TransitionFromScene { get; set; } = GameScene.UNKNOWN;// Last scene
    public static GameScene TransitionToScene { get; set; } = GameScene.UNKNOWN;// New scene
    public static GameScene CurrentScene { get; set; } = GameScene.LOGO;        // Defines what is the current scene

    public static void TransitionToNewScene(GameScene scene)
    {
        OnTransition = true;
        TransitionFadeOut = false;
        TransitionFromScene = CurrentScene;
        TransitionToScene = scene;
        TransitionAlpha = 0.0f;
    }

    // Update transition effect (fade-in, fade-out)
    public static void UpdateTransition()
    {
        if (!TransitionFadeOut)
        {
            TransitionAlpha += Raylib.GetFrameTime();

            // NOTE: Due to float internal representation, condition jumps on 1.0f instead of 1.05f
            // For that reason we compare against 1.01f, to avoid last frame loading stop
            if (TransitionAlpha > 1.01f)
            {
                TransitionAlpha = 1.0f;

                // Unload current screen
                switch (TransitionFromScene)
                {
                    //case LOGO: UnloadLogoScreen(); break;
                    //case TITLE: UnloadTitleScreen(); break;
                    //case OPTIONS: UnloadOptionsScreen(); break;
                    case GameScene.GAMEPLAY: GameplayScene.UnloadGameplayScene(); break;
                    case GameScene.MENU: MenuScene.UnloadMenuScene(); break;
                    //case ENDING: UnloadEndingScreen(); break;
                    default: break;
                }

                // Load next screen
                switch (TransitionToScene)
                {
                    //case LOGO: InitLogoScreen(); break;
                    //case TITLE: InitTitleScreen(); break;
                    case GameScene.GAMEPLAY: GameplayScene.InitGameplayScene(); break;
                    case GameScene.MENU: MenuScene.InitMenuScene(); break;
                    //case ENDING: InitEndingScreen(); break;
                    default: break;
                }

                CurrentScene = TransitionToScene;

                // Activate fade out effect to next loaded screen
                TransitionFadeOut = true;
            }
        }
        else  // Transition fade out logic
        {
            TransitionAlpha -= Raylib.GetFrameTime();

            if (TransitionAlpha < -0.01f)
            {
                TransitionAlpha = 0.0f;
                TransitionFadeOut = false;
                OnTransition = false;
                TransitionFromScene = GameScene.UNKNOWN;
                TransitionToScene = GameScene.UNKNOWN;
            }
        }
    }

    // Draw transition effect (full-screen rectangle)
    public static void DrawTransition()
    {
        Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, Raylib.Fade(Raylib.BLACK, TransitionAlpha));
    }

    public static void UpdateScene()
    {
        if (!OnTransition)
        {
            // Update
            //----------------------------------------------------------------------------------
            switch (CurrentScene)
            {
                case GameScene.GAMEPLAY:
                {
                    GameplayScene.UpdateGameplayScene();
                    if (GameplayScene.FinishGameplayScene() == 1) TransitionToNewScene(GameScene.MENU);
                    //else if (FinishGameplayScreen() == 2) TransitionToScreen(TITLE);

                }
                break;
                case GameScene.MENU:
                {
                    MenuScene.UpdateMenuScene();
                    if (MenuScene.FinishMenuScene() == 2) TransitionToNewScene(GameScene.GAMEPLAY);
                    else if (MenuScene.FinishMenuScene() == -1) TransitionToNewScene(GameScene.UNKNOWN);
                }
                break;
                default: break;
            }
        }
        else UpdateTransition();    // Update transition (fade-in, fade-out)
    }

    public static void DrawScene()
    {
        switch (CurrentScene)
        {
            case GameScene.GAMEPLAY: GameplayScene.DrawGameplayScene(); break;
            case GameScene.MENU: MenuScene.DrawMenuScene(); break;
            default: break;
        }
    }
}
