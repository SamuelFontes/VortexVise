using System.Text.RegularExpressions;
using System.Numerics;

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

        Func<bool, int> bn = b => b ? 1 : 0; // converts bool to 1 or 0
        // Collisions are not serialized, they will use the position and the size on the logic to deserialize

        foreach (PlayerState state in PlayerStates)
        {
            serializedState += $"||P|ID{state.Id}|PX{state.Position.X}|PY{state.Position.Y}|VX{state.Velocity.X}|VY{state.Velocity.Y}|D{state.Direction}|TG{bn(state.IsTouchingTheGround)}|IL{bn(state.Input.Left)}|IR{bn(state.Input.Right)}|IU{bn(state.Input.Up)}|IPD{bn(state.Input.Down)}|IH{bn(state.Input.Hook)}|IC{bn(state.Input.CancelHook)}|IJ{bn(state.Input.Jump)}|HPX{state.HookState.Position.X}|HPY{state.HookState.Position.Y}|HVX{state.HookState.Velocity.X}|HVY{state.HookState.Velocity.Y}|HA{bn(state.HookState.IsHookAttached)}|HR{bn(state.HookState.IsHookReleased)}|HPR{bn(state.HookState.IsPressingHookKey)}||";
        }

        return serializedState;
    }
    public static GameState Deserialize(string serializedState)
    {
        // Do you even regex bro?
        var state = new GameState();

        try
        {
            state.CurrentTime = Convert.ToDouble(Regex.Match(serializedState, @"(?<=(\|CT))[\s\S]*?(?=\|)").Value);
            state.Gravity = float.Parse(Regex.Match(serializedState, @"(?<=(\|G))[\s\S]*?(?=\|)").Value);
            var playerMatches = Regex.Matches(serializedState, @"\|\|[\s\S]*?\|\|");

            foreach (Match match in playerMatches)
            {
                Guid playerId = Guid.Parse(Regex.Match(match.Value, @"(?<=(\|ID))[\s\S]*?(?=\|)").Value);
                var player = new PlayerState(playerId);
                player.Position = new Vector2(float.Parse(Regex.Match(match.Value, @"(?<=(\|PX))[\s\S]*?(?=\|)").Value), float.Parse(Regex.Match(match.Value, @"(?<=(\|PY))[\s\S]*?(?=\|)").Value));
                player.Velocity = new Vector2(float.Parse(Regex.Match(match.Value, @"(?<=(\|VX))[\s\S]*?(?=\|)").Value), float.Parse(Regex.Match(match.Value, @"(?<=(\|VY))[\s\S]*?(?=\|)").Value));
                player.Direction = Convert.ToInt32(Regex.Match(match.Value, @"(?<=(\|D))[\s\S]*?(?=\|)"));
                player.IsTouchingTheGround = Regex.Match(match.Value,@"(?<=(\|TG))[\s\S]*?(?=\|)").Value == "1";
                player.Input = new InputState()
                {
                    Left = Regex.Match(match.Value,@"(?<=(\|IL))[\s\S]*?(?=\|)").Value == "1",
                    Right = Regex.Match(match.Value,@"(?<=(\|IR))[\s\S]*?(?=\|)").Value == "1",
                    Up = Regex.Match(match.Value,@"(?<=(\|IU))[\s\S]*?(?=\|)").Value == "1",
                    Down = Regex.Match(match.Value,@"(?<=(\|IPD))[\s\S]*?(?=\|)").Value == "1",
                    Jump = Regex.Match(match.Value,@"(?<=(\|IJ))[\s\S]*?(?=\|)").Value == "1",
                    Hook = Regex.Match(match.Value,@"(?<=(\|IH))[\s\S]*?(?=\|)").Value == "1",
                    CancelHook = Regex.Match(match.Value,@"(?<=(\|IC))[\s\S]*?(?=\|)").Value == "1",
                };
                player.HookState = new HookState()
                {

                };
            }

        }
        catch (Exception e)
        {
            // Better be safe, otherwise someone sending crappy data will crash the server
            Console.WriteLine(e.Message);
        }

        return state;
    }

}
