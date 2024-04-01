
namespace VortexVise.Models;

public class GameMatch
{
    public Guid Id { get; set; }
    public string MatchOwner { get; set; }
    public int MaxPlayers { get; set; } = 8;
    public List<string> Players { get; set; }
    public double LastGameUpdate { get; set; }
}
