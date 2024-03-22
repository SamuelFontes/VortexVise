using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Enums;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

public class Weapon
{
    public string Name { get; set; }
    public string TextureLocation { get; set; }
    public Color TextureColor { get; set; } = Raylib.WHITE;
    public WeaponType WeaponType { get; set; }
    public float ReloadDelay { get; set; }
    public Color ProjectileColor { get; set; } = Raylib.WHITE;
    public int Damage { get; set; }
    public int Knockback { get; set; }
    public int SelfKnockback { get; set; }
    public StatusEffects Effect { get; set; }
    public int EffectAmount { get; set; }
    public StatusEffects SelfEffect { get; set; }
    public int SelfEffectChancePercentage { get; set; }
    public int SelfEffectAmount { get; set; }
}
