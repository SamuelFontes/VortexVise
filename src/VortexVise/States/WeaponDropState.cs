﻿using System.Numerics;
using ZeroElectric.Vinculum;

namespace VortexVise.States;

/// <summary>
/// State for dropped itens.
/// </summary>
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
    public Rectangle Collision { get { return new Rectangle((int)Position.X, (int)Position.Y, 32, 32); } }
}