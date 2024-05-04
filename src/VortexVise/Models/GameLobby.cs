
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
    public int MaxPlayers { get; set; } = 8;
    public List<PlayerProfile> Players { get; set; } = [];
    public double LastGameUpdate { get; set; }
    public string SelectedMapId { get; set; } = "";
}
