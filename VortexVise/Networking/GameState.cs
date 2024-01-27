using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;
using VortexVise.Models;

namespace VortexVise.Networking;

public class GameState
{
    public double Time { get; set; }
    public Input Input { get; set; }
    public float Gravity { get; set; }
    public List<PlayerState> PlayerStates { get; set; } = [];

    public GameState(Player player, float gravity)
    {
        Input = player.GetInput();
        PlayerStates.Add(new PlayerState(player));
        Gravity = gravity;
    }
    public void SimulatePlayerState(Player player, float deltaTime, Map map)
    {
        var state = PlayerStates.Where(p => p.Id == player.Id).FirstOrDefault();
        if (state == null) throw new Exception("Can't find player");

        state.Velocity = player.ProcessVelocity(deltaTime, Input);
        state.Position = player.ProcessPosition(Gravity, deltaTime, state.Velocity);
        state.HookState = player.Hook.Simulate(player, map, Gravity, deltaTime, Input);
        player.ApplyState(state);
    }
}
