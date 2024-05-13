using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VortexVise.States;

namespace VortexVise.Networking;

public static class GameStateSerializer
{
    /// <summary>
    /// Serialize to json the gamestate, storing the Vector2 in the SerializableVector2 
    /// </summary>
    /// <param name="state">GameState</param>
    /// <returns>Json serilized strings for game state.</returns>
    public static string SerializeState(GameState state)
    {
        foreach (var p in state.PlayerStates)
        {
            p.SerializablePosition = new(p.Position);
            p.SerializableVelocity = new(p.Velocity);
            p.HookState.SerializablePosition = new(p.HookState.Position);
            p.HookState.SerializableVelocity = new(p.HookState.Velocity);
            p.Animation.SerializablePosition = new(p.Animation.Position);
        }

        foreach (var w in state.WeaponDrops)
        {
            w.SerializablePosition = new(w.Position);
            w.SerializableVelocity = new(w.Velocity);
        }

        foreach(var d in state.DamageHitBoxes)
        {
            d.SerializableVelocity = new(d.Velocity);
        }

        foreach(var a in state.Animations)
        {
            a.SerializablePosition = new(a.Position);
        }

        var response = JsonSerializer.Serialize(state, SourceGenerationContext.Default.GameState);
        return response;
    }

    public static GameState DeserializeState(string serializedState)
    {
        GameState state = JsonSerializer.Deserialize(serializedState, SourceGenerationContext.Default.GameState);
        foreach (var p in state.PlayerStates)
        {
            p.Position = new(p.SerializablePosition.X, p.SerializablePosition.Y);
            p.Velocity = new(p.SerializableVelocity.X, p.SerializableVelocity.Y);
            p.HookState.Position = new(p.HookState.SerializablePosition.X,p.HookState.SerializablePosition.Y);
            p.HookState.Velocity = new(p.HookState.SerializableVelocity.X,p.HookState.SerializableVelocity.Y);
            p.Animation.Position = new(p.Animation.SerializablePosition.X, p.Animation.SerializablePosition.Y);
        }

        foreach (var w in state.WeaponDrops)
        {
            w.Position = new(w.SerializablePosition.X, w.SerializablePosition.Y);
            w.Velocity = new(w.SerializableVelocity.X, w.SerializableVelocity.Y);
        }

        foreach(var d in state.DamageHitBoxes)
        {
            d.Velocity = new(d.SerializableVelocity.X, d.SerializableVelocity.Y);
        }

        foreach(var a in state.Animations)
        {
            a.Position = new(a.SerializablePosition.X, a.SerializablePosition.Y);
        }
        return state;
    }
    public static string SerializeInput(InputState input, Guid playerId, ulong tick)
    {
        input.PlayerId = playerId;
        input.Tick = tick;
        var response = JsonSerializer.Serialize(input, SourceGenerationContext.Default.InputState);
        return response;
    }
    public static InputState DeserializeInput(string serializedInput)
    {
        InputState s = JsonSerializer.Deserialize(serializedInput, SourceGenerationContext.Default.InputState);
        return s;
    }

    public static void ApproximateState(GameState localState,GameState receivedState, Guid playerId)
    {
        // When receive the packet do Clients Approximate Physics Locally
        var receivedPlayerState = receivedState.PlayerStates.FirstOrDefault(p => p.Id == playerId);
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
