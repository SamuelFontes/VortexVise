using System.Numerics;
using VortexVise.Core.Enums;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.Logic;
using VortexVise.Desktop.States;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.GameContext;

/// <summary>
/// This will handle all game rendering
/// </summary>
public static class GameRenderer
{
    /// <summary>
    /// Render game state to the screen.
    /// </summary>
    /// <param name="state">Current game state.</param>
    public static void DrawGameState(IRendererService rendererService, GameState state, PlayerState mainPlayer)
    {
        // Clears menu states
        GameUserInterface.IsShowingScoreboard = false;

        // All rendering logic should go here
        DrawMap(rendererService);
        DrawWeaponDrops(rendererService, state);
        foreach (var playerState in state.PlayerStates)
        {
            if (playerState.Id == mainPlayer.Id) continue;

            DrawHookState(rendererService, playerState);
            DrawPlayerState(rendererService, playerState);
            DrawEnemyHealthBar(rendererService, playerState);
        }
        // Draw main player on top for this screen
        DrawHookState(rendererService, mainPlayer);
        DrawPlayerState(rendererService, mainPlayer);
        DrawProjectiles(rendererService, state);
        DrawStateAnimations(rendererService, state);
        DrawHud(rendererService, state, mainPlayer);
    }

    private static void DrawStateAnimations(IRendererService rendererService, GameState state)
    {
        foreach (var animation in state.Animations)
        {
            if (animation.Animation == null) continue;

            var frameStartX = animation.State * animation.Animation.Size;
            System.Drawing.Rectangle sourceRec = new(frameStartX, 0, animation.Animation.Size, animation.Animation.Texture.Height);

            System.Drawing.Rectangle destRec = new((int)animation.Position.X, (int)animation.Position.Y, animation.Animation.Size * animation.Animation.Scale, animation.Animation.Texture.Height * animation.Animation.Scale);

            rendererService.DrawTexturePro(animation.Animation.Texture, sourceRec, destRec, new(0, 0), 0, animation.Animation.Color);

        }
    }

    private static void DrawProjectiles(IRendererService rendererService, GameState state)
    {
        foreach (var hitbox in state.DamageHitBoxes)
        {
            if (hitbox.Weapon.ProjectileTextureLocation == string.Empty) continue;
            System.Drawing.Rectangle sourceRec = new(0, 0, hitbox.Weapon.ProjectileTexture.Width, hitbox.Weapon.ProjectileTexture.Height);

            System.Drawing.Rectangle destRec = new((int)(hitbox.HitBox.X + hitbox.HitBox.Width * 0.5f), hitbox.HitBox.Y + (int)(hitbox.HitBox.Height * 0.5f), hitbox.Weapon.ProjectileTexture.Width, hitbox.Weapon.ProjectileTexture.Height);

            rendererService.DrawTexturePro(hitbox.Weapon.ProjectileTexture, sourceRec, destRec, new((int)(hitbox.Weapon.ProjectileTexture.Width * 0.5f), (int)(hitbox.Weapon.ProjectileTexture.Height * 0.5f)), (int)WeaponLogic.WeaponRotation * 23, System.Drawing.Color.White);
        }

    }

    static void DrawMap(IRendererService rendererService)
    {
        rendererService.DrawTextureRec(GameMatch.CurrentMap.Texture, new(0, 0, GameMatch.CurrentMap.Texture.Width * GameMatch.MapMirrored, GameMatch.CurrentMap.Texture.Height), new Vector2(0, 0), System.Drawing.Color.White);
        if (Utils.Debug())
        {
            foreach (var collision in GameMatch.CurrentMap.Collisions) // DEBUG
            {
                rendererService.DrawRectangleRec(collision, System.Drawing.Color.Blue);
            }
        }

    }

