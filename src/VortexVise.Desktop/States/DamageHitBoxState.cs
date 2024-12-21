using System.Numerics;
using VortexVise.Desktop.GameGlobals;
using VortexVise.Desktop.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.States;

/// <summary>
/// Used to create hit boxes that can hurt entities. 
/// </summary>
public class DamageHitBoxState
{
    public DamageHitBoxState(Guid playerId, Rectangle hitBox, Weapon weapon, float hitBoxTimer, int direction, Vector2 velocity, bool shouldColide, WeaponState weaponState)
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

    public Guid PlayerId { get; set; }
    public Rectangle HitBox { get; set; }
    public Vector2 Velocity { get; set; }
    public SerializableVector2 SerializableVelocity { get; set; }
    public Weapon Weapon { get; set; }
    public WeaponState WeaponState { get; set; }
    public float HitBoxTimer { get; set; }
    public int Direction { get; set; }
    public bool ShouldDisappear { get; set; } = false;
    public bool ShouldColide { get; set; } = false;
    public bool IsExplosion { get; set; } = false;

    /// <summary>
    /// Make the thing explode
    /// </summary>
    /// <param name="gameState">Current Game State</param>
    public void Explode(GameState gameState)
    {
        if (IsExplosion) return;
        HitBox = new(HitBox.X - 48, HitBox.Y - 48, HitBox.Width + 96, HitBox.Height + 96);
        Velocity = new(0, 0);
        ShouldColide = false;
        HitBoxTimer = 0.2f;
        ShouldDisappear = false;
        IsExplosion = true;
        gameState.Animations.Add(new() { Animation = GameAssets.Animations.Explosion, Position = new(HitBox.X, HitBox.Y) });
        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Explosion);
    }
}
