﻿using System.Numerics;
using VortexVise.Enums;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

/// <summary>
/// Game Level/Map
/// </summary>
public class Map
{
    /// <summary>
    /// Data file checksum
    /// </summary>
    public string Id { get; set; } = ""; 
    public string Name { get; set; } = "";
    public List<Rectangle> Collisions { get; set; } = new List<Rectangle>();
    public List<SerializableVector2> PlayerSpawnPoints { get; set; } = new List<SerializableVector2>();
    public List<SerializableVector2> EnemySpawnPoints { get; set; } = new List<SerializableVector2>();
    public List<SerializableVector2> ItemSpawnPoints { get; set; } = new List<SerializableVector2>();
    public List<GameMode> GameModes { get; set; } = new List<GameMode> { };
    public string TextureLocation { get; set; } = "";
    public string MapLocation { get; set; } = "";
    public string BGM { get; set; } = "";
    public string BGS { get; set; } = "";
    public Texture Texture { get; set; }
}
