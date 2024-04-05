using System.Numerics;
using System.Text.RegularExpressions;
using VortexVise.Logic;

namespace VortexVise.States;

public class GameState
{
    public double CurrentTime { get; set; }
    public float Gravity { get; set; }
    public List<PlayerState> PlayerStates { get; set; } = new List<PlayerState>();
    public List<WeaponDropState> WeaponDrops { get; set; } = new();
    public List<DamageHitBoxState> DamageHitBoxes { get; set; } = new();

    // TODO: Add compression
    public string SerializeState()
    {
        // This will serialize the state to send over udp every frame
        // Why not JSON serialization? AOT

        string serializedState = "[S]";

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
    public static GameState DeserializeState(string serializedState)
    {
        double a;
        if (double.TryParse("3.3", out a))
            if (a == 3.3d)
                serializedState = serializedState.Replace(",", ".");
        // Do you even regex bro?
        var state = new GameState();

        try
        {
            state.CurrentTime = Convert.ToDouble(Regex.Match(serializedState, @"(?<=(\|CT))[\s\S]*?(?=\|)").Value);
            state.Gravity = float.Parse(Regex.Match(serializedState, @"(?<=(\|G))[\s\S]*?(?=\|)").Value);
            var playerMatches = Regex.Matches(serializedState, @"\|\|[\s\S]*?\|\|");

            foreach (Match match in playerMatches)
            {
                int playerId = int.Parse(Regex.Match(match.Value, @"(?<=(\|ID))[\s\S]*?(?=\|)").Value);
                var player = new PlayerState(playerId);
                player.Position = new Vector2(float.Parse(Regex.Match(match.Value, @"(?<=(\|PX))[\s\S]*?(?=\|)").Value), float.Parse(Regex.Match(match.Value, @"(?<=(\|PY))[\s\S]*?(?=\|)").Value));
                player.Velocity = new Vector2(float.Parse(Regex.Match(match.Value, @"(?<=(\|VX))[\s\S]*?(?=\|)").Value), float.Parse(Regex.Match(match.Value, @"(?<=(\|VY))[\s\S]*?(?=\|)").Value));
                player.Collision = PlayerLogic.GetPlayerCollision(player.Position);
                player.Direction = Convert.ToInt32(Regex.Match(match.Value, @"(?<=(\|D))[\s\S]*?(?=\|)").Value);
                player.IsTouchingTheGround = Regex.Match(match.Value, @"(?<=(\|TG))[\s\S]*?(?=\|)").Value == "1";
                player.Input = new InputState()
                {
                    Left = Regex.Match(match.Value, @"(?<=(\|IL))[\s\S]*?(?=\|)").Value == "1",
                    Right = Regex.Match(match.Value, @"(?<=(\|IR))[\s\S]*?(?=\|)").Value == "1",
                    Up = Regex.Match(match.Value, @"(?<=(\|IU))[\s\S]*?(?=\|)").Value == "1",
                    Down = Regex.Match(match.Value, @"(?<=(\|IPD))[\s\S]*?(?=\|)").Value == "1",
                    Jump = Regex.Match(match.Value, @"(?<=(\|IJ))[\s\S]*?(?=\|)").Value == "1",
                    Hook = Regex.Match(match.Value, @"(?<=(\|IH))[\s\S]*?(?=\|)").Value == "1",
                    CancelHook = Regex.Match(match.Value, @"(?<=(\|IC))[\s\S]*?(?=\|)").Value == "1",
                };
                player.HookState = new HookState()
                {
                    Position = new Vector2(float.Parse(Regex.Match(match.Value, @"(?<=(\|HPX))[\s\S]*?(?=\|)").Value), float.Parse(Regex.Match(match.Value, @"(?<=(\|HPY))[\s\S]*?(?=\|)").Value)),
                    Velocity = new Vector2(float.Parse(Regex.Match(match.Value, @"(?<=(\|HVX))[\s\S]*?(?=\|)").Value), float.Parse(Regex.Match(match.Value, @"(?<=(\|HVY))[\s\S]*?(?=\|)").Value)),
                    IsHookAttached = Regex.Match(match.Value, @"(?<=(\|HA))[\s\S]*?(?=\|)").Value == "1",
                    IsHookReleased = Regex.Match(match.Value, @"(?<=(\|HR))[\s\S]*?(?=\|)").Value == "1",
                    IsPressingHookKey = Regex.Match(match.Value, @"(?<=(\|HPR))[\s\S]*?(?=\|)").Value == "1",

                };
                player.HookState.Collision = PlayerHookLogic.GetHookCollision(player.HookState.Position);
                state.PlayerStates.Add(player);
            }

        }
        catch (Exception e)
        {
            // Better be safe, otherwise someone sending crappy data will crash the server
            Console.WriteLine(e.Message);
        }

        return state;
    }
    public static string SerializeInput(InputState input, int playerId, double time)
    {
        // This will serialize the input to send over udp every frame
        // Why not JSON serialization? AOT

        string serializedInput = "[I]";
        serializedInput += "|CT" + time;


        Func<bool, int> bn = b => b ? 1 : 0; // converts bool to 1 or 0

        serializedInput += $"|ID{playerId}|IL{bn(input.Left)}|IR{bn(input.Right)}|IU{bn(input.Up)}|IPD{bn(input.Down)}|IH{bn(input.Hook)}|IC{bn(input.CancelHook)}|IJ{bn(input.Jump)}";
        serializedInput += "|"; // This is needed to deserialize

        return serializedInput;
    }
    public static (Guid, InputState, double) DeserializeInput(string serializedInput)
    {
        Guid playerId = Guid.Empty;
        var input = new InputState();
        double time = 0;

        try
        {
            time = Convert.ToDouble(Regex.Match(serializedInput, @"(?<=(\|CT))[\s\S]*?(?=\|)").Value);
            playerId = Guid.Parse(Regex.Match(serializedInput, @"(?<=(\|ID))[\s\S]*?(?=\|)").Value);
            input = new InputState()
            {
                Left = Regex.Match(serializedInput, @"(?<=(\|IL))[\s\S]*?(?=\|)").Value == "1",
                Right = Regex.Match(serializedInput, @"(?<=(\|IR))[\s\S]*?(?=\|)").Value == "1",
                Up = Regex.Match(serializedInput, @"(?<=(\|IU))[\s\S]*?(?=\|)").Value == "1",
                Down = Regex.Match(serializedInput, @"(?<=(\|IPD))[\s\S]*?(?=\|)").Value == "1",
                Jump = Regex.Match(serializedInput, @"(?<=(\|IJ))[\s\S]*?(?=\|)").Value == "1",
                Hook = Regex.Match(serializedInput, @"(?<=(\|IH))[\s\S]*?(?=\|)").Value == "1",
                CancelHook = Regex.Match(serializedInput, @"(?<=(\|IC))[\s\S]*?(?=\|)").Value == "1",
            };

        }
        catch (Exception e)
        {
            // Better be safe, otherwise someone sending crappy data will crash the server
            Console.WriteLine(e.Message);
        }

        return (playerId, input, time);
    }

    public void ApproximateState(GameState localState, int playerId)
    {
        // When receive the packet do Clients Approximate Physics Locally
        var receivedPlayerState = PlayerStates.FirstOrDefault(p => p.Id == playerId);
        if (receivedPlayerState == null) return; // This should not happen 

        var lastLocalPlayerState = localState.PlayerStates.FirstOrDefault(p => p.Id == playerId);
        if (lastLocalPlayerState == null) return; // This should not happen 

        Vector2 difference = receivedPlayerState.Position - lastLocalPlayerState.Position;
        float distance = difference.Length();

        if (distance > 2.0f)
            lastLocalPlayerState.Position = receivedPlayerState.Position;
        else if (distance > 0.1)
            lastLocalPlayerState.Position += difference * 0.1f;

        receivedPlayerState.Position = lastLocalPlayerState.Position;
    }

}
