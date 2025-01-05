﻿using VortexVise.Core.Enums;
using VortexVise.Core.Extensions;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.GameContext;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Scenes;

/// <summary>
/// This should handle the scene transitions and define what is the current scene on the game.
/// </summary>
public class SceneManager
{
    private MenuScene MenuScene { get; set; }
    private GameplayScene GameplayScene { get; set; }
    public float TransitionAlpha { get; set; } = 0.0f;                   // Transition Alpha
    public bool OnTransition { get; set; } = false;                      // Is scene transition happening
    public bool TransitionFadeOut { get; set; } = false;                 // Is scene fading out
    public GameScene TransitionFromScene { get; set; } = GameScene.UNKNOWN;// Last scene
    public GameScene TransitionToScene { get; set; } = GameScene.UNKNOWN;// New scene
    public GameScene CurrentScene { get; set; } = GameScene.LOGO;        // Defines what is the current scene
    private readonly IInputService _inputService;

    public SceneManager(IInputService inputService, GameCore gameCore, IRendererService rendererService, ICollisionService collisionService)
    {
        _inputService = inputService;
        MenuScene = new MenuScene(_inputService, this, gameCore, rendererService, collisionService);
        GameplayScene = new GameplayScene(_inputService);
    }

    public void TransitionToNewScene(GameScene scene)
    {
        OnTransition = true;
        TransitionFadeOut = false;
        TransitionFromScene = CurrentScene;
        TransitionToScene = scene;
        TransitionAlpha = 0.0f;
    }

    // Update transition effect (fade-in, fade-out)
    public void UpdateTransition(GameCore gameCore, IRendererService rendererService, IAssetService assetService, ICollisionService collisionService)
    {
        if (!TransitionFadeOut)
        {
            TransitionAlpha += rendererService.GetFrameTime();

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
                    case GameScene.GAMEPLAY: GameplayScene.UnloadGameplayScene(assetService); break;
                    case GameScene.MENU: MenuScene.UnloadMenuScene(); break;
                    //case ENDING: UnloadEndingScreen(); break;
                    default: break;
                }

                // Load next screen
                switch (TransitionToScene)
                {
                    //case LOGO: InitLogoScreen(); break;
                    //case TITLE: InitTitleScreen(); break;
                    case GameScene.GAMEPLAY: GameplayScene.InitGameplayScene(gameCore, assetService); break;
                    case GameScene.MENU: MenuScene = new MenuScene(_inputService, this, gameCore, rendererService, collisionService); break;
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
            TransitionAlpha -= rendererService.GetFrameTime();

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
    public void DrawTransition(GameCore gameCore, IRendererService rendererService)
    {
        rendererService.DrawRectangleRec(new System.Drawing.Rectangle(0, 0, gameCore.GameScreenWidth, gameCore.GameScreenHeight), System.Drawing.Color.Black.Fade(TransitionAlpha));
    }

    public void UpdateScene(SceneManager sceneManager, ICollisionService collisionService, GameCore gameCore, IRendererService rendererService, IAssetService assetService)
    {
        if (!OnTransition)
        {
            // Update
            //----------------------------------------------------------------------------------
            switch (CurrentScene)
            {
                case GameScene.GAMEPLAY:
                    {
                        GameplayScene.UpdateGameplayScene(sceneManager, collisionService, gameCore);
                        if (GameplayScene.FinishGameplayScene() == 1) TransitionToNewScene(GameScene.MENU);
                        //else if (FinishGameplayScreen() == 2) TransitionToScreen(TITLE);

                    }
                    break;
                case GameScene.MENU:
                    {
                        MenuScene.UpdateMenuScene(gameCore, rendererService, collisionService);
                        if (MenuScene.FinishMenuScene() == 2) TransitionToNewScene(GameScene.GAMEPLAY);
                        else if (MenuScene.FinishMenuScene() == -1) TransitionToNewScene(GameScene.UNKNOWN);
                    }
                    break;
                default: break;
            }
        }
        else UpdateTransition(gameCore, rendererService, assetService, collisionService);    // Update transition (fade-in, fade-out)
    }

    public void DrawScene(IRendererService rendererService, GameCore gameCore, ICollisionService collisionService)
    {
        switch (CurrentScene)
        {
            case GameScene.GAMEPLAY: GameplayScene.DrawGameplayScene(rendererService, gameCore, collisionService); break;
            case GameScene.MENU: MenuScene.DrawMenuScene(rendererService, gameCore); break;
            default: break;
        }
    }
}
