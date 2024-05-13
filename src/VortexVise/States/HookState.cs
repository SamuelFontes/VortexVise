using System.Numerics;
using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.States;

/// <summary>
/// State for a player ninja rope/grappling hook.
/// </summary>
public class HookState
{
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public SerializableVector2 SerializablePosition { get; set; } = new SerializableVector2(0, 0);
    public SerializableVector2 SerializableVelocity { get; set; } = new SerializableVector2(0, 0);
    public Rectangle Collision { get { return new Rectangle(Position.X, Position.Y, 8, 8); } }
    public bool IsHookAttached { get; set; } = false;
    public bool IsHookReleased { get; set; } = false;
    public bool IsPressingHookKey { get; set; } = false;
}
