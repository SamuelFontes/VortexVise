using ZeroElectric.Vinculum;

namespace VortexVise.Models;

/// <summary>
/// Define the player appearance.
/// </summary>
public class Skin
{
    public int Id { get; set; }
    public Texture Texture { get; set; }
    public string TextureLocation { get; set; } = string.Empty;
}
