using System.Numerics;
using VortexVise.Core.Enums;
using VortexVise.Core.Extensions;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Networking;
using VortexVise.Core.States;
using VortexVise.Core.Models;
using VortexVise.Core.Utilities;

namespace VortexVise.Core.GameGlobals
{
    /// <summary>
    /// This will handle all game user interface that will overlay over different scenes. For example the custom mouse cursor.
    /// </summary>
    public static class GameUserInterface
    {
        public static ITextureAsset? Cursor { get; private set; }
        public static Vector2 CursorPosition { get; private set; }
        public static float TimeIdle { get; private set; } = 0;
        public static float CursorAlpha { get; private set; } = 0;
        public static bool IsCursorVisible { get; set; } = true;
        public static bool DisableCursor { get; set; } = false;
        public static bool IsShowingScoreboard { get; set; } = false;
        public static void InitUserInterface<T>() where T : ITextureAsset, new()
        {
            Cursor = new T();// I want this to instantiate the implementation 
            Cursor.Load("Resources/Common/cursor.png");
            IsCursorVisible = false;
            DisableCursor = false;
        }
        public static void UpdateUserInterface(IRendererService rendererService)
        {
            // Handle cursor, show and hide cursor based on movement
            //---------------------------------------------------------
            Vector2 newCursorPosition = rendererService.GetMousePosition();
            Vector2 virtualMouse = new();
            virtualMouse.X = newCursorPosition.X - (rendererService.GetScreenWidth() - GameCore.GameScreenWidth) * 0.5f;
            virtualMouse.Y = newCursorPosition.Y - (rendererService.GetScreenHeight() - GameCore.GameScreenHeight) * 0.5f;
            virtualMouse = virtualMouse.Clamp(new(0, 0), new Vector2(GameCore.GameScreenWidth, GameCore.GameScreenHeight));
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
                else if (TimeIdle < 2) TimeIdle += rendererService.GetFrameTime();
            }
            if (IsCursorVisible && CursorAlpha < 255) CursorAlpha = 255; // When showing it shows at once
            else if (!IsCursorVisible && CursorAlpha > 0) CursorAlpha -= rendererService.GetFrameTime() * 300; // Slowly hides cursor when no movement

            CursorPosition = newCursorPosition;
        }

        public static void DrawUserInterface(IRendererService rendererService)
        {
            DrawDebug();
            DrawCursor(rendererService);
            DrawHud(rendererService);
        }
        public static void UnloadUserInterface()
        {
            Cursor.Unload();
        }

        private static void DrawCursor(IRendererService rendererService)
        {
            // Draw Cursor
            //---------------------------------------------------------
            int alpha = 0;
            if (CursorAlpha > 255) alpha = 255;
            else alpha = (int)CursorAlpha;


            if (!DisableCursor)
            {
                rendererService.DrawTexture(Cursor, (int)CursorPosition.X, (int)CursorPosition.Y, System.Drawing.Color.FromArgb(alpha, 255, 255, 255));
            }

        }
        private static void DrawDebug()
        {
            //Raylib.DrawTextEx(GameAssets.Misc.Font, Utils.DebugText, new(8, 540 - 32), 32f, 0, System.Drawing.Color.White);
            //Raylib.DrawFPS(0, 0);
        }

        private static void DrawHud(IRendererService rendererService)
        {
            if (GameMatch.GameState?.MatchState == MatchStates.Playing)
            {
                //  Maybe this needs to be optimized who knows
                // Player 1
                var p = new Vector2(8, 8);
                var playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerOneProfile.Id);
                if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

                // Player 2
                p = new Vector2(GameCore.GameScreenWidth - 128 - 8, 8);
                playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerTwoProfile.Id);
                if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