    static void DrawWeaponDrops(IRendererService rendererService, GameState currentGameState)
    {
        foreach (var drop in currentGameState.WeaponDrops)
        {
            var rotation = (int)WeaponLogic.WeaponRotation;
            if (drop.WeaponState.Weapon.WeaponType == WeaponType.Heal) rotation = 0;
            System.Drawing.Rectangle sourceRec = new(0, 0, drop.WeaponState.Weapon.Texture.Width, drop.WeaponState.Weapon.Texture.Height);

            System.Drawing.Rectangle destRec = new((int)(drop.Position.X + drop.WeaponState.Weapon.Texture.Width * 0.5f), (int)(drop.Position.Y + drop.WeaponState.Weapon.Texture.Height * 0.5f), drop.WeaponState.Weapon.Texture.Width, drop.WeaponState.Weapon.Texture.Height);

            rendererService.DrawTexturePro(drop.WeaponState.Weapon.Texture, sourceRec, destRec, new(drop.WeaponState.Weapon.Texture.Width * 0.5f, drop.WeaponState.Weapon.Texture.Height * 0.5f), rotation, System.Drawing.Color.White);

            if (Utils.Debug()) Raylib.DrawRectangleRec(drop.Collision.ToRaylibRectangle(), Raylib.PURPLE); // Debug
        }
        if (Utils.Debug()) foreach (var h in currentGameState.DamageHitBoxes) rendererService.DrawRectangleRec(h.HitBox, System.Drawing.Color.Red); // Debug
    }

    static void DrawHookState(IRendererService rendererService, PlayerState playerState)
    {
        if (playerState.IsDead) return;
        if (playerState.HookState.IsHookReleased)
        {
            Raylib.DrawLineEx(PlayerLogic.GetPlayerCenterPosition(playerState.Position), new Vector2((int)playerState.HookState.Position.X + 3, (int)playerState.HookState.Position.Y + 3), 1, new Color(159, 79, 0, 255));
            rendererService.DrawTexture(GameAssets.Gameplay.HookTexture, (int)playerState.HookState.Position.X, (int)playerState.HookState.Position.Y, System.Drawing.Color.White);

            if (Utils.Debug())
                rendererService.DrawRectangleRec(playerState.HookState.Collision, System.Drawing.Color.Green); // Debug
        }



    }
    static void DrawPlayerState(IRendererService rendererService, PlayerState playerState)
    {
        if (playerState.IsDead) return;
        // Draw player 
        // -------------------------------------------------
        System.Drawing.Rectangle sourceRec = new(0, 0, playerState.Skin.Texture.Width * playerState.Direction, playerState.Skin.Texture.Height);

        System.Drawing.Rectangle destRec = new((int)(playerState.Position.X + playerState.Skin.Texture.Width * 0.5f), (int)(playerState.Position.Y + playerState.Skin.Texture.Height * 0.5f), playerState.Skin.Texture.Width, playerState.Skin.Texture.Height);

        var rotation = playerState.Animation.GetAnimationRotation();
        float y = destRec.Y;
        if (rotation != 0) y -= 2f; // this adds a little bump to the walking animation
        destRec.Y = (int)y;

        // Player color
        var color = System.Drawing.Color.White;
        if (playerState.DamagedTimer > 0) color = System.Drawing.Color.Red;

        rendererService.DrawTexturePro(playerState.Skin.Texture, sourceRec, destRec, new Vector2((int)(playerState.Skin.Texture.Width * 0.5f), (int)(playerState.Skin.Texture.Height * 0.5f)), rotation, color); // Draw Player 


        // Draw Weapon
        // -------------------------------------------------
        var weapon = playerState.WeaponStates.FirstOrDefault(x => x.IsEquipped);
        if (weapon != null)
        {
            switch (weapon.Weapon.WeaponType)
            {
                case WeaponType.Shotgun: destRec.Y += 5; break;
                case WeaponType.SMG: destRec.Y += 5; break;
                case WeaponType.Pistol: destRec.Y += 5; break;
                case WeaponType.MeleeBlunt:
                    {
                        if (weapon.ReloadTimer >= weapon.Weapon.ReloadDelay * 0.2f)
                        {
                            destRec.X += 5 * playerState.Direction;
                            sourceRec.Width = -sourceRec.Width;
                            rotation -= 45 * playerState.Direction;
                        }
                        else
                        {
                            destRec.X -= 16 * playerState.Direction;
                            destRec.Y += 5;
                            rotation = 0;
                        }
                        break;
                    }
                case WeaponType.MeleeCut:
                    {
                        if (weapon.ReloadTimer >= weapon.Weapon.ReloadDelay * 0.2f)
                        {
                            destRec.X += 5 * playerState.Direction;
                            sourceRec.Width = -sourceRec.Width;
                            rotation -= 45 * playerState.Direction;
                        }
                        else
                        {
                            destRec.X -= 16 * playerState.Direction;
                            destRec.Y += 5;
                            rotation = 0;
                        }
                        break;
                    }
            }
            rendererService.DrawTexturePro(weapon.Weapon.Texture, sourceRec, destRec, new Vector2(playerState.Skin.Texture.Width * 0.5f, playerState.Skin.Texture.Height * 0.5f), rotation, System.Drawing.Color.White); // Draw Player 
        }

        if (Utils.Debug())
        {
            Raylib.DrawRectangleRec(playerState.Collision, Raylib.GREEN); // Debug
        }
    }

