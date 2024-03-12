using System.Numerics;
using VortexVise.Logic;
using ZeroElectric.Vinculum;

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
    public AnimationState Animation { get; set; } = new AnimationState();
    public PlayerState(Guid id)
    {
        Id = id;
        Position = PlayerLogic.SpawnPoint;
    }
    public bool IsLookingRight()
    {
        return Direction == -1;
    }
    public void AddVelocity(Vector2 velocity)
    {
        Velocity += new Vector2(velocity.X, velocity.Y);
    }

    public void AddVelocityWithDeltaTime(Vector2 velocity, float deltaTime)
    {
        Velocity += new Vector2(velocity.X * deltaTime, velocity.Y * deltaTime);
    }
    public void ResetVelocity()
    {
        Velocity = Vector2.Zero;
    }

}
