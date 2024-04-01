using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.States;

public class WeaponDropState
{
    public WeaponDropState()
    {
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
}
