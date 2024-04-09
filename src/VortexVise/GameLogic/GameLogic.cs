﻿using VortexVise.GameGlobals;
using VortexVise.States;

namespace VortexVise.Logic;

/// <summary>
/// Contains all gameplay logic to simulate a state/frame.
/// </summary>
public static class GameLogic
{
    public static GameState SimulateState(GameState lastState, double currentTime, float deltaTime, bool isNetworkFrame)
    {
        GameState state = new()
        {
            Gravity = lastState.Gravity,
            CurrentTime = currentTime,
            DamageHitBoxes = lastState.DamageHitBoxes,
        };

        // Copy last state
        WeaponLogic.CopyLastState(state, lastState);
        WeaponLogic.ProcessHitBoxes(state, deltaTime);

        // Simulate Player State
        foreach (var lastPlayerState in lastState.PlayerStates)
        {
            PlayerState currentPlayerState = new(lastPlayerState.Id);

            // Either read player input or get input from last frame TODO: update input with last input received for all the players here, unless it's going back in time to correct stuff
            if (!GameInput.ReadLocalPlayerInput(isNetworkFrame, currentPlayerState, lastPlayerState))
                currentPlayerState.Input = lastPlayerState.Input;


            // Handle Player Behaviour
            PlayerLogic.CopyLastPlayerState(currentPlayerState, lastPlayerState);
            PlayerLogic.AddPlayerTimers(currentPlayerState, deltaTime);
            PlayerLogic.SetPlayerDirection(currentPlayerState);
            PlayerLogic.ProcessPlayerMovement(currentPlayerState, deltaTime);
            PlayerLogic.ProcessPlayerJump(currentPlayerState, deltaTime);
            PlayerLogic.ApplyPlayerGravity(currentPlayerState, deltaTime, state.Gravity);
            PlayerHookLogic.SimulateHookState(currentPlayerState, state.Gravity, deltaTime);
            WeaponLogic.ApplyHitBoxesDamage(state, currentPlayerState);
            PlayerLogic.ApplyPlayerVelocity(currentPlayerState, deltaTime);
            PlayerLogic.ApplyCollisions(currentPlayerState, deltaTime);

            PlayerLogic.ProcessPlayerPickUpItem(state, currentPlayerState);

            WeaponLogic.ProcessPlayerShooting(currentPlayerState, state, deltaTime);

            // Handle animation
            currentPlayerState.Animation.ProcessDash(deltaTime);
            currentPlayerState.Animation.ProcessAnimationRotation(currentPlayerState.Velocity, currentPlayerState.Input, deltaTime);

            state.PlayerStates.Add(currentPlayerState);
        }

        // Handle Weapon Drops
        WeaponLogic.SpawnWeapons(state, deltaTime);
        WeaponLogic.UpdateWeaponDrops(state, deltaTime);


        return state;
    }
}
