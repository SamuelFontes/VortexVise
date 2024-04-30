using System.Numerics;
using VortexVise.Enums;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

/// <summary>
/// Game Level/Map
/// </summary>
public class Map
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public List<Rectangle> Collisions { get; set; } = new List<Rectangle>();
    public List<Vector2> PlayerSpawnPoints { get; set; } = new List<Vector2>();
    public List<Vector2> EnemySpawnPoints { get; set; } = new List<Vector2>();
    public List<Vector2> ItemSpawnPoints { get; set; } = new List<Vector2>();
    public List<GameMode> GameModes { get; set; } = new List<GameMode> { };
    public string TextureLocation { get; set; } = "";
    public string MapLocation { get; set; } = "";
    public string BGM { get; set; } = "";
    public string BGS { get; set; } = "";
    public Texture Texture { get; set; }
}
