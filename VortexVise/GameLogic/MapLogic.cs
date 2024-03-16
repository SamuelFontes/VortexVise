using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Text.RegularExpressions;
using VortexVise.Models;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

public static class MapLogic
{
    public static List<Map> Maps { get; set; } = new List<Map>();
    public static Texture2D MapTexture; // This is the whole map baked into an image
    public static List<Rectangle> MapCollisions = new List<Rectangle>();
    public static Map CurrentMap { get; set; }

    static string mapLocation = "Resources/Maps";
    static int mapMirrored = 1;

    public static void Init()
    {
        // Get all files from the Resources/Maps folder to read list of avaliable game levels aka maps
        string[] mapFiles = Directory.GetFiles(mapLocation, "*.ini", SearchOption.TopDirectoryOnly);
        string[] pngFiles = Directory.GetFiles(mapLocation, "*.png", SearchOption.TopDirectoryOnly);
        foreach (var file in mapFiles)
        {
            string fileContent = File.ReadAllText(file);
            try
            {
                var map = new Map();

                // Read map name
                map.Name = Regex.Match(fileContent, @"(?<=NAME\s*=\s*?)[\s\S]+?(?=\s\s)").Value.Trim();
                if (map.Name == null || map.Name.Length == 0) throw new Exception("Can't read map NAME");

                // Read map collisions
                var txtCollisions = Regex.Match(fileContent, @"(?<=COLLISIONS \s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesCollisions = Regex.Matches(txtCollisions, @"[\s\S]*?;");
                foreach (Match match in matchesCollisions)
                {
                    var collision = new Rectangle();
                    collision.x = float.Parse(Regex.Match(match.Value, @"\d+(?=,\d+,\d+,\d+;)").Value);
                    collision.y = float.Parse(Regex.Match(match.Value, @"(?<=\d+,)\d+(?=,\d+,\d+;)").Value);
                    collision.width = float.Parse(Regex.Match(match.Value, @"(?<=\d+,\d+,)\d+(?=,\d+;)").Value);
                    collision.height = float.Parse(Regex.Match(match.Value, @"(?<=\d+,\d+,\d+,)\d+(?=;)").Value);
                    map.Collisions.Add(collision);
                }
                if (map.Collisions.Count == 0) throw new Exception("Can't read map COLLISIONS");

                // Read map player spawn
                var txtPlayerSpawn = Regex.Match(fileContent, @"(?<=PLAYER_SPAWN\s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesPlayerSpawn = Regex.Matches(txtPlayerSpawn, @"[\s\S]*?;");
                foreach (Match match in matchesPlayerSpawn)
                {
                    var spawn = new Vector2();
                    spawn.X = float.Parse(Regex.Match(match.Value, @"\d+(?=,\d+;)").Value);
                    spawn.Y = float.Parse(Regex.Match(match.Value, @"(?<=\d+,)\d+(?=;)").Value);
                    map.PlayerSpawnPoints.Add(spawn);
                }
                if (map.PlayerSpawnPoints.Count == 0) throw new Exception("Can't read map PlayerSpawnPoints");

                // Read map enemy spawn
                var txtEnemySpawn = Regex.Match(fileContent, @"(?<=ENEMY_SPAWN\s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesEnemySpawn = Regex.Matches(txtEnemySpawn, @"[\s\S]*?;");
                foreach (Match match in matchesEnemySpawn)
                {
                    var spawn = new Vector2();
                    spawn.X = float.Parse(Regex.Match(match.Value, @"\d+(?=,\d+;)").Value);
                    spawn.Y = float.Parse(Regex.Match(match.Value, @"(?<=\d+,)\d+(?=;)").Value);
                    map.EnemySpawnPoints.Add(spawn);
                }
                if (map.EnemySpawnPoints.Count == 0) throw new Exception("Can't read map EnemySpawnPoints");

                // Read map item spawn
                var txtItemSpawn = Regex.Match(fileContent, @"(?<=ITEM_SPAWN\s*=\s*?)[\s\S]+?;(?=\s\s)").Value;
                var matchesItemSpawn = Regex.Matches(txtItemSpawn, @"[\s\S]*?;");
                foreach (Match match in matchesItemSpawn)
                {
                    var spawn = new Vector2();
                    spawn.X = float.Parse(Regex.Match(match.Value, @"\d+(?=,\d+;)").Value);
                    spawn.Y = float.Parse(Regex.Match(match.Value, @"(?<=\d+,)\d+(?=;)").Value);
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

                Maps.Add(map);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading map {file}: {ex.Message}");
                if (Maps.Count == 0) throw new Exception("Can't find any map");
            }
        }
    }

    public static void LoadRandomMap()
    {
        CurrentMap = Maps.OrderBy(x => Guid.NewGuid()).First();
        MapTexture = Raylib.LoadTexture(CurrentMap.TextureLocation);

        // Mirror map random
        var random = new Random().Next(2);
        if (random == 0)
        {
            // Regular Map
            mapMirrored = 1;
            MapCollisions = CurrentMap.Collisions;
        }
        else
        {
            // Inverted map
            var collisions = CurrentMap.Collisions;
            var mirroredCollisions = new List<Rectangle>();
            mapMirrored = -1;
            foreach(var collision in collisions)
            {
                var mirroredCollision = new Rectangle();
                mirroredCollision.X = MapTexture.width - collision.x - collision.width;
                mirroredCollision.Y = collision.Y;
                mirroredCollision.width = collision.width;
                mirroredCollision.height = collision.height;
                mirroredCollisions.Add(mirroredCollision);
            }
            MapCollisions = mirroredCollisions;
        }

    }

    public static void Draw()
    {
        Raylib.DrawTextureRec(MapTexture, new(0, 0, MapTexture.width * mapMirrored, MapTexture.height), new Vector2(0, 0), Raylib.WHITE);
        if (Utils.Debug())
        {
            foreach (var collision in MapCollisions) // DEBUG
            {
                Raylib.DrawRectangleRec(collision, Raylib.BLUE);
            }
        }

    }

    public static List<Rectangle> GetCollisions()
    {
        return MapCollisions;
    }

    public static Vector2 GetMapSize()
    {
        return new Vector2(MapTexture.width, MapTexture.height);
    }

}
