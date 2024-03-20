using VortexVise.GameGlobals;
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
            if (GameCore.PlayerOneGamepad != -9 && lastPlayerState.Id == GameCore.PlayerOneProfile.Id)
                ReadPlayerInput(isNetworkFrame, currentPlayerState, lastPlayerState, GameCore.PlayerOneGamepad, 0);
            else if (GameCore.PlayerTwoGamepad != -9 && lastPlayerState.Id == GameCore.PlayerTwoProfile.Id)
                ReadPlayerInput(isNetworkFrame, currentPlayerState, lastPlayerState, GameCore.PlayerTwoGamepad, 1);
            else if (GameCore.PlayerThreeGamepad != -9 && lastPlayerState.Id == GameCore.PlayerThreeProfile.Id)
                ReadPlayerInput(isNetworkFrame, currentPlayerState, lastPlayerState, GameCore.PlayerThreeGamepad, 2);
            else if (GameCore.PlayerFourGamepad != -9 && lastPlayerState.Id == GameCore.PlayerFourProfile.Id)
                ReadPlayerInput(isNetworkFrame, currentPlayerState, lastPlayerState, GameCore.PlayerFourGamepad, 3);
            else
            {
                currentPlayerState.Input = lastPlayerState.Input;
            }
            currentPlayerState.Direction = PlayerLogic.ProcessDirection(deltaTime, currentPlayerState.Input, lastPlayerState);
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
    private static void ReadPlayerInput(bool isNetworkFrame, PlayerState currentPlayerState, PlayerState lastPlayerState, int gamepad, int playerIndex)
    {
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

    }
}
