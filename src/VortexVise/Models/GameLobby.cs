
namespace VortexVise.Models;

/// <summary>
/// Used to list game matches on server backend.
/// </summary>
public class GameLobby
{
    public GameLobby(Guid ownerId)
    {

    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; } 
    public int MaxPlayers { get; set; } = 8;
    public List<PlayerProfile> Players { get; set; } = [];
    public double LastGameUpdate { get; set; }
}
