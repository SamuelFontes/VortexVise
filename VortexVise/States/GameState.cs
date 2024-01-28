using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;

namespace VortexVise.States;

public class GameState
{
    public double Time { get; set; }
    public InputState Input { get; set; }
    public float Gravity { get; set; }
    public List<PlayerState> PlayerStates { get; set; } = [];

    public GameState(PlayerLogic player, float gravity)
    {
        Input = player.GetInput();
        PlayerStates.Add(new PlayerState(player));
        Gravity = gravity;
    }
    public void SimulatePlayerState(PlayerLogic player, float deltaTime, MapLogic map)
    {
        var state = PlayerStates.Where(p => p.Id == player.Id).FirstOrDefault();
        if (state == null) throw new Exception("Can't find player");

        state.Velocity = player.ProcessVelocity(deltaTime, Input);
        state.Position = player.ProcessPosition(Gravity, deltaTime, state.Velocity);
        state.HookState = player.Hook.Simulate(player, map, Gravity, deltaTime, Input);
        player.ApplyState(state);
    }
}
