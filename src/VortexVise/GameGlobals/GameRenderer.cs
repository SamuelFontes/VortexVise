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
    public static void DrawGameState(GameState state)
    {
        // All rendering logic should go here
        DrawMap();
        DrawWeaponDrops(state);
        foreach (var playerState in state.PlayerStates)
        {
            DrawHookState(playerState);
            PlayerLogic.DrawState(playerState);
        }
    }

    static void DrawMap()
    {
        Raylib.DrawTextureRec(GameAssets.Gameplay.CurrentMapTexture, new(0, 0, GameAssets.Gameplay.CurrentMapTexture.width * GameMatch.MapMirrored, GameAssets.Gameplay.CurrentMapTexture.height), new Vector2(0, 0), Raylib.WHITE);
        if (Utils.Debug())
        {
            foreach (var collision in GameMatch.MapCollisions) // DEBUG
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
        if (playerState.HookState.IsHookReleased)
        {
            Raylib.DrawLineEx(PlayerLogic.GetPlayerCenterPosition(playerState.Position), new Vector2(playerState.HookState.Position.X + 3, playerState.HookState.Position.Y + 3), 1, new Color(159, 79, 0, 255));
            Raylib.DrawTexture(GameAssets.Gameplay.HookTexture, (int)playerState.HookState.Position.X, (int)playerState.HookState.Position.Y, Raylib.WHITE);

            if (Utils.Debug())
                Raylib.DrawRectangleRec(playerState.HookState.Collision, Raylib.GREEN); // Debug
        }



    }

}
