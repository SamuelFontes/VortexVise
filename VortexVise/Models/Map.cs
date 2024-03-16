using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Enums;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

public class Map
{
    public string Name { get; set; } = "";
    public List<Rectangle> Collisions { get; set; } = new List<Rectangle>();
    public List<Vector2> PlayerSpawnPoints { get; set; } = new List<Vector2>();
    public List<Vector2> EnemySpawnPoints { get; set; } = new List<Vector2>();
    public List<Vector2> ItemSpawnPoints { get; set; } = new List<Vector2>();
    public List<GameMode> GameModes { get; set; } = new List<GameMode> { };
    public string TextureLocation { get; set; } = "";
}
