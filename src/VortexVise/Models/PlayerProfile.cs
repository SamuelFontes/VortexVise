namespace VortexVise.Models;

/// <summary>
/// Define information about the player.
/// </summary>
public class PlayerProfile
{
    public int Id { get; set; }
    public int Gamepad { get; set; } = -9;
    public string Name { get; set; } = string.Empty;
    public Skin Skin { get; set; } = new Skin() { Id = -1 };
}
