
using VortexVise.Core.Enums;

namespace VortexVise.Models;

/// <summary>
/// Used to list game matches on server backend.
/// </summary>
public class GameLobby
{
    public GameLobby(PlayerProfile owner)
    {
        Owner = owner;
        Players.Add(Owner);
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public PlayerProfile Owner { get; set; }
    public List<PlayerProfile> Players { get; set; } = [];
    public MatchStates MatchState { get; set; } = MatchStates.Lobby;
    public Map CurrentMap { get; set; }
    public GameMode CurrentGameMode { get; set; } = GameMode.DeathMatch;
    public int MaxPlayers { get; set; } = 8;
    public double LastGameUpdate { get; set; }
}
