using System.Numerics;
using VortexVise.Core.Models;

namespace VortexVise.Desktop.States;

/// <summary>
/// State for a player ninja rope/grappling hook.
/// </summary>
public class HookState
{
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public SerializableVector2 SerializablePosition { get; set; } = new SerializableVector2(0, 0);
    public SerializableVector2 SerializableVelocity { get; set; } = new SerializableVector2(0, 0);
    public System.Drawing.Rectangle Collision { get { return new System.Drawing.Rectangle((int)Position.X, (int)Position.Y, 8, 8); } }
    public bool IsHookAttached { get; set; } = false;
    public bool IsHookReleased { get; set; } = false;
    public bool IsPressingHookKey { get; set; } = false;
}
