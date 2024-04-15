using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.States;

/// <summary>
/// Define a player state in the simulated frame/tick.
/// </summary>
public class PlayerState
{
    public int Id { get; set; }
    public Vector2 Position { get; set; } = new Vector2(0, 0);
    public Vector2 Velocity { get; set; } = new Vector2(0, 0);
    public int Direction { get; set; } = 1;
    public bool IsTouchingTheGround { get; set; } = false;
    public bool CanDash { get; set; } = true;
    public float TimeSinceJump  { get; set; }= 0;
    public int HeathPoints = 100;
    public bool IsDead { get; set; } = false;
    public float SpawnTimer { get; set; } = 0;
    public float DamagedTimer { get; set; } = 0;
    public bool IsBot { get; set; } = false;
    public Skin Skin { get; set; }
    public PlayerStats Stats { get; set; }
    public Rectangle Collision { get; set; } = new Rectangle(20, 12, 25, 45);

    public InputState Input { get; set; } = new InputState();
    public HookState HookState { get; set; } = new HookState();
    public AnimationState Animation { get; set; } = new AnimationState();
    public List<WeaponState> WeaponStates { get; set; } = new List<WeaponState>();

    public PlayerState(int id, Skin skin)
    {
        Id = id;
        Position = GameMatch.PlayerSpawnPoint;
        Skin = skin;
        Stats = new PlayerStats() { PlayerId = id };
    }
    public bool IsLookingRight()
    {
        return Direction == -1;
    }

    public void SetVelocityX(float newVelocityX)
    {
        Velocity = new(newVelocityX, Velocity.Y);
    }

    public void SetVelocityY(float newVelocityY)
    {
        Velocity = new(Velocity.X, newVelocityY);
    }

    public void AddVelocity(Vector2 velocity)
    {
        Velocity += velocity;
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