    /// <summary>
    /// This draws hud in game world relative to players view
    /// </summary>
    /// <param name="state"></param>
    /// <param name="mainPlayer"></param>
    private static void DrawHud(IRendererService rendererService, GameState state, PlayerState mainPlayer)
    {
        // Draw weapon grab indicator
        // --------------------------------------------------------
        foreach (var drop in state.WeaponDrops)
        {
            if (Raylib.CheckCollisionRecs(drop.Collision.ToRaylibRectangle(), mainPlayer.Collision))
            {
                rendererService.DrawTexture(GameAssets.HUD.SelectionSquare, (int)drop.Position.X, (int)drop.Position.Y, System.Drawing.Color.White);
            }
        }

        var weapon = mainPlayer.WeaponStates.FirstOrDefault(x => x.IsEquipped);
        if (!mainPlayer.IsDead)
        {
            // Draw player health
            // --------------------------------------------------------
            Vector2 p = new((int)mainPlayer.Position.X, (int)mainPlayer.Position.Y);
            p += new Vector2(8, -16);
            {
                rendererService.DrawTextureEx(GameAssets.HUD.WideBarEmpty, p, 0, 1, new Color(255, 255, 255, 255).ToDrawingColor());
                var spriteHeight = GameAssets.HUD.WideBarGreen.Height;
                var total = 100;
                //var percent = (weapon.ReloadTimer * 100) / weapon.Weapon.ReloadDelay;
                var percent = (mainPlayer.HeathPoints * 100) / GameMatch.DefaultPlayerHeathPoints;
                int overlayHeight = (int)((percent * spriteHeight) / total);
                if (overlayHeight % 2 != 0) overlayHeight++;

                rendererService.DrawTexturePro(GameAssets.HUD.WideBarGreen, new(0, GameAssets.HUD.WideBarGreen.Height - overlayHeight, GameAssets.HUD.WideBarGreen.Width, overlayHeight), new((int)p.X, (int)p.Y + GameAssets.HUD.WideBarGreen.Height - overlayHeight, GameAssets.HUD.WideBarGreen.Width, overlayHeight), new(0, 0), 0, System.Drawing.Color.LightGray);
            }

            // Draw jetpack fuel
            // --------------------------------------------------------
            {
                p = new((int)mainPlayer.Position.X, (int)mainPlayer.Position.Y);
                p += new Vector2(2, -16);
                rendererService.DrawTextureEx(GameAssets.HUD.ThinBarEmpty, p, 0, 1, new Color(255, 255, 255, 255).ToDrawingColor());
                var spriteHeight = GameAssets.HUD.ThinBarOrange.Height;
                var total = 100;
                var percent = (mainPlayer.JetPackFuel * 100) / GameMatch.DefaultJetPackFuel;

                int overlayHeight = (int)((percent * spriteHeight) / total);
                if (overlayHeight % 2 != 0) overlayHeight++;

                rendererService.DrawTexturePro(GameAssets.HUD.ThinBarOrange, new(0, GameAssets.HUD.ThinBarOrange.Height - overlayHeight, GameAssets.HUD.ThinBarOrange.Width, overlayHeight), new((int)p.X, (int)(p.Y + GameAssets.HUD.ThinBarOrange.Height - overlayHeight), GameAssets.HUD.ThinBarOrange.Width, overlayHeight), new(0, 0), 0, System.Drawing.Color.White);
            }

            // Draw weapon bar
            // --------------------------------------------------------
            p = new((int)mainPlayer.Position.X, (int)mainPlayer.Position.Y);
            p += new Vector2(22, -16);
            if (weapon != null)
            {
                rendererService.DrawTextureEx(GameAssets.HUD.ThinBarEmpty, p, 0, 1, new Color(255, 255, 255, 255).ToDrawingColor());
                var spriteHeight = GameAssets.HUD.ThinBarBlue.Height;
                var total = 100;
                var percent = (weapon.ReloadTimer * 100) / weapon.Weapon.ReloadDelay;
                if (percent == 100)
                {
                    rendererService.DrawTextureEx(GameAssets.HUD.ThinBarBlue, p, 0, 1, new Color(255, 255, 255, 255).ToDrawingColor());
                }
                else
                {

                    int overlayHeight = (int)((percent * spriteHeight) / total);
                    if (overlayHeight % 2 != 0) overlayHeight++;

                    rendererService.DrawTexturePro(GameAssets.HUD.ThinBarBlue, new(0, GameAssets.HUD.ThinBarBlue.Height - overlayHeight, GameAssets.HUD.ThinBarBlue.Width, overlayHeight), new((int)p.X, (int)(p.Y + GameAssets.HUD.ThinBarBlue.Height - overlayHeight), GameAssets.HUD.ThinBarBlue.Width, overlayHeight), new(0, 0), 0, new Color(100, 100, 100, 255).ToDrawingColor());
                }
            }
            else
                rendererService.DrawTextureEx(GameAssets.HUD.ThinBarEmpty, p, 0, 1, new Color(255, 255, 255, 255).ToDrawingColor());
        }

        //Scoreboard
        if (mainPlayer.Input.Select && !GameUserInterface.IsShowingScoreboard)
        {
            GameUserInterface.IsShowingScoreboard = true;
        }

    }

