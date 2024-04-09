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
    public static void DrawGameState(GameState state)
    {
        // All rendering logic should go here
        DrawMap();
        WeaponLogic.DrawWeaponDrops(state);
        foreach (var playerState in state.PlayerStates)
        {
            PlayerHookLogic.DrawState(playerState);
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
}
