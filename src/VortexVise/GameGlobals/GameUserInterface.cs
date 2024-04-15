﻿using System.Numerics;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

/// <summary>
/// This will handle all game user interface that will overlay over different scenes. For example the custom mouse cursor.
/// </summary>
static internal class GameUserInterface
{
    public static Texture Cursor { get; private set; }
    public static Vector2 CursorPosition { get; private set; }
    public static float TimeIdle { get; private set; } = 0;
    public static float CursorAlpha { get; private set; } = 0;
    public static bool IsCursorVisible { get; set; } = true;
    public static bool DisableCursor { get; set; } = false;
    public static void InitUserInterface()
    {
        Cursor = Raylib.LoadTexture("Resources/Common/cursor.png");
        IsCursorVisible = false;
        DisableCursor = false;
    }
    public static void UpdateUserInterface()
    {
        // Handle cursor, show and hide cursor based on movement
        //---------------------------------------------------------
        Vector2 newCursorPosition = Raylib.GetMousePosition();
        Vector2 virtualMouse = new();
        virtualMouse.X = (newCursorPosition.X - (Raylib.GetScreenWidth() - GameCore.GameScreenWidth * GameCore.GameScreenScale) * 0.5f) / GameCore.GameScreenScale;
        virtualMouse.Y = (newCursorPosition.Y - (Raylib.GetScreenHeight() - GameCore.GameScreenHeight * GameCore.GameScreenScale) * 0.5f) / GameCore.GameScreenScale;
        virtualMouse = RayMath.Vector2Clamp(virtualMouse, new(0, 0), new Vector2(GameCore.GameScreenWidth, GameCore.GameScreenHeight));
        newCursorPosition = virtualMouse;

        if (newCursorPosition.X != CursorPosition.X || newCursorPosition.Y != CursorPosition.Y)
        {
            // Show cursor on movement
            IsCursorVisible = true;
            TimeIdle = 0;
        }
        else
        {
            // Hide cursor after a few seconds
            if (IsCursorVisible && TimeIdle > 2) IsCursorVisible = false;
            else if (TimeIdle < 2) TimeIdle += Raylib.GetFrameTime();
        }
        if (IsCursorVisible && CursorAlpha < 255) CursorAlpha = 255; // When showing it shows at once
        else if (!IsCursorVisible && CursorAlpha > 0) CursorAlpha -= Raylib.GetFrameTime() * 300; // Slowly hides cursor when no movement

        CursorPosition = newCursorPosition;
    }

    public static void DrawUserInterface()
    {
        DrawDebug();
        DrawCursor();
    }
    public static void UnloadUserInterface()
    {
        Raylib.UnloadTexture(Cursor);
    }

    private static void DrawCursor()
    {
        // Draw Cursor
        //---------------------------------------------------------
        int alpha = 0;
        if (CursorAlpha > 255) alpha = 255;
        else alpha = (int)CursorAlpha;


        if (!DisableCursor)
        {
            Raylib.DrawTexture(Cursor, (int)CursorPosition.X, (int)CursorPosition.Y, new Color(255, 255, 255, alpha));
        }

    }
    private static void DrawDebug()
    {
        Raylib.DrawTextEx(GameAssets.Misc.Font, Utils.DebugText, new(8, 540 - 32), 32f, 0, Raylib.WHITE);
        //Raylib.DrawFPS(0, 0);
    }

}

