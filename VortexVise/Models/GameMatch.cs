
namespace VortexVise.Models;

public class GameMatch
{
    public Guid Id { get; set; }
    public string MatchOwner { get; set; }
    public int MaxPlayers { get; set; }
    public List<string> Players { get; set; }
    public double LastGameUpdate { get; set; }
}
