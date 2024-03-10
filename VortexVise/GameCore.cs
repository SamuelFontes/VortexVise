using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise;


public static class GameCore
{
    public static int GameTickRate { get; private set; } = 64;                  // Defines tickrate the game and server runs at
    public static int GameScreenWidth { get; private set; } = 960;              // Defines internal game resolution
    public static int GameScreenHeight { get; private set; } = 540;             // Defines internal game resolution
    public static float GameScreenScale { get; set; }                           // How much the screen is scalling 
    public static GameScene CurrentScene { get; set; } = GameScene.LOGO;        // Defines what is the current scene
    public static bool GameShouldClose { get; set; } = false;                   // If true the game will close 
    public static float TransitionAlpha { get; set; } = 0.0f;                   // Transition Alpha
    public static bool OnTransition { get; set; } = false;                      // Is scene transition happening
    public static bool TransitionFadeOut { get; set; } = false;                 // Is scene fading out
    public static GameScene TransitionFromScene { get; set; } = GameScene.UNKNOWN;// Last scene
    public static GameScene TransitionToScene { get; set; } = GameScene.UNKNOWN;// New scene



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
            TransitionAlpha += 5 * Raylib.GetFrameTime();

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
                    //case ENDING: UnloadEndingScreen(); break;
                    default: break;
                }

                // Load next screen
                switch (TransitionToScene)
                {
                    //case LOGO: InitLogoScreen(); break;
                    //case TITLE: InitTitleScreen(); break;
                    case GameScene.GAMEPLAY: GameplayScene.InitGameplayScene(); break;
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
            TransitionAlpha -= 2 * Raylib.GetFrameTime();

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
        Raylib.DrawRectangle(0, 0, GameScreenWidth, GameScreenHeight, Raylib.Fade(Color.Black, TransitionAlpha));
    }



}
