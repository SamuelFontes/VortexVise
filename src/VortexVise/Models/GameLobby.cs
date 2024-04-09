﻿
namespace VortexVise.Models;

/// <summary>
/// Used to list game matches on server backend.
/// </summary>
public class GameLobby
{
    public Guid Id { get; set; }
    public string MatchOwner { get; set; } = string.Empty;
    public int MaxPlayers { get; set; } = 8;
    public List<string> Players { get; set; } = [];
    public double LastGameUpdate { get; set; }
}
