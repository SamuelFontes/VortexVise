using System.Numerics;
using VortexVise.Core;
using VortexVise.Core.Enums;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.Networking;
using VortexVise.Desktop.States;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.GameContext;

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
    public static void UpdateUserInterface(GameCore gameCore)
    {
        // Handle cursor, show and hide cursor based on movement
        //---------------------------------------------------------
        Vector2 newCursorPosition = Raylib.GetMousePosition();
        Vector2 virtualMouse = new();
        virtualMouse.X = (newCursorPosition.X - (Raylib.GetScreenWidth() - gameCore.GameScreenWidth) * 0.5f);
        virtualMouse.Y = (newCursorPosition.Y - (Raylib.GetScreenHeight() - gameCore.GameScreenHeight) * 0.5f);
        virtualMouse = RayMath.Vector2Clamp(virtualMouse, new(0, 0), new Vector2(gameCore.GameScreenWidth, gameCore.GameScreenHeight));
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

    public static void DrawUserInterface(IRendererService rendererService, GameCore gameCore)
    {
        DrawDebug();
        DrawCursor();
        DrawHud(rendererService, gameCore);
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
        //Raylib.DrawTextEx(GameAssets.Misc.Font, Utils.DebugText, new(8, 540 - 32), 32f, 0, Raylib.WHITE);
        //Raylib.DrawFPS(0, 0);
    }

    private static void DrawHud(IRendererService rendererService, GameCore gameCore)
    {
        if (GameMatch.GameState?.MatchState == MatchStates.Playing)
        {
            //  Maybe this needs to be optimized who knows
            // Player 1
            var p = new Vector2(8, 8);
            var playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == gameCore.PlayerOneProfile.Id);
            if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

            // Player 2
            p = new Vector2(gameCore.GameScreenWidth - 128 - 8, 8);
            playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == gameCore.PlayerTwoProfile.Id);
            if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

            // Player 3
            p = new Vector2(8, gameCore.GameScreenHeight * 0.5f + 8);
            playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == gameCore.PlayerThreeProfile.Id);
            if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

            // Player 4
            p = new Vector2(gameCore.GameScreenWidth - 128 - 8, gameCore.GameScreenHeight * 0.5f + 8);
            playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == gameCore.PlayerFourProfile.Id);
            if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

            if (IsShowingScoreboard)
            {
                var players = GameMatch.GameState.PlayerStates.OrderByDescending(x => x.Stats.Kills).ToList();
                Raylib.DrawRectangle(0, 0, gameCore.GameScreenWidth, gameCore.GameScreenHeight, new(0, 0, 0, 100));
                DrawScoreboard(rendererService, players, gameCore);
            }

            // Draw kill feed 
            // ----------------------------------
            var y = 48;
            foreach (var kill in GameMatch.GameState.KillFeedStates)
            {
                var color = Raylib.WHITE;
                if (kill.Timer < 3) color.a = (byte)(kill.Timer * 255 / 3);
                int x = (int)(gameCore.GameScreenWidth * 0.5f);
                if (Utils.GetNumberOfLocalPlayers(gameCore) == 1)
                {
                    x = (int)(gameCore.GameScreenWidth - 64);
                    y = 8;
                }
                x -= (int)(GameAssets.HUD.KillFeedBackground.Width * 0.5f);
                rendererService.DrawTexture(GameAssets.HUD.KillFeedBackground, x, y, color.ToDrawingColor());

                var killed = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == kill.KilledId);
                if (killed == null) continue;

                if (kill.KilledId != kill.KillerId && kill.KillerId != Guid.Empty)
                {
                    var killer = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == kill.KillerId);
                    if (killer == null) continue;

                    rendererService.DrawTexture(killer.Skin.Texture, x, y, color.ToDrawingColor());
                    x += 40;
                    rendererService.DrawTexture(GameAssets.HUD.Kill, x, y + 8, color.ToDrawingColor());
                    x += 20;
                    rendererService.DrawTexture(killed.Skin.Texture, x, y, color.ToDrawingColor());
                }
                else
                {
                    x += 40;
                    rendererService.DrawTexture(GameAssets.HUD.Death, x, y + 8, color.ToDrawingColor());
                    x += 20;
                    rendererService.DrawTexture(killed.Skin.Texture, x, y, color.ToDrawingColor());
                }
                y += 40;
            }
        }



        // Global HUD
        if (GameMatch.GameState?.MatchState == MatchStates.Warmup)
        {
            Raylib.DrawRectangle(0, 0, gameCore.GameScreenWidth, gameCore.GameScreenHeight, new(0, 0, 0, 100));
            Utils.DrawTextCentered($"STARTING IN {(int)GameMatch.GameState.MatchTimer + 1}", new(gameCore.GameScreenWidth * 0.5f, gameCore.GameScreenHeight * 0.5f), 32, Raylib.WHITE);
        }
        else if (GameMatch.GameState?.MatchState == MatchStates.Playing)
        {
            var t = TimeSpan.FromSeconds((int)GameMatch.GameState.MatchTimer);
            Utils.DrawTextCentered($"{t.ToString(@"mm\:ss")}", new(gameCore.GameScreenWidth * 0.5f, 32), 32, Raylib.WHITE);
        }
        else if (GameMatch.GameState?.MatchState == MatchStates.EndScreen)
        {
            Raylib.DrawRectangle(0, 0, gameCore.GameScreenWidth, gameCore.GameScreenHeight, new(0, 0, 0, 100));
            var t = TimeSpan.FromSeconds((int)GameMatch.GameState.MatchTimer);
            Utils.DrawTextCentered($"RESULTS - {t.ToString(@"mm\:ss")}", new(gameCore.GameScreenWidth * 0.5f, 32), 32, Raylib.WHITE);
            var y = 64;
            var players = GameMatch.GameState.PlayerStates.OrderByDescending(x => x.Stats.Kills).ToList();
            if (players.Count > 1 && players[0].Stats.Kills > players[1].Stats.Kills)
            {
                //Utils.DrawTextCentered($"PLAYER {players[0].Id} WON!", new(gameCore.GameScreenWidth * 0.5f, y), 32, Raylib.WHITE);
            }
            else
            {
                Utils.DrawTextCentered($"DRAW", new(gameCore.GameScreenWidth * 0.5f, y), 32, Raylib.WHITE);
            }
            GameUserInterface.DrawScoreboard(rendererService, players, gameCore);
        }
        else if (GameMatch.GameState?.MatchState == MatchStates.Voting)
        {
            Raylib.DrawRectangle(0, 0, gameCore.GameScreenWidth, gameCore.GameScreenHeight, new(0, 0, 0, 100));
            var t = TimeSpan.FromSeconds((int)GameMatch.GameState.MatchTimer);
            Utils.DrawTextCentered($"MAP VOTING - {t.ToString(@"mm\:ss")}", new(gameCore.GameScreenWidth * 0.5f, 32), 32, Raylib.WHITE);
        }


        if (GameClient.IsConnected) Raylib.DrawText(GameClient.Ping.ToString(), 0, 32, 32, Raylib.RAYWHITE);
    }

    private static void DrawPlayerInfo(IRendererService rendererService, PlayerState playerState, Vector2 position)
    {

        rendererService.DrawTexture(GameAssets.HUD.HudBorder, (int)position.X, (int)position.Y, System.Drawing.Color.White);
        if (!playerState.IsDead)
        {
            rendererService.DrawTextureEx(playerState.Skin.Texture, position, 0, 2, System.Drawing.Color.White);
            var skinHeight = playerState.Skin.Texture.Height; // fixed skin height
            var total = 100;
            var hpPercent = (playerState.HeathPoints * 100) / GameMatch.DefaultPlayerHeathPoints;
            int redHeight = (hpPercent * skinHeight) / total;
            if (redHeight % 2 != 0) redHeight++;

            rendererService.DrawTexturePro(playerState.Skin.Texture, new(0, redHeight, 32, 32 - redHeight), new((int)position.X, (int)position.Y + redHeight * 2, 64, 64 - (redHeight * 2)), new(0, 0), 0, System.Drawing.Color.Red);
        }
        else
        {
            rendererService.DrawTextureEx(playerState.Skin.Texture, position, 0, 2, System.Drawing.Color.DarkGray);
        }

        // Draw bullets
        var weaponPosition = position + new Vector2(72, 0);
        var weapon = playerState.WeaponStates.FirstOrDefault(x => x.IsEquipped);
        if (weapon != null)
        {
            rendererService.DrawTextureEx(weapon.Weapon.Texture, weaponPosition, 0, 1, System.Drawing.Color.White);
            weaponPosition += new Vector2(-12, 32);
            for (int i = 0; i < weapon.CurrentAmmo; i++)
            {
                rendererService.DrawTextureEx(GameAssets.HUD.BulletCounter, weaponPosition, 0, 1, new Color(255, 255, 255, 255).ToDrawingColor());
                weaponPosition += new Vector2(8, 0);
                if (i != 0 && i % 7 == 0) weaponPosition += new Vector2(-64, 8);
            }
        }

        var scorePosition = position + new Vector2(8, 72);

        rendererService.DrawTexture(GameAssets.HUD.Kill, (int)scorePosition.X, (int)scorePosition.Y, System.Drawing.Color.White);
        scorePosition += new Vector2(32, 8);
        Utils.DrawTextCentered($"{playerState.Stats.Kills}", scorePosition, 16, Raylib.WHITE);
        scorePosition += new Vector2(32, -8);
        rendererService.DrawTexture(GameAssets.HUD.Death, (int)scorePosition.X, (int)scorePosition.Y, System.Drawing.Color.White);
        scorePosition += new Vector2(32, 8);
        Utils.DrawTextCentered($"{playerState.Stats.Deaths}", scorePosition, 16, Raylib.WHITE);


    }

    public static void DrawScoreboard(IRendererService rendererService, List<PlayerState> players, GameCore gameCore)
    {
        var y = 96;
        foreach (var player in players)
        {
            int x = (int)(gameCore.GameScreenWidth * 0.5f);
            x -= 64;
            //Utils.DrawTextCentered($"Player {player.Id}", new(x, y), 16, Raylib.WHITE);
            rendererService.DrawTexture(player.Skin.Texture, x - 16, y - 16, System.Drawing.Color.White);
            x += 32;
            y -= 8;
            rendererService.DrawTexture(GameAssets.HUD.Kill, x, y, System.Drawing.Color.White);
            x += 32;
            y += 8;
            Utils.DrawTextCentered($"{player.Stats.Kills}", new(x, y), 16, Raylib.WHITE);
            x += 32;
            y -= 8;
            rendererService.DrawTexture(GameAssets.HUD.Death, x, y, System.Drawing.Color.White);
            x += 32;
            y += 8;
            Utils.DrawTextCentered($"{player.Stats.Deaths}", new(x, y), 16, Raylib.WHITE);
            y += 32;
        }

    }


}

