#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'. Honestly this will only run once so we don't care about performance
using System.Numerics;
using VortexVise.Core.Enums;
using VortexVise.Desktop.GameGlobals;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.Scenes;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Logic;

/// <summary>
/// Handles the Map/Level logic for loading and changing maps.
/// </summary>
public static class MapLogic
{

    public static void LoadMap(Map map)
    {
        //if (shouldStopMusic) GameAssets.MusicAndAmbience.StopMusic(); 
        // FIXME: when in gameplay it should stop the music

        GameMatch.CurrentMap = map;

        // Mirror map
        if (GameMatch.MapMirrored == -1)
        {
            // Inverted map
            var collisions = GameMatch.CurrentMap.Collisions;
            var mirroredCollisions = new List<Rectangle>();
            foreach (var collision in collisions)
            {
                var mirroredCollision = new Rectangle
                {
                    X = GameMatch.CurrentMap.Texture.width - collision.x - collision.width,
                    Y = collision.Y,
                    width = collision.width,
                    height = collision.height
                };
                mirroredCollisions.Add(mirroredCollision);
            }
            //FIXME: this shouldn't replace the map collisions
            //GameMatch.CurrentMap.Collisions = mirroredCollisions;
        }

        GameMatch.GameState?.ResetGameState();
    }

    public static void LoadRandomMap()
    {
        var map = GameAssets.Gameplay.Maps.OrderBy(x => Guid.NewGuid()).First();
        LoadMap(map);
    }

    public static void LoadNextMap(SceneManager sceneManager)
    {
        var map = GameAssets.Gameplay.Maps.SkipWhile(x => x.Id != GameMatch.CurrentMap.Id).Skip(1).DefaultIfEmpty(GameAssets.Gameplay.Maps.First()).First();
        LoadMap(map);
    }
    public static void LoadPreviousMap(SceneManager sceneManager)
    {
        var map = GameAssets.Gameplay.Maps.TakeWhile(x => x.Id != GameMatch.CurrentMap.Id).DefaultIfEmpty(GameAssets.Gameplay.Maps.Last()).Last();
        LoadMap(map);
    }


    public static List<Rectangle> GetCollisions()
    {
        return GameMatch.CurrentMap.Collisions;
    }

    public static Vector2 GetMapSize()
    {
        return new Vector2(GameMatch.CurrentMap.Texture.width, GameMatch.CurrentMap.Texture.height);
    }

}
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
