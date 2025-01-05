using VortexVise.Desktop.States;
using VortexVise.Core.States;
using VortexVise.Core.Interfaces;
using VortexVise.Core.GameContext;
using VortexVise.Core.Enums;

namespace VortexVise.Desktop.GameContext;

/// <summary>
/// Handles all player input
/// </summary>
public static class GameInput
{
    /// <summary>
    /// Used for only buffering inputs and processing it on network/tick frames to keep game behavior consistent between clients and server in network play.
    /// </summary>
    private static readonly InputState[] InputBuffer = [new(), new(), new(), new()];

    /// <summary>
    /// This will be used to read the input for local players on the game logic.
    /// </summary>
    /// <param name="isNetworkFrame">If true input will be processed. Otherwise it will buffer for next network/tick frame.</param>
    /// <param name="currentPlayerState">Current player state</param>
    /// <param name="lastPlayerState">Last player state</param>
    /// <returns>A boolean indicating if this player is one of the local players.</returns>
    public static bool ReadLocalPlayerInput(bool isNetworkFrame, PlayerState currentPlayerState, PlayerState lastPlayerState, IInputService inputService, GameCore gameCore)
    {
        bool isLocalPlayer = false;
        GamepadSlot gamepad = GamepadSlot.Disconnected;
        var playerIndex = 0;
        if (gameCore.PlayerOneProfile.Gamepad != GamepadSlot.Disconnected && lastPlayerState.Id == gameCore.PlayerOneProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = gameCore.PlayerOneProfile.Gamepad;
            playerIndex = 0;
        }
        else if (gameCore.PlayerTwoProfile.Gamepad != GamepadSlot.Disconnected && lastPlayerState.Id == gameCore.PlayerTwoProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = gameCore.PlayerTwoProfile.Gamepad;
            playerIndex = 1;
        }
        else if (gameCore.PlayerThreeProfile.Gamepad != GamepadSlot.Disconnected && lastPlayerState.Id == gameCore.PlayerThreeProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = gameCore.PlayerThreeProfile.Gamepad;
            playerIndex = 2;
        }
        else if (gameCore.PlayerFourProfile.Gamepad != GamepadSlot.Disconnected && lastPlayerState.Id == gameCore.PlayerFourProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = gameCore.PlayerFourProfile.Gamepad;
            playerIndex = 3;
        }
        if (!isLocalPlayer || gamepad == GamepadSlot.Disconnected) return isLocalPlayer;

        if (isNetworkFrame)
        {
            currentPlayerState.Input = inputService.ReadPlayerInput(gamepad); // Only read new inputs on frames we send to the server, the other frames are only for rendering 
            currentPlayerState.Input.ApplyInputBuffer(InputBuffer[playerIndex]);
            InputBuffer[playerIndex].ClearInputBuffer();
        }
        else
        {
            currentPlayerState.Input = lastPlayerState.Input;
            InputBuffer[playerIndex].ApplyInputBuffer(inputService.ReadPlayerInput(gamepad));
        }
        return isLocalPlayer;
    }

}
