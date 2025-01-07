using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.GameLogic;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Scenes;
using VortexVise.Core.Services;

namespace VortexVise.Core
{
    public class Game
    {
        private GameServices services;
        private SceneManager sceneManager; 
        public Game(GameServices services)
        {
            this.services = services;
        }

        public void Init<TFontAsset, TMusicAsset, TSoundAsset, TTextureAsset>()
            where TFontAsset : IFontAsset, new()
            where TMusicAsset : IMusicAsset, new()
            where TSoundAsset : ISoundAsset, new()
            where TTextureAsset : ITextureAsset, new()
        {
            // Initialization
            services.WindowService.InitializeWindow();

            // Load Assets
            GameAssets.InitializeAssets<TFontAsset, TMusicAsset, TSoundAsset, TTextureAsset>(services.AssetService);                                                                          // Load global data 
            GameUserInterface.InitUserInterface<TTextureAsset>();

            sceneManager = new SceneManager(services.InputService, services.RendererService, services.CollisionService);

            // Initiate music
            GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance);      // Play main menu song

            // Setup and init first screen
            sceneManager.CurrentScene = GameScene.Menu;


        }

        public void Update<TPlayerCamera>() where TPlayerCamera: IPlayerCamera, new()
        {
            // UPDATE
            //----------------------------------------------------------------------------------
            services.WindowService.HandleWindowEvents();

            // Get window size every frame
            int newWidth = services.WindowService.GetScreenWidth();
            int newHeight = services.WindowService.GetScreenHeight();
            if(newWidth != GameCore.GameScreenWidth || newHeight != GameCore.GameScreenHeight)
            {
                GameCore.GameScreenWidth = newWidth;
                GameCore.GameScreenHeight = newHeight;
                GameCore.ResolutionUpdate = true;
            }
            else
            {
                GameCore.ResolutionUpdate = false;
            }

            // Update music
            if (GameAssets.MusicAndAmbience.Music.IsPlaying) GameAssets.MusicAndAmbience.Music.Update();       // NOTE: Music keeps playing between screens

            // Update game
            sceneManager.UpdateScene<TPlayerCamera>(sceneManager, services.CollisionService, services.RendererService, services.AssetService, services.InputService);

            // Update user interface
            GameUserInterface.UpdateUserInterface(services);

            // DRAW
            //----------------------------------------------------------------------------------

            services.RendererService.BeginDrawing();
            services.RendererService.ClearBackground(Color.Black);

            // Draw scene (gameplay or menu)
            sceneManager.DrawScene(services.RendererService, services.CollisionService);

            // Draw full screen rectangle in front of everything when changing screens
            if (sceneManager.OnTransition) sceneManager.DrawTransition(services.RendererService);

            // Draw UI on top
            GameUserInterface.DrawUserInterface(services.RendererService);

            services.RendererService.EndDrawing();
        }

        public void Unload()
        {
            GameAssets.UnloadAssets(services.AssetService);
        }
    }
}
