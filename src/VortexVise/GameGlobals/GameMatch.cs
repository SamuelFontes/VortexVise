﻿using System.Net.NetworkInformation;
using System.Numerics;
using VortexVise.Models;
using VortexVise.States;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

/// <summary>
/// The configuration for the current match settings.
/// </summary>
public static class GameMatch
{
    // Match Options
    //----------------------------------------------------------------------------------
    /// <summary>
    /// Time in seconds it takes a player to respawn after death.
    /// </summary>
    public static int PlayerSpawnDelay { get; set; } = 3;
    /// <summary>
    /// Time in seconds it takes between wapon spawns.
    /// </summary>
    public static int WeaponSpawnDelay { get; set; } = 8;
    /// <summary>
    /// Current loaded map.
    /// </summary>
    public static Map CurrentMap { get; set; } = new();
    /// <summary>
    /// If we want to flip the map use -1. Maybe deprecated.
    /// </summary>
    public static int MapMirrored = 1;
    /// <summary>
    /// Starting player health.
    /// </summary>
    public static int DefaultPlayerHeathPoints = 100; 
    /// <summary>
    /// Current gameplay cameras, used to render player view.
    /// </summary>
    public static List<PlayerCamera> PlayerCameras { get; set; } = [];

    // Hook Options
    //----------------------------------------------------------------------------------
    /// <summary>
    /// Size of hook head collision.
    /// </summary>
    public static int HookSize = 8;
    /// <summary>
    /// Defines how much the grappling hook pulls.
    /// </summary>
    public static readonly float HookPullForce = 3000;
    /// <summary>
    /// Define how close can a player be from the object the hook is attached to.
    /// </summary>
    public static readonly float HookPullOffset = 64;
    /// <summary>
    /// Define how fast the hook goes when shoot.
    /// </summary>
    public static readonly float HookShootForce = 1200;
    /// <summary>
    /// Define max hook length, when it reachs this size it will stop shooting.
    /// </summary>
    public static readonly float HookSizeLimit = 1000;

    // Player Options
    //----------------------------------------------------------------------------------
    /// <summary>
    /// Returns random player spawn point.
    /// </summary>
    static public Vector2 PlayerSpawnPoint { get { return CurrentMap.PlayerSpawnPoints.OrderBy(x => Guid.NewGuid()).First(); } }
    /// <summary>
    /// Max player move speed.
    /// </summary>
    public static readonly float PlayerMaxSpeed = 350;
    /// <summary>
    /// Define how high the player can jump.
    /// </summary>
    public static readonly float PlayerJumpForce = 450;
    /// <summary>
    /// Define how fast can the player fall.
    /// </summary>
    public static readonly float PlayerMaxGravity = 1000;
    /// <summary>
    /// Define how fast player can get to max speed.
    /// </summary>
    public static readonly float PlayerAcceleration = 750;
    /// <summary>
    /// This is defines when the player hitbox starts. The top left of the player sprite starts with 0,0. So this helps to heep the hitbox at the center of the player.
    /// </summary>
    public static Vector2 PlayerCollisionOffset = new(10, 6);
    /// <summary>
    /// Define how fast a granade go
    /// </summary>
    public static readonly float GranadeForce = 250;

    public static List<Bot> Bots { get; set; } = new();
    /// <summary>
    /// Current game state. Please don't mess with this outside the GameLogic. It's here to reset things when changing maps.
    /// </summary>
    public static GameState? GameState { get; set; }
}
