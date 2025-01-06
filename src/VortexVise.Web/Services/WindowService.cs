using Raylib_cs;
using System;
using System.Numerics;
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
            // Debug toggle
            if (Raylib.IsKeyPressed(KeyboardKey.F9))
            {
                Utils.SwitchDebug();
            }
        }

        public int GetScreenHeight()
        {
            return Raylib_cs.Raylib.GetScreenHeight();
        }


        public int GetScreenWidth()
        {
            return Raylib_cs.Raylib.GetScreenWidth();
        }


        public Vector2 GetMousePosition()
        {
            return Raylib_cs.Raylib.GetMousePosition();
        }
    }
}
