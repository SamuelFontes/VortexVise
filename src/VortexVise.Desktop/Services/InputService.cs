using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Enums;
using VortexVise.Core.Interfaces;
using VortexVise.Core.States;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Services
{
    internal class InputService : IInputService
    {
        public InputState ReadPlayerInput(GamepadSlot gamepad)
        {
            int gamepadId = (int)gamepad;
            InputState input = new();
            if (gamepadId == -1)
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
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -0.5f)
                    input.Left = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -0.5f)
                    input.UILeft = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > 0.5f)
                    input.Right = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > 0.5f)
                    input.UIRight = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -0.5f)
                    input.Up = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -0.5f)
                    input.UIUp = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > 0.8f)
                    input.Down = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > 0.5f)
                    input.UIDown = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                    input.Jump = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                    input.CancelHook = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_LEFT))
                    input.Hook = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN))
                    input.Confirm = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT))
                    input.Back = true;
                //if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_1))
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT))
                    input.GrabDrop = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_RIGHT_TRIGGER_2))
                    input.FireWeapon = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_MIDDLE_LEFT))
                    input.Select = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_2))
                    input.JetPack = true;

            }
            return input;

        }

        public void UpdateTextUsingKeyboard(ref string text, ref bool isCursorVisible)
        {
            if ((Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE) || Raylib.IsKeyPressedRepeat(KeyboardKey.KEY_BACKSPACE)) && text.Length > 0)
            {
                isCursorVisible = false;
                text = text.Remove(text.Length - 1);
            }
            else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && Raylib.IsKeyPressed(KeyboardKey.KEY_V))
            {
                text += Raylib.GetClipboardTextAsString();
            }
            else
            {
                int keyPressed = Raylib.GetCharPressed();
                if (keyPressed != 0)
                {
                    isCursorVisible = false;
                    unsafe
                    {
                        int codepointSize = 0;
                        string textPressed = Raylib.CodepointToUTF8String(keyPressed, &codepointSize);
                        if (textPressed.Length > codepointSize)
                            textPressed = textPressed.Remove(textPressed.Length - (textPressed.Length - codepointSize));
                        text += textPressed;
                    }
                }
            }
        }
    }
}
