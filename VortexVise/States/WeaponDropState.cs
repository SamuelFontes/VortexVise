using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.States;

public class WeaponDropState
{
    public WeaponState WeaponState { get; set; }
    public float DropTimer { get; set; } = 0;
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
}
