using VortexVise.Core.Enums;

namespace VortexVise.Core.Models;

/// <summary>
/// Define information about the player.
/// </summary>
public class PlayerProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public GamepadSlot Gamepad { get; set; } = GamepadSlot.MouseAndKeyboard;
    public string Name { get; set; } = string.Empty;
    public Skin Skin { get; set; } = new Skin() { Id = "" };
}
