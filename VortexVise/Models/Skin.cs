using ZeroElectric.Vinculum;

namespace VortexVise.Models;

/// <summary>
/// Define the player apperance.
/// </summary>
public class Skin
{
    public Guid Id { get; set; }
    public Texture Texture { get; set; }
    public string TextureLocation { get; set; }
}
