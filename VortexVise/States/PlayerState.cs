using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;

namespace VortexVise.States;

public class PlayerState
{
    public Guid Id { get; set; } = new Guid();
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public int Direction { get; set; } = 1;
    public bool IsTouchingTheGround { get; set; } = false;
    public Rectangle Collision { get; set; } = new Rectangle(20, 12, 25, 45);
    public InputState Input { get; set; } = new InputState();
    public HookState HookState { get; set; } = new HookState();
    public bool IsLookingRight()
    {
        return Direction == -1;
    }
}
