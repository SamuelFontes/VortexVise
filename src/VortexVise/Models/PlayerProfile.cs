namespace VortexVise.Models;

/// <summary>
/// Define information about the player.
/// </summary>
public class PlayerProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SkinLocation { get; set; } = string.Empty;
    // TODO: add more profile stuff here
}
