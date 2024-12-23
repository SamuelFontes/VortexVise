using System.Drawing;
using VortexVise.Core.Enums;

namespace VortexVise.Core.Models;

/// <summary>
/// Thing used to kill other things.
/// </summary>
public class Weapon
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TextureLocation { get; set; } = string.Empty;
    public string ProjectileTextureLocation { get; set; } = string.Empty;
    public Color TextureColor { get; set; } = Color.White;
    public WeaponType WeaponType { get; set; }
    public float ReloadDelay { get; set; }
    public Color Color { get; set; } = Color.White;
    public int Damage { get; set; }
    public int Knockback { get; set; }
    public int Ammo { get; set; }
    public int SelfKnockback { get; set; }
    public StatusEffects Effect { get; set; } = StatusEffects.None;
    public int EffectAmount { get; set; }
    public StatusEffects SelfEffect { get; set; } = StatusEffects.None;
    public int SelfEffectPercentageChance { get; set; }
    public int SelfEffectAmount { get; set; }
    public ITextureAsset? Texture { get; set; }
    public ITextureAsset? ProjectileTexture { get; set; }
}
