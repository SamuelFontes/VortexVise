﻿using VortexVise.States;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;

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
    public static bool ReadLocalPlayerInput(bool isNetworkFrame, PlayerState currentPlayerState, PlayerState lastPlayerState)
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
            currentPlayerState.Input = GetInput(gamepad); // Only read new inputs on frames we send to the server, the other frames are only for rendering 
            currentPlayerState.Input.ApplyInputBuffer(InputBuffer[playerIndex]);
            InputBuffer[playerIndex].ClearInputBuffer();
        }
        else
        {
            currentPlayerState.Input = lastPlayerState.Input;
            InputBuffer[playerIndex].ApplyInputBuffer(GetInput(gamepad));
        }
        return isLocalPlayer;
    }

    /// <summary>
    /// Read player input for the current frame.
    /// </summary>
    /// <param name="gamepad"> Gamepad number. -1 is mouse and keyboard, 0 to 3 are gamepad slots.</param>
    /// <returns>A input object representing the current inputs pressed this frame for this specific game controller.</returns>
    public static InputState GetInput(int gamepad)
    {
        InputState input = new();
        if (gamepad == -1)
        {
            // Mouse and keyboard
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                input.Left = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                input.Right = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W) || Raylib.IsKeyDown(KeyboardKey.KEY_UP))
                input.Up = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_S) || Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
                input.Down = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT))// || Raylib.IsKeyPressed(KeyboardKey.KEY_A))
                input.UILeft = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT))// || Raylib.IsKeyPressed(KeyboardKey.KEY_D))
                input.UIRight = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))// || Raylib.IsKeyPressed(KeyboardKey.KEY_W))
                input.UIUp = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))// || Raylib.IsKeyPressed(KeyboardKey.KEY_S))
                input.UIDown = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) || Raylib.IsKeyDown(KeyboardKey.KEY_K) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT))
                input.Jump = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Raylib.IsKeyPressed(KeyboardKey.KEY_K) || Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT_ALT))
                input.CancelHook = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_E) || Raylib.IsKeyPressed(KeyboardKey.KEY_L))
                input.GrabDrop = true;
            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT) || Raylib.IsKeyDown(KeyboardKey.KEY_J))
                input.Hook = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Raylib.IsKeyPressed(KeyboardKey.KEY_J) || Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
                input.Confirm = true;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                input.Back = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_I) || Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
                input.FireWeapon = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_TAB))
                input.Select = true;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
                input.JetPack = true;
        }
        else
        {
            // Gamepad
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -0.5f)
                input.Left = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -0.5f)
                input.UILeft = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > 0.5f)
                input.Right = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > 0.5f)
                input.UIRight = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -0.5f)
                input.Up = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -0.5f)
                input.UIUp = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > 0.8f)
                input.Down = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN) || Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > 0.5f)
                input.UIDown = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                input.Jump = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                input.CancelHook = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_LEFT))
                input.Hook = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                input.Confirm = true;
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT))
                input.Back = true;
            //if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_1))
            if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT))
                input.GrabDrop = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_TRIGGER_2))
                input.FireWeapon = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_MIDDLE_LEFT))
                input.Select = true;
            if (Raylib.IsGamepadButtonDown(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_2))
                input.JetPack = true;

        }
        return input;
    }

}
