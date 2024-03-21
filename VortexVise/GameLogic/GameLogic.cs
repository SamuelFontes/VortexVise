﻿using VortexVise.GameGlobals;
using VortexVise.States;

namespace VortexVise.Logic;

public static class GameLogic
{
    private static InputState[] InputBuffer = { new(), new(), new(), new() }; // records all input and send them on network frames
    public static GameState SimulateState(GameState lastState, double currentTime, float deltaTime, bool isNetworkFrame)
    {
        GameState state = new()
        {
            Gravity = lastState.Gravity,
            CurrentTime = currentTime
        };

        // Simulate Player State
        foreach (var lastPlayerState in lastState.PlayerStates)
        {
            PlayerState currentPlayerState = new PlayerState(lastPlayerState.Id);

            // Either read player input or get input from last frame TODO: update input with last input received for all the players here, unless it's going back in time to correct stuff
            if (!ReadLocalPlayerInput(isNetworkFrame, currentPlayerState, lastPlayerState))
                currentPlayerState.Input = lastPlayerState.Input;

            PlayerLogic.SetPlayerDirection(currentPlayerState, lastPlayerState);
            (currentPlayerState.Velocity, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ProcessVelocity(deltaTime, currentPlayerState.Input, lastPlayerState, state.Gravity);
            currentPlayerState.HookState = lastPlayerState.HookState;
            currentPlayerState.Position = PlayerLogic.ProcessPosition(deltaTime, currentPlayerState, lastPlayerState.Position);
            currentPlayerState.HookState = HookLogic.SimulateState(state.Gravity, deltaTime, currentPlayerState);

            (currentPlayerState.Position, currentPlayerState.Velocity, currentPlayerState.Collision, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ApplyCollisions(currentPlayerState.Position, currentPlayerState.Velocity, lastPlayerState.Collision);

            // Handle animation
            currentPlayerState.Animation = lastPlayerState.Animation;
            currentPlayerState.Animation.ProcessAnimationRotation(currentPlayerState.Velocity, currentPlayerState.Input);

            state.PlayerStates.Add(currentPlayerState);
        }

        return state;
    }
    public static void DrawState(GameState state)
    {
        // All rendering logic should go here
        MapLogic.Draw();
        foreach (var playerState in state.PlayerStates)
        {
            HookLogic.DrawState(playerState);
            PlayerLogic.DrawState(playerState);
        }
    }
    private static bool ReadLocalPlayerInput(bool isNetworkFrame, PlayerState currentPlayerState, PlayerState lastPlayerState)
    {
        bool isLocalPlayer = false;
        int gamepad = -9;
        var playerIndex = 0;
        if (GameCore.PlayerOneGamepad != -9 && lastPlayerState.Id == GameCore.PlayerOneProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerOneGamepad;
            playerIndex = 0;
        }
        else if (GameCore.PlayerTwoGamepad != -9 && lastPlayerState.Id == GameCore.PlayerTwoProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerTwoGamepad;
            playerIndex = 1;
        }
        else if (GameCore.PlayerThreeGamepad != -9 && lastPlayerState.Id == GameCore.PlayerThreeProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerThreeGamepad;
            playerIndex = 2;
        }
        else if (GameCore.PlayerFourGamepad != -9 && lastPlayerState.Id == GameCore.PlayerFourProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerFourGamepad;
            playerIndex = 3;
        }
        if (!isLocalPlayer || gamepad == -9) return isLocalPlayer;

        if (isNetworkFrame)
        {
            currentPlayerState.Input = PlayerLogic.GetInput(gamepad); // Only read new inputs on frames we send to the server, the other frames are only for rendering 
            currentPlayerState.Input.ApplyInputBuffer(InputBuffer[playerIndex]);
            InputBuffer[playerIndex].ClearInputBuffer();
        }
        else
        {
            currentPlayerState.Input = lastPlayerState.Input;
            InputBuffer[playerIndex].ApplyInputBuffer(PlayerLogic.GetInput(gamepad));
        }
        return isLocalPlayer;
    }
}
