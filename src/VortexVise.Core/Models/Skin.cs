using VortexVise.Core.Interfaces;

namespace VortexVise.Core.Models;

/// <summary>
/// Define the player appearance.
/// </summary>
public class Skin
{
    /// <summary>
    /// File checksum
    /// </summary>
    public string Id { get; set; } = "";
    public ITextureAsset? Texture { get; set; }
    public string TextureLocation { get; set; } = string.Empty;
}
