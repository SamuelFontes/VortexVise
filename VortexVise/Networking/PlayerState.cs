using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;

namespace VortexVise.Networking;

public class PlayerState
{
    public Guid Id { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public HookState HookState { get; set; }
    public PlayerState(Player player)
    {
        Id = player.Id;
        Position = player.GetPosition();
        Velocity = player.GetVelocity();
    }

}
