using System.Numerics;
using System.Text.RegularExpressions;
using VortexVise.Models;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Logic;

public static class MapLogic
{
    static List<Map> Maps { get; set; } = new List<Map>();
    static string _mapName;
    static string _mapDescription;
    static string _texturePath;
    public static Texture2D _mapTexture; // This is the whole map baked into an image
    static List<Rectangle> _collisions = new List<Rectangle>();

    public static void Init()
    {
        // Get all files from the Resources/Maps folder to read list of avaliable game levels aka maps
        string[] mapFiles = Directory.GetFiles("Resources/Maps", "*.ini", SearchOption.TopDirectoryOnly);
        string[] pngFiles = Directory.GetFiles("Resources/Maps", "*.png", SearchOption.TopDirectoryOnly);
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
                var mapFileName = Regex.Match(fileContent, @"[\s\w\d]+(?=\.ini)").Value;
                mapFileName += ".png";
                if(!pngFiles.Contains(mapFileName)) throw new Exception($"Can't find image file {mapFileName}");
                map.TextureLocation = mapFileName;

                Maps.Add(map);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading map {file}: {ex.Message}");
            }
        }
    }

    public static void LoadMap(string mapName, bool isServer)
    {
        // TODO: Load collisions and image from a file
        string mapFolder = "Resources/Sprites/Maps/";
        if (!isServer)
            _mapTexture = Raylib.LoadTexture(mapFolder + mapName + ".png");
        else
            _mapTexture = new Texture2D() { height = 2048, width = 2048 }; // TODO: Get the map size from some place


        // TODO: Load from a json or some shit like that
        _collisions.Clear();
        _collisions.Add(new Rectangle(85, 343, 245, 16));
        _collisions.Add(new Rectangle(13, 448, 38, 16));
        _collisions.Add(new Rectangle(54, 466, 179, 16));
        _collisions.Add(new Rectangle(240, 460, 66, 16));
        _collisions.Add(new Rectangle(305, 501, 71, 16));
        _collisions.Add(new Rectangle(361, 297, 74, 16));
        _collisions.Add(new Rectangle(442, 308, 135, 16));
        _collisions.Add(new Rectangle(570, 149, 252, 16));
        _collisions.Add(new Rectangle(745, 352, 226, 16));
        _collisions.Add(new Rectangle(670, 456, 347, 16));
        _collisions.Add(new Rectangle(415, 690, 258, 16));
        _collisions.Add(new Rectangle(190, 696, 64, 16));
        _collisions.Add(new Rectangle(252, 728, 39, 16));
        _collisions.Add(new Rectangle(111, 813, 281, 16));
        _collisions.Add(new Rectangle(337, 855, 61, 16));
        _collisions.Add(new Rectangle(771, 682, 92, 16));
        _collisions.Add(new Rectangle(712, 806, 129, 16));
        _collisions.Add(new Rectangle(843, 819, 85, 16));
        _collisions.Add(new Rectangle(609, 854, 128, 16));
    }

    public static void Draw()
    {
        Raylib.DrawTextureEx(_mapTexture, new Vector2(0, 0), 0, 1, Raylib.WHITE);
        if (Utils.Debug())
        {
            foreach (var collision in _collisions) // DEBUG
            {
                Raylib.DrawRectangleRec(collision, Raylib.BLUE);
            }
        }

    }

    public static List<Rectangle> GetCollisions()
    {
        return _collisions;
    }

    public static Vector2 GetMapSize()
    {
        return new Vector2(_mapTexture.width, _mapTexture.height);
    }

}
