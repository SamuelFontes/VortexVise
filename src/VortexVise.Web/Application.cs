using Raylib_cs;
using System;
using System.Runtime.InteropServices.JavaScript;
using VortexVise.Core;
using VortexVise.Core.Services;
using VortexVise.Web.Models;
using VortexVise.Web.Services;

namespace RaylibWasm
{
    public partial class Application
    {
        private static Texture2D logo;
        private static Game Game;

        /// <summary>
        /// Application entry point
        /// </summary>
        public static void Main()
        {
            // Run game 
            GameServices services = new GameServices(new AssetService(), new CollisionService(), new InputService(), new RendererService(), new SoundService(), new WindowService());
            Game = new Game(services);
            Game.Init<FontAsset, MusicAsset, SoundAsset, TextureAsset>();

            //game.Unload();
        }

        /// <summary>
        /// Updates frame
        /// </summary>
        [JSExport]
        public static void UpdateFrame()
        {
            Console.WriteLine("RUN FRAME");
            Game.Update<PlayerCamera>();
        }
    }
}
