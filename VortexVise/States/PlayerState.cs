using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;
using VortexVise.Utilities;

namespace VortexVise.States;

public class PlayerState
{
    public Guid Id { get; set; }
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public int Direction { get; set; } = 1;
    public bool IsTouchingTheGround { get; set; } = false;
    public Rectangle Collision { get; set; } = new Rectangle(20, 12, 25, 45);
    public InputState Input { get; set; } = new InputState();
    public HookState HookState { get; set; } = new HookState();
    public SerializableVector2 SerializablePosition { get; set; } = new SerializableVector2(new(0, 0));
    public SerializableVector2 SerializableVelocity { get; set; } = new SerializableVector2(new(0, 0));
    public SerializableRectangle SerializableCollision { get; set; } = new SerializableRectangle(new(0, 0, 0, 0));
    public PlayerState(Guid id) {  Id = id; }
    public bool IsLookingRight()
    {
        return Direction == -1;
    }
    public void PrepareSerialization()
    {
        SerializablePosition = new SerializableVector2(Position);
        SerializableVelocity = new SerializableVector2(Velocity);
        SerializableCollision = new SerializableRectangle(Collision);
    }
    public void PostSerialization()
    {
        Position = SerializablePosition.ToVector2();
        Velocity = SerializableVelocity.ToVector2();
        Collision = SerializableCollision.ToRectangle();
    }
}
