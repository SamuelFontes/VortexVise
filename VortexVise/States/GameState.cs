namespace VortexVise.States;

public class GameState
{
    public double CurrentTime { get; set; }
    public float Gravity { get; set; }
    public List<PlayerState> PlayerStates { get; set; } = [];

    public string Serialize()
    {
        // This will serialize the state to send over udp every frame
        // Why not JSON serialization? AOT

        string serializedState = "";

        // this will not be a json serialization, it will be a crappy specific bullshit accumulator that I will parse with regex, deal with it
        serializedState += "|CT" + CurrentTime.ToString();
        serializedState += "|G" + Gravity.ToString();

        serializedState += "|PC" + PlayerStates.Count;
        var counter = 0;

        Func<bool,int> bn = b => b ? 1 : 0; // converts bool to 1 or 0
        // Collisions are not serialized, they will use the position and the size on the logic to deserialize

        foreach (PlayerState state in PlayerStates)
        {
            serializedState += $"|P{counter}|ID{state.Id}|PX{state.Position.X}|PY{state.Position.Y}|VX{state.Velocity.X}|VY{state.Velocity.Y}|D{state.Direction}|TG{bn(state.IsTouchingTheGround)}|IL{bn(state.Input.Left)}|IR{bn(state.Input.Right)}|IU{bn(state.Input.Up)}|ID{bn(state.Input.Down)}|IH{bn(state.Input.Hook)}|IC{bn(state.Input.CancelHook)}|IR{bn(state.Input.Right)}|HPX{state.HookState.Position.X}|HPY{state.HookState.Position.Y}|HVX{state.HookState.Velocity.X}|HVY{state.HookState.Velocity.Y}|HA{bn(state.HookState.IsHookAttached)}|HR{bn(state.HookState.IsHookReleased)}|HPR{bn(state.HookState.IsPressingHookKey)}";
        }

        return serializedState;
    }

}
