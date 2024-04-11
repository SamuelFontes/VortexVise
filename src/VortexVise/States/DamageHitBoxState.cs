using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.States;

/// <summary>
/// Used to create hit boxes that can hurt entities. 
/// </summary>
public class DamageHitBoxState
{
    public DamageHitBoxState(int playerId, Rectangle hitBox, Weapon weapon, float hitBoxTimer, int direction)
    {
        PlayerId = playerId;
        HitBox = hitBox;
        Weapon = weapon;
        HitBoxTimer = hitBoxTimer;
        Direction = direction;
    }

    public int PlayerId { get; set; }
    public Rectangle HitBox { get; set; }
    public Weapon Weapon { get; set; }
    public float HitBoxTimer { get; set; }
    public int Direction { get; set; }
    public bool ShouldDisappear { get; set; } = false;
}
