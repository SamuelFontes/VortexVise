using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Scenes;
using VortexVise.Core.Services;

namespace VortexVise.Core
{
    public class Game
    {
        private GameServices _services;
        private SceneManager sceneManager; 
        public Game(GameServices services)
        {
            _services = services;
        }

        public void Init<TFontAsset, TMusicAsset, TSoundAsset, TTextureAsset>()
            where TFontAsset : IFontAsset, new()
            where TMusicAsset : IMusicAsset, new()
            where TSoundAsset : ISoundAsset, new()
            where TTextureAsset : ITextureAsset, new()
        {
            // Initialization
            _services.WindowService.InitializeWindow();

            // Load Assets
            GameAssets.InitializeAssets<TFontAsset, TMusicAsset, TSoundAsset, TTextureAsset>(_services.AssetService);                                                                          // Load global data 
            GameUserInterface.InitUserInterface<TTextureAsset>();
            sceneManager = new SceneManager(_services.InputService, _services.RendererService, _services.CollisionService);

            // Initiate music
            GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance);      // Play main menu song

            // Setup and init first screen
            sceneManager.CurrentScene = GameScene.MENU;


        }

        public void Update<TPlayerCamera>() where TPlayerCamera: IPlayerCamera, new()
        {
            // UPDATE
            //----------------------------------------------------------------------------------
            Console.WriteLine(" window events");
            _services.WindowService.HandleWindowEvents();

            // Update music
            Console.WriteLine("update music");
            if (GameAssets.MusicAndAmbience.Music.IsPlaying) GameAssets.MusicAndAmbience.Music.Update();       // NOTE: Music keeps playing between screens

            Console.WriteLine("update game");
            // Update game
            sceneManager.UpdateScene<TPlayerCamera>(sceneManager, _services.CollisionService, _services.RendererService, _services.AssetService, _services.InputService);

            // Update user interface
            GameUserInterface.UpdateUserInterface(_services.RendererService);

            // DRAW
            //----------------------------------------------------------------------------------

            _services.RendererService.BeginDrawing();
            _services.RendererService.ClearBackground(Color.Black);

            // Draw scene (gameplay or menu)
            sceneManager.DrawScene(_services.RendererService, _services.CollisionService);

            // Draw full screen rectangle in front of everything when changing screens
            if (sceneManager.OnTransition) sceneManager.DrawTransition(_services.RendererService);

            // Draw UI on top
            GameUserInterface.DrawUserInterface(_services.RendererService);

            _services.RendererService.EndDrawing();
        }

        public void Unload()
        {
            GameAssets.UnloadAssets(_services.AssetService);
        }
    }
}
