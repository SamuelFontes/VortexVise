using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Logic;
using VortexVise.States;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

/// <summary>
/// This will handle all game rendering
/// </summary>
public static class GameRenderer
{
    /// <summary>
    /// Render game state to the screen.
    /// </summary>
    /// <param name="state">Current game state.</param>
    public static void DrawGameState(GameState state, PlayerState mainPlayer)
    {
        // All rendering logic should go here
        DrawMap();
        DrawWeaponDrops(state);
        foreach (var playerState in state.PlayerStates)
        {
            if (playerState.Id == mainPlayer.Id) continue;

            DrawHookState(playerState);
            DrawPlayerState(playerState);
        }
        // Draw main player on top for this screen
        DrawHookState(mainPlayer);
        DrawPlayerState(mainPlayer);
        DrawProjectiles(state);
    }

    private static void DrawProjectiles(GameState state)
    {
        foreach (var hitbox in state.DamageHitBoxes)
        {
            if (hitbox.Weapon.ProjectileTextureLocation == string.Empty) continue;
            Rectangle sourceRec = new(0.0f, 0.0f, hitbox.Weapon.ProjectileTexture.width, hitbox.Weapon.ProjectileTexture.height);

            Rectangle destRec = new(hitbox.HitBox.X + hitbox.HitBox.width * 0.5f, hitbox.HitBox.Y + hitbox.HitBox.height * 0.5f, hitbox.Weapon.ProjectileTexture.width, hitbox.Weapon.ProjectileTexture.height);

            Raylib.DrawTexturePro(hitbox.Weapon.ProjectileTexture, sourceRec, destRec, new(hitbox.Weapon.ProjectileTexture.width * 0.5f, hitbox.Weapon.ProjectileTexture.height * 0.5f), (int)WeaponLogic.WeaponRotation, Raylib.WHITE);
        }

    }

    static void DrawMap()
    {
        Raylib.DrawTextureRec(GameAssets.Gameplay.CurrentMapTexture, new(0, 0, GameAssets.Gameplay.CurrentMapTexture.width * GameMatch.MapMirrored, GameAssets.Gameplay.CurrentMapTexture.height), new Vector2(0, 0), Raylib.WHITE);
        if (Utils.Debug())
        {
            foreach (var collision in GameMatch.CurrentMap.Collisions) // DEBUG
            {
                Raylib.DrawRectangleRec(collision, Raylib.BLUE);
            }
        }

    }

    static void DrawWeaponDrops(GameState currentGameState)
    {
        foreach (var drop in currentGameState.WeaponDrops)
        {
            Rectangle sourceRec = new(0.0f, 0.0f, drop.WeaponState.Weapon.Texture.width, drop.WeaponState.Weapon.Texture.height);

            Rectangle destRec = new(drop.Position.X + drop.WeaponState.Weapon.Texture.width * 0.5f, drop.Position.Y + drop.WeaponState.Weapon.Texture.height * 0.5f, drop.WeaponState.Weapon.Texture.width, drop.WeaponState.Weapon.Texture.height);

            Raylib.DrawTexturePro(drop.WeaponState.Weapon.Texture, sourceRec, destRec, new(drop.WeaponState.Weapon.Texture.width * 0.5f, drop.WeaponState.Weapon.Texture.height * 0.5f), (int)WeaponLogic.WeaponRotation, Raylib.WHITE);

            if (Utils.Debug()) Raylib.DrawRectangleRec(drop.Collision, Raylib.PURPLE); // Debug
        }
        if (Utils.Debug()) foreach (var h in currentGameState.DamageHitBoxes) Raylib.DrawRectangleRec(h.HitBox, Raylib.RED); // Debug
    }

    static void DrawHookState(PlayerState playerState)
    {
        if (playerState.IsDead) return;
        if (playerState.HookState.IsHookReleased)
        {
            Raylib.DrawLineEx(PlayerLogic.GetPlayerCenterPosition(playerState.Position), new Vector2(playerState.HookState.Position.X + 3, playerState.HookState.Position.Y + 3), 1, new Color(159, 79, 0, 255));
            Raylib.DrawTexture(GameAssets.Gameplay.HookTexture, (int)playerState.HookState.Position.X, (int)playerState.HookState.Position.Y, Raylib.WHITE);

            if (Utils.Debug())
                Raylib.DrawRectangleRec(playerState.HookState.Collision, Raylib.GREEN); // Debug
        }



    }
    static void DrawPlayerState(PlayerState playerState)
    {
        if (playerState.IsDead) return;
        Rectangle sourceRec = new(0.0f, 0.0f, playerState.Skin.Texture.width * playerState.Direction, playerState.Skin.Texture.height);

        Rectangle destRec = new(playerState.Position.X + playerState.Skin.Texture.width * 0.5f, playerState.Position.Y + playerState.Skin.Texture.height * 0.5f, playerState.Skin.Texture.width, playerState.Skin.Texture.height);

        var rotation = playerState.Animation.GetAnimationRotation();
        if (rotation != 0) destRec.Y -= 2f; // this adds a little bump to the walking animation

        Raylib.DrawTexturePro(playerState.Skin.Texture, sourceRec, destRec, new Vector2(playerState.Skin.Texture.width * 0.5f, playerState.Skin.Texture.height * 0.5f), rotation, Raylib.WHITE); // Draw Player 

        var weapon = playerState.WeaponStates.FirstOrDefault(x => x.IsEquipped);
        if (weapon != null)
        {
            switch (weapon.Weapon.WeaponType)
            {
                case Enums.WeaponType.Shotgun: destRec.Y += 5; break;
                case Enums.WeaponType.SMG: destRec.Y += 5; break;
                case Enums.WeaponType.Pistol: destRec.Y += 5; break;
                case Enums.WeaponType.MeleeBlunt:
                {
                    if (weapon.ReloadTimer >= weapon.Weapon.ReloadDelay * 0.2f)
                    {
                        destRec.X += 5 * playerState.Direction;
                        sourceRec.Width = -sourceRec.width;
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
                case Enums.WeaponType.MeleeCut:
                {
                    if (weapon.ReloadTimer >= weapon.Weapon.ReloadDelay * 0.2f)
                    {
                        destRec.X += 5 * playerState.Direction;
                        sourceRec.Width = -sourceRec.width;
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
            Raylib.DrawTexturePro(weapon.Weapon.Texture, sourceRec, destRec, new Vector2(playerState.Skin.Texture.width * 0.5f, playerState.Skin.Texture.height * 0.5f), rotation, Raylib.WHITE); // Draw Player 
        }

        if (Utils.Debug())
        {
            Raylib.DrawRectangleRec(playerState.Collision, Raylib.GREEN); // Debug
        }
    }


}
