using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Services
{
    internal class WindowService : IWindowService
    {
        public void InitializeWindow()
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);                                               // Make game window resizeble
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
            // window should close
            GameCore.GameShouldClose = Raylib.WindowShouldClose();
            // fullscreen toggle
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
            {
                if (GameSettings.BorderlessFullScreen)
                    Raylib.ToggleBorderlessWindowed();
                else
                    Raylib.ToggleFullscreen();
            }
            // Debug toggle
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_F7)) Utils.SwitchDebug();
        }
        public int GetScreenHeight()
        {
            return ZeroElectric.Vinculum.Raylib.GetScreenHeight();
        }

        public int GetScreenWidth()
        {
            return ZeroElectric.Vinculum.Raylib.GetScreenWidth();
        }

        public Vector2 GetMousePosition()
        {
            return ZeroElectric.Vinculum.Raylib.GetMousePosition();
        }

    }
}