    public static void DrawEnemyHealthBar(IRendererService rendererService, PlayerState playerState)
    {
        if (playerState.IsDead) return;
        // Draw player health
        // --------------------------------------------------------
        Vector2 p = new((int)playerState.Position.X, (int)playerState.Position.Y);
        p += new Vector2(8, -16);
        {
            rendererService.DrawTextureEx(GameAssets.HUD.WideBarEmpty, p, 0, 1, new Color(255, 255, 255, 255).ToDrawingColor());
            var spriteHeight = GameAssets.HUD.WideBarRed.Height;
            var total = 100;
            //var percent = (weapon.ReloadTimer * 100) / weapon.Weapon.ReloadDelay;
            var percent = (playerState.HeathPoints * 100) / GameMatch.DefaultPlayerHeathPoints;
            int overlayHeight = (int)((percent * spriteHeight) / total);
            if (overlayHeight % 2 != 0) overlayHeight++;

            rendererService.DrawTexturePro(GameAssets.HUD.WideBarRed, new(0, GameAssets.HUD.WideBarRed.Height - overlayHeight, GameAssets.HUD.WideBarRed.Width, overlayHeight), new((int)p.X, (int)(p.Y + GameAssets.HUD.WideBarRed.Height - overlayHeight), GameAssets.HUD.WideBarRed.Width, overlayHeight), new(0, 0), 0, System.Drawing.Color.LightGray);
        }

    }

}
