namespace VortexVise.Desktop.Models;

/// <summary>
/// Define information about the player.
/// </summary>
public class PlayerProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Gamepad { get; set; } = -9;
    public string Name { get; set; } = string.Empty;
    public Skin Skin { get; set; } = new Skin() { Id = "" };
}
