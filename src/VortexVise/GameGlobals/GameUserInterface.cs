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
    public static bool IsShowingScoreboard { get; set; } = false;
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

            if (IsShowingScoreboard)
            {
                var players = GameMatch.GameState.PlayerStates.OrderByDescending(x => x.Stats.Kills).ToList();
                Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, new(0, 0, 0, 100));
                DrawScoreboard(players);
            }

            // Draw kill feed 
            // ----------------------------------
            var y = 48;
            foreach (var kill in GameMatch.GameState.KillFeedStates)
            {
                var color = Raylib.WHITE;
                if(kill.Timer < 3) color.a = (byte)(kill.Timer * 255 / 3);
                int x = (int)(GameCore.GameScreenWidth * 0.5f);
                x -= (int)(GameAssets.HUD.KillFeedBackground.width * 0.5f);
                Raylib.DrawTexture(GameAssets.HUD.KillFeedBackground, x, y, color);
                
                var killed = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == kill.KilledId);
                if (killed == null) continue;

                if (kill.KilledId != kill.KillerId && kill.KillerId != -1)
                {
                    var killer = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == kill.KillerId);
                    if (killer == null) continue;

                    Raylib.DrawTexture(killer.Skin.Texture, x, y, color);
                    x += 40;
                    Raylib.DrawTexture(GameAssets.HUD.Kill, x, y + 8, color);
                    x += 20;
                    Raylib.DrawTexture(killed.Skin.Texture, x, y, color);
                }
                else
                {
                    x += 20;
                    Raylib.DrawTexture(GameAssets.HUD.Death, x, y + 8, color);
                    x += 20;
                    Raylib.DrawTexture(killed.Skin.Texture, x, y, color);
                }
                y += 40;
            }
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

        // Draw bullets
        var weaponPosition = position + new Vector2(72, 0);
        var weapon = playerState.WeaponStates.FirstOrDefault(x => x.IsEquipped);
        if (weapon != null)
        {
            Raylib.DrawTextureEx(weapon.Weapon.Texture, weaponPosition, 0, 1, Raylib.WHITE);
            weaponPosition += new Vector2(-12, 32);
            for (int i = 0; i < weapon.CurrentAmmo; i++)
            {
                Raylib.DrawTextureEx(GameAssets.HUD.BulletCounter, weaponPosition, 0, 1, new(255, 255, 255, 255));
                weaponPosition += new Vector2(8, 0);
                if (i != 0 && i % 7 == 0) weaponPosition += new Vector2(-64, 8);
            }
        }

        var scorePosition = position + new Vector2(8, 72);

        Raylib.DrawTexture(GameAssets.HUD.Kill, (int)scorePosition.X, (int)scorePosition.Y, Raylib.WHITE);
        scorePosition += new Vector2(32, 8);
        Utils.DrawTextCentered($"{playerState.Stats.Kills}", scorePosition, 16, Raylib.WHITE);
        scorePosition += new Vector2(32, -8);
        Raylib.DrawTexture(GameAssets.HUD.Death, (int)scorePosition.X, (int)scorePosition.Y, Raylib.WHITE);
        scorePosition += new Vector2(32, 8);
        Utils.DrawTextCentered($"{playerState.Stats.Deaths}", scorePosition, 16, Raylib.WHITE);


    }

    public static void DrawScoreboard(List<PlayerState> players)
    {
        var y = 96;
        foreach (var player in players)
        {
            int x = (int)(GameCore.GameScreenWidth * 0.5f);
            x -= 128;
            Utils.DrawTextCentered($"Player {player.Id + 1}", new(x, y), 16, Raylib.WHITE);
            x += 128;
            y -= 8;
            Raylib.DrawTexture(GameAssets.HUD.Kill, x, y, Raylib.WHITE);
            x += 32;
            y += 8;
            Utils.DrawTextCentered($"{player.Stats.Kills}", new(x, y), 16, Raylib.WHITE);
            x += 32;
            y -= 8;
            Raylib.DrawTexture(GameAssets.HUD.Death, x, y, Raylib.WHITE);
            x += 32;
            y += 8;
            Utils.DrawTextCentered($"{player.Stats.Deaths}", new(x, y), 16, Raylib.WHITE);
            y += 16;
        }

    }


}

