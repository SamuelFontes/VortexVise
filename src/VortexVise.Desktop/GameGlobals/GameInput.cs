using VortexVise.Desktop.States;
using VortexVise.Core.States;
using VortexVise.Core.Interfaces;

namespace VortexVise.Desktop.GameGlobals;

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
    public static bool ReadLocalPlayerInput(bool isNetworkFrame, PlayerState currentPlayerState, PlayerState lastPlayerState, IInputService inputService)
    {
        bool isLocalPlayer = false;
        int gamepad = -9;
        var playerIndex = 0;
        if (GameCore.PlayerOneProfile.Gamepad != -9 && lastPlayerState.Id == GameCore.PlayerOneProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerOneProfile.Gamepad;
            playerIndex = 0;
        }
        else if (GameCore.PlayerTwoProfile.Gamepad != -9 && lastPlayerState.Id == GameCore.PlayerTwoProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerTwoProfile.Gamepad;
            playerIndex = 1;
        }
        else if (GameCore.PlayerThreeProfile.Gamepad != -9 && lastPlayerState.Id == GameCore.PlayerThreeProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerThreeProfile.Gamepad;
            playerIndex = 2;
        }
        else if (GameCore.PlayerFourProfile.Gamepad != -9 && lastPlayerState.Id == GameCore.PlayerFourProfile.Id)
        {
            isLocalPlayer = true;
            gamepad = GameCore.PlayerFourProfile.Gamepad;
            playerIndex = 3;
        }
        if (!isLocalPlayer || gamepad == -9) return isLocalPlayer;

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
