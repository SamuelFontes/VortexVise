using VortexVise.Enums;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

/// <summary>
/// Thing used to kill other things.
/// </summary>
public class Weapon
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TextureLocation { get; set; }
    public Color TextureColor { get; set; } = Raylib.WHITE;
    public WeaponType WeaponType { get; set; }
    public float ReloadDelay { get; set; }
    public Color Color { get; set; } = Raylib.WHITE; // This is for the gun and the projectile
    public int Damage { get; set; }
    public int Knockback { get; set; }
    public int SelfKnockback { get; set; }
    public StatusEffects Effect { get; set; } = StatusEffects.None;
    public int EffectAmount { get; set; }
    public StatusEffects SelfEffect { get; set; } = StatusEffects.None;
    public int SelfEffectPercentageChance { get; set; }
    public int SelfEffectAmount { get; set; }
    public Texture Texture { get; set; }
}
