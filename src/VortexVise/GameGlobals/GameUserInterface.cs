using System.Numerics;
using VortexVise.States;
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
        DrawHud();
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

    private static void DrawHud()
    {
        if (GameMatch.GameState?.MatchState == Enums.MatchStates.Playing)
        {
            //  Maybe this needs to be optimized who knows
            // Player 1
            var p = new Vector2(8, 8);
            var playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerOneProfile.Id);
            if (playerState != null) DrawPlayerInfo(playerState, p);

            // Player 2
            p = new Vector2(GameCore.GameScreenWidth - 128 - 8, 8);
            playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerTwoProfile.Id);
            if (playerState != null) DrawPlayerInfo(playerState, p);

            // Player 3
            p = new Vector2(8, GameCore.GameScreenHeight * 0.5f + 8);
            playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerThreeProfile.Id);
            if (playerState != null) DrawPlayerInfo(playerState, p);

            // Player 4
            p = new Vector2(GameCore.GameScreenWidth - 128 - 8, GameCore.GameScreenHeight * 0.5f + 8);
            playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerFourProfile.Id);
            if (playerState != null) DrawPlayerInfo(playerState, p);
        }



    }

    private static void DrawPlayerInfo(PlayerState playerState, Vector2 position)
    {

        Raylib.DrawTexture(GameAssets.HUD.HudBorder, (int)position.X, (int)position.Y, Raylib.WHITE);
        if (!playerState.IsDead)
        {
            Raylib.DrawTextureEx(playerState.Skin.Texture, position, 0, 2, Raylib.WHITE);
            var skinHeight = playerState.Skin.Texture.height; // fixed skin height
            var total = 100;
            var hpPercent = (playerState.HeathPoints * 100) / GameMatch.DefaultPlayerHeathPoints;
            int redHeight = (hpPercent * skinHeight) / total;
            if (redHeight % 2 != 0) redHeight++;

            Raylib.DrawTexturePro(playerState.Skin.Texture, new(0, redHeight, 32, 32 - redHeight), new(position.X, position.Y + redHeight * 2, 64, 64 - (redHeight * 2)), new(0, 0), 0, Raylib.RED);
        }
        else
        {
            Raylib.DrawTextureEx(playerState.Skin.Texture, position, 0, 2, Raylib.DARKGRAY);
        }
        position += new Vector2(72, 0);
        var weapon = playerState.WeaponStates.FirstOrDefault(x => x.IsEquipped);
        if (weapon != null)
        {
            Raylib.DrawTextureEx(weapon.Weapon.Texture, position, 0, 1, Raylib.WHITE);
            position += new Vector2(-12, 32);
            for (int i = 0; i < weapon.CurrentAmmo; i++)
            {
                Raylib.DrawTextureEx(GameAssets.HUD.BulletCounter, position, 0, 1, new(255, 255, 255, 255));
                position += new Vector2(8, 0);
                if (i != 0 && i % 7 == 0) position += new Vector2(-64, 8);
            }
        }

    }


}

