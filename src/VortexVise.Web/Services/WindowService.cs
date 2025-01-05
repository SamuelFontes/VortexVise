using Raylib_cs;
using System;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Utilities;

namespace VortexVise.Web.Services
{
    public class WindowService : IWindowService
    {

        public void InitializeWindow()
        {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);                                               // Make game window resizeble
            Raylib.InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, GameCore.GameName);                  // Create game window
            Raylib.InitAudioDevice();                                                                               // Initialize audio device
            Raylib.HideCursor();                                                                                    // Hide windows cursor
            Raylib.SetTargetFPS(GameSettings.TargetFPS);                                                            // Set game target FPS
            Raylib.SetExitKey(0);                                                                                   // Disable escape closing the game
            Image icon = Raylib.LoadImage("Resources/Skins/afatso.png");                                            // Load icon at runtime
            Raylib.SetWindowIcon(icon);                                                                             // Set icon
            Raylib.UnloadImage(icon);                                                                               // Unload icon from memory
        }


        public void CloseWindow()
        {
            Raylib.CloseAudioDevice();     // Close audio context
            Raylib.CloseWindow();          // Close window and OpenGL context
        }


        public void HandleWindowEvents()
        {
            Console.WriteLine("IsKeyPress?");
            // Debug toggle
            if (Raylib.IsKeyPressed(KeyboardKey.F7))
            {
                Console.WriteLine("Gonna debug");
                Utils.SwitchDebug();
                Console.WriteLine("Debug");
            }
            Console.WriteLine("Nope");
        }
    }
}
