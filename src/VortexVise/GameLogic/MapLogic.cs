#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'. Honestly this will only run once so we don't care about performance
using System.Numerics;
using System.Text.RegularExpressions;
using VortexVise.GameGlobals;
using VortexVise.Models;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

/// <summary>
/// Handles the Map/Level logic for loading and changing maps.
/// </summary>
public static class MapLogic
{
    public static void Init()
    {
        string mapLocation = "Resources/Maps";
        // Get all files from the Resources/Maps folder to read list of avaliable game levels aka maps
        string[] mapFiles = Directory.GetFiles(mapLocation, "*.ini", SearchOption.TopDirectoryOnly);
        string[] pngFiles = Directory.GetFiles(mapLocation, "*.png", SearchOption.TopDirectoryOnly);
        foreach (var file in mapFiles)
        {
            string fileContent = File.ReadAllText(file);
            try
            {
                var map = new Map
                {
                    // Read map name
                    Name = Regex.Match(fileContent, @"(?<=NAME\s*=\s*?)[\s\S]+?(?=\s\s)").Value.Trim()
                };
                if (map.Name == null || map.Name.Length == 0) throw new Exception("Can't read map NAME");

                // Read map collisions
                var txtCollisions = Regex.Match(fileContent, @"(?<=COLLISIONS\s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesCollisions = Regex.Matches(txtCollisions, @"[\s\S]*?;");
                foreach (Match match in matchesCollisions.Cast<Match>())
                {
                    var collision = new Rectangle
                    {
                        x = float.Parse(Regex.Match(match.Value, @"[\d\.]+(?=,[\d\.]+,[\d\.]+,[\d\.]+;)").Value),
                        y = float.Parse(Regex.Match(match.Value, @"(?<=[\d\.]+,)[\d\.]+(?=,[\d\.]+,[\d\.]+;)").Value),
                        width = float.Parse(Regex.Match(match.Value, @"(?<=[\d\.]+,[\d\.]+,)[\d\.]+(?=,[\d\.]+;)").Value),
                        height = float.Parse(Regex.Match(match.Value, @"(?<=[\d\.]+,[\d\.]+,[\d\.]+,)[\d\.]+(?=;)").Value)
                    };
                    map.Collisions.Add(collision);
                }
                if (map.Collisions.Count == 0) throw new Exception("Can't read map COLLISIONS");

                // Read map player spawn
                var txtPlayerSpawn = Regex.Match(fileContent, @"(?<=PLAYER_SPAWN\s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesPlayerSpawn = Regex.Matches(txtPlayerSpawn, @"[\s\S]*?;");
                foreach (Match match in matchesPlayerSpawn.Cast<Match>())
                {
                    var spawn = new Vector2
                    {
                        X = float.Parse(Regex.Match(match.Value, @"[\d\.]+(?=,[\d\.]+;)").Value),
                        Y = float.Parse(Regex.Match(match.Value, @"(?<=[\d\.],)[\d\.]+(?=;)").Value)
                    };
                    map.PlayerSpawnPoints.Add(spawn);
                }
                if (map.PlayerSpawnPoints.Count == 0) throw new Exception("Can't read map PlayerSpawnPoints");

                // Read map enemy spawn
                var txtEnemySpawn = Regex.Match(fileContent, @"(?<=ENEMY_SPAWN\s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesEnemySpawn = Regex.Matches(txtEnemySpawn, @"[\s\S]*?;");
                foreach (Match match in matchesEnemySpawn.Cast<Match>())
                {
                    var spawn = new Vector2
                    {
                        X = float.Parse(Regex.Match(match.Value, @"[\d\.]+(?=,[\d\.]+;)").Value),
                        Y = float.Parse(Regex.Match(match.Value, @"(?<=[\d\.]+,)[\d\.]+(?=;)").Value)
                    };
                    map.EnemySpawnPoints.Add(spawn);
                }
                if (map.EnemySpawnPoints.Count == 0) throw new Exception("Can't read map EnemySpawnPoints");

                // Read map item spawn
                var txtItemSpawn = Regex.Match(fileContent, @"(?<=ITEM_SPAWN\s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesItemSpawn = Regex.Matches(txtItemSpawn, @"[\s\S]*?;");
                foreach (Match match in matchesItemSpawn.Cast<Match>())
                {
                    var spawn = new Vector2
                    {
                        X = float.Parse(Regex.Match(match.Value, @"[\d\.]+(?=,[\d\.]+;)").Value),
                        Y = float.Parse(Regex.Match(match.Value, @"(?<=[\d\.]+,)[\d\.]+(?=;)").Value)
                    };
                    map.ItemSpawnPoints.Add(spawn);
                }
                if (map.ItemSpawnPoints.Count == 0) throw new Exception("Can't read map ItemSpawnPoints");

                // Read map gamemodes
                var txtGameModes = Regex.Match(fileContent, @"(?<=GAME_MODES\s*=[\S,\s]*?).*").Value;
                if (txtGameModes.Contains("DM"))
                {
                    map.GameModes.Add(Enums.GameMode.DeathMatch);
                    map.GameModes.Add(Enums.GameMode.TeamDeathMatch);
                }
                if (txtGameModes.Contains("SURVIVAL"))
                    map.GameModes.Add(Enums.GameMode.Survival);
                if (map.GameModes.Count == 0) throw new Exception("Can't read map GAME_MODES");

                // Check for image file
                var mapFileName = Regex.Match(file, @"[\s\S]+(?=\.ini)").Value;
                mapFileName += ".png";
                if (!pngFiles.Contains(mapFileName)) throw new Exception($"Can't find image file {mapFileName}");
                map.TextureLocation = mapFileName;
                map.MapLocation = file;

                GameAssets.Gameplay.Maps.Add(map);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading map {file}: {ex.Message}");
            }
        }
        if (GameAssets.Gameplay.Maps.Count == 0) throw new Exception("Can't find any map");
    }

    public static void Unload()
    {
        Raylib.UnloadTexture(GameAssets.Gameplay.CurrentMapTexture);

    }

    public static void LoadRandomMap()
    {
        if (GameMatch.CurrentMap != null) Raylib.UnloadTexture(GameAssets.Gameplay.CurrentMapTexture);
        GameMatch.CurrentMap = GameAssets.Gameplay.Maps.OrderBy(x => Guid.NewGuid()).First();
        GameAssets.Gameplay.CurrentMapTexture = Raylib.LoadTexture(GameMatch.CurrentMap.TextureLocation);

        // Mirror map random
        var random = new Random().Next(2);
        if (random == 0)
        {
            // Regular Map
            GameMatch.MapMirrored = 1;
            GameMatch.MapCollisions = GameMatch.CurrentMap.Collisions;
        }
        else
        {
            // Inverted map
            var collisions = GameMatch.CurrentMap.Collisions;
            var mirroredCollisions = new List<Rectangle>();
            GameMatch.MapMirrored = -1;
            foreach (var collision in collisions)
            {
                var mirroredCollision = new Rectangle
                {
                    X = GameAssets.Gameplay.CurrentMapTexture.width - collision.x - collision.width,
                    Y = collision.Y,
                    width = collision.width,
                    height = collision.height
                };
                mirroredCollisions.Add(mirroredCollision);
            }
            GameMatch.MapCollisions = mirroredCollisions;
        }

    }


    public static List<Rectangle> GetCollisions()
    {
        return GameMatch.MapCollisions;
    }

    public static Vector2 GetMapSize()
    {
        return new Vector2(GameAssets.Gameplay.CurrentMapTexture.width, GameAssets.Gameplay.CurrentMapTexture.height);
    }

}
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
