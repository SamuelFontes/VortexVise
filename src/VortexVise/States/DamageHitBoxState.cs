using System.Numerics;
using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.States;

/// <summary>
/// Used to create hit boxes that can hurt entities. 
/// </summary>
public class DamageHitBoxState
{
    public DamageHitBoxState(int playerId, Rectangle hitBox, Weapon weapon, float hitBoxTimer, int direction, Vector2 velocity, bool shouldColide, WeaponState weaponState)
    {
        PlayerId = playerId;
        HitBox = hitBox;
        Weapon = weapon;
        HitBoxTimer = hitBoxTimer;
        Direction = direction;
        Velocity = velocity;
        ShouldColide = shouldColide;
        WeaponState = weaponState;
    }

    public int PlayerId { get; set; }
    public Rectangle HitBox { get; set; }
    public Vector2 Velocity { get; set; }
    public Weapon Weapon { get; set; }
    public WeaponState WeaponState { get; set; }
    public float HitBoxTimer { get; set; }
    public int Direction { get; set; }
    public bool ShouldDisappear { get; set; } = false;
    public bool ShouldColide { get; set; } = false;
    public bool IsExplosion { get; set; } = false;
    public void Explode()
    {
        if (IsExplosion) return;
        HitBox = new(HitBox.X - 64, HitBox.Y - 64, HitBox.Width + 128, HitBox.Height + 128);
        Velocity = new(0, 0);
        ShouldColide = false;
        HitBoxTimer = 0.2f;
        ShouldDisappear = false;
        IsExplosion = true;
    }
}
