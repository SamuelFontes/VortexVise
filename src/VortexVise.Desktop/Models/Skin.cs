using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Models;

/// <summary>
/// Define the player appearance.
/// </summary>
public class Skin
{
    /// <summary>
    /// File checksum
    /// </summary>
    public string Id { get; set; } = "";
    public Texture Texture { get; set; }
    public string TextureLocation { get; set; } = string.Empty;
}