                // Player 3
                p = new Vector2(8, GameCore.GameScreenHeight * 0.5f + 8);
                playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerThreeProfile.Id);
                if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

                // Player 4
                p = new Vector2(GameCore.GameScreenWidth - 128 - 8, GameCore.GameScreenHeight * 0.5f + 8);
                playerState = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == GameCore.PlayerFourProfile.Id);
                if (playerState != null) DrawPlayerInfo(rendererService, playerState, p);

                if (IsShowingScoreboard)
                {
                    var players = GameMatch.GameState.PlayerStates.OrderByDescending(x => x.Stats.Kills).ToList();
                    rendererService.DrawRectangleRec(new System.Drawing.Rectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight), System.Drawing.Color.FromArgb(100, 0, 0, 0));
                    DrawScoreboard(rendererService, players);
                }

                // Draw kill feed 
                // ----------------------------------
                var y = 48;
                foreach (var kill in GameMatch.GameState.KillFeedStates)
                {
                    var color = System.Drawing.Color.White;
                    if (kill.Timer < 3) color = System.Drawing.Color.FromArgb((byte)(kill.Timer * 255 / 3), color);
                    int x = (int)(GameCore.GameScreenWidth * 0.5f);
                    if (Utils.GetNumberOfLocalPlayers() == 1)
                    {
                        x = GameCore.GameScreenWidth - 64;
                        y = 8;
                    }
                    x -= (int)(GameAssets.HUD.KillFeedBackground.Width * 0.5f);
                    rendererService.DrawTexture(GameAssets.HUD.KillFeedBackground, x, y, color);

                    var killed = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == kill.KilledId);
                    if (killed == null) continue;

                    if (kill.KilledId != kill.KillerId && kill.KillerId != Guid.Empty)
                    {
                        var killer = GameMatch.GameState.PlayerStates.FirstOrDefault(x => x.Id == kill.KillerId);
                        if (killer == null) continue;

                        rendererService.DrawTexture(killer.Skin.Texture, x, y, color);
                        x += 40;
                        rendererService.DrawTexture(GameAssets.HUD.Kill, x, y + 8, color);
                        x += 20;
                        rendererService.DrawTexture(killed.Skin.Texture, x, y, color);
                    }
                    else
                    {
                        x += 40;
                        rendererService.DrawTexture(GameAssets.HUD.Death, x, y + 8, color);
                        x += 20;
                        rendererService.DrawTexture(killed.Skin.Texture, x, y, color);
                    }
                    y += 40;
                }
            }



            // Global HUD
            if (GameMatch.GameState?.MatchState == MatchStates.Warmup)
            {
                rendererService.DrawRectangleRec(new System.Drawing.Rectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight), System.Drawing.Color.FromArgb(100, 0, 0, 0));
                rendererService.DrawTextCentered(GameAssets.Misc.Font, $"STARTING IN {(int)GameMatch.GameState.MatchTimer + 1}", new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f), 32, System.Drawing.Color.White);
            }
            else if (GameMatch.GameState?.MatchState == MatchStates.Playing)
            {
                var t = TimeSpan.FromSeconds((int)GameMatch.GameState.MatchTimer);
                rendererService.DrawTextCentered(GameAssets.Misc.Font, $"{t.ToString(@"mm\:ss")}", new(GameCore.GameScreenWidth * 0.5f, 32), 32, System.Drawing.Color.White);
            }
            else if (GameMatch.GameState?.MatchState == MatchStates.EndScreen)
            {
                rendererService.DrawRectangleRec(new System.Drawing.Rectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight), System.Drawing.Color.FromArgb(100, 0, 0, 0));
                var t = TimeSpan.FromSeconds((int)GameMatch.GameState.MatchTimer);
                rendererService.DrawTextCentered(GameAssets.Misc.Font, $"RESULTS - {t.ToString(@"mm\:ss")}", new(GameCore.GameScreenWidth * 0.5f, 32), 32, System.Drawing.Color.White);
                var y = 64;
                var players = GameMatch.GameState.PlayerStates.OrderByDescending(x => x.Stats.Kills).ToList();
                if (players.Count > 1 && players[0].Stats.Kills > players[1].Stats.Kills)
                {
                    //Utils.DrawTextCentered($"PLAYER {players[0].Id} WON!", new(GameCore.GameScreenWidth * 0.5f, y), 32, System.Drawing.ColorSystem.Drawing.Color.White);
                }
                else
                {
                    rendererService.DrawTextCentered(GameAssets.Misc.Font, $"DRAW", new(GameCore.GameScreenWidth * 0.5f, y), 32, System.Drawing.Color.White);
                }
                DrawScoreboard(rendererService, players);
            }
            else if (GameMatch.GameState?.MatchState == MatchStates.Voting)
            {
                rendererService.DrawRectangleRec(new System.Drawing.Rectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight), System.Drawing.Color.FromArgb(100, 0, 0, 0));
                var t = TimeSpan.FromSeconds((int)GameMatch.GameState.MatchTimer);
                rendererService.DrawTextCentered(GameAssets.Misc.Font, $"MAP VOTING - {t.ToString(@"mm\:ss")}", new(GameCore.GameScreenWidth * 0.5f, 32), 32, System.Drawing.Color.White);
            }


            if (GameClient.IsConnected) rendererService.DrawText(GameClient.Ping.ToString(), new(0, 32), 32, System.Drawing.Color.White);
        }

        private static void DrawPlayerInfo(IRendererService rendererService, PlayerState playerState, Vector2 position)
        {

            rendererService.DrawTexture(GameAssets.HUD.HudBorder, (int)position.X, (int)position.Y, System.Drawing.Color.White);
            if (!playerState.IsDead)
            {
                rendererService.DrawTextureEx(playerState.Skin.Texture, position, 0, 2, System.Drawing.Color.White);
                var skinHeight = playerState.Skin.Texture.Height; // fixed skin height
                var total = 100;
                var hpPercent = playerState.HeathPoints * 100 / GameMatch.DefaultPlayerHeathPoints;
                int redHeight = hpPercent * skinHeight / total;
                if (redHeight % 2 != 0) redHeight++;

                rendererService.DrawTexturePro(playerState.Skin.Texture, new(0, redHeight, 32, 32 - redHeight), new((int)position.X, (int)position.Y + redHeight * 2, 64, 64 - redHeight * 2), new(0, 0), 0, System.Drawing.Color.Red);
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
                    rendererService.DrawTextureEx(GameAssets.HUD.BulletCounter, weaponPosition, 0, 1, System.Drawing.Color.FromArgb(255, 255, 255, 255));
                    weaponPosition += new Vector2(8, 0);
                    if (i != 0 && i % 7 == 0) weaponPosition += new Vector2(-64, 8);
                }
            }

            var scorePosition = position + new Vector2(8, 72);

            rendererService.DrawTexture(GameAssets.HUD.Kill, (int)scorePosition.X, (int)scorePosition.Y, System.Drawing.Color.White);
            scorePosition += new Vector2(32, 8);
            rendererService.DrawTextCentered(GameAssets.Misc.Font, $"{playerState.Stats.Kills}", scorePosition, 16, System.Drawing.Color.White);
            scorePosition += new Vector2(32, -8);
            rendererService.DrawTexture(GameAssets.HUD.Death, (int)scorePosition.X, (int)scorePosition.Y, System.Drawing.Color.White);
            scorePosition += new Vector2(32, 8);
            rendererService.DrawTextCentered(GameAssets.Misc.Font, $"{playerState.Stats.Deaths}", scorePosition, 16, System.Drawing.Color.White);


        }

        public static void DrawScoreboard(IRendererService rendererService, List<PlayerState> players)
        {
            var y = 96;
            foreach (var player in players)
            {
                int x = (int)(GameCore.GameScreenWidth * 0.5f);
                x -= 64;
                //rendererService.DrawTextCentered($"Player {player.Id}", new(x, y), 16, System.Drawing.Color.White);
                rendererService.DrawTexture(player.Skin.Texture, x - 16, y - 16, System.Drawing.Color.White);
                x += 32;
                y -= 8;
                rendererService.DrawTexture(GameAssets.HUD.Kill, x, y, System.Drawing.Color.White);
                x += 32;
                y += 8;
                rendererService.DrawTextCentered(GameAssets.Misc.Font, $"{player.Stats.Kills}", new(x, y), 16, System.Drawing.Color.White);
                x += 32;
                y -= 8;
                rendererService.DrawTexture(GameAssets.HUD.Death, x, y, System.Drawing.Color.White);
                x += 32;
                y += 8;
                rendererService.DrawTextCentered(GameAssets.Misc.Font, $"{player.Stats.Deaths}", new(x, y), 16, System.Drawing.Color.White);
                y += 32;
            }

        }


    }
}
