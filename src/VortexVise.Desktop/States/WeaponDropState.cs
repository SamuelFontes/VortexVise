using System.Numerics;
using VortexVise.Core.Models;
using VortexVise.Desktop.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.States;

/// <summary>
/// State for dropped itens.
/// </summary>
public class WeaponDropState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public WeaponDropState(WeaponState weaponState, Vector2 position)
    {
        WeaponState = weaponState;
        Position = position;
    }
    public WeaponDropState(WeaponState weaponState, float dropTimer, Vector2 position, Vector2 velocity)
    {
        WeaponState = weaponState;
        DropTimer = dropTimer;
        Position = position;
        Velocity = velocity;
    }

    public WeaponState WeaponState { get; set; }
    public float DropTimer { get; set; } = 0;
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public SerializableVector2 SerializablePosition { get; set; }
    public SerializableVector2 SerializableVelocity { get; set; }
    public Rectangle Collision { get { return new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 48, 48); } }
}
