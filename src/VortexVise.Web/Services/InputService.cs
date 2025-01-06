using Raylib_cs;
using VortexVise.Core.Enums;
using VortexVise.Core.Interfaces;
using VortexVise.Core.States;

namespace VortexVise.Web.Services
{
    public class InputService : IInputService
    {

        public DebugCommand GetDebugCommand()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Home))
                return DebugCommand.AddDummyGamepad;
            else
                return DebugCommand.None;
        }


        public GamepadSlot GetPressStart()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsGestureDetected(Gesture.Tap))
                return GamepadSlot.MouseAndKeyboard; // Mouse and keyboard
            else if (Raylib.IsGamepadButtonPressed(0, GamepadButton.MiddleRight) || Raylib.IsGamepadButtonPressed(0, GamepadButton.RightFaceDown))
                return GamepadSlot.GamepadOne;
            else if (Raylib.IsGamepadButtonPressed(1, GamepadButton.MiddleRight) || Raylib.IsGamepadButtonPressed(1, GamepadButton.RightFaceDown))
                return GamepadSlot.GamepadTwo;
            else if (Raylib.IsGamepadButtonPressed(2, GamepadButton.MiddleRight) || Raylib.IsGamepadButtonPressed(2, GamepadButton.RightFaceDown))
                return GamepadSlot.GamepadThree;
            else if (Raylib.IsGamepadButtonPressed(3, GamepadButton.MiddleRight) || Raylib.IsGamepadButtonPressed(3, GamepadButton.RightFaceDown))
                return GamepadSlot.GamepadFour;
            else
                return GamepadSlot.Disconnected;
        }


        public InputState ReadPlayerInput(GamepadSlot gamepad)
        {
            int gamepadId = (int)gamepad;
            InputState input = new();
            if (gamepadId == -1)
            {
                // Mouse and keyboard
                if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left))
                    input.Left = true;
                if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right))
                    input.Right = true;
                if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up))
                    input.Up = true;
                if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down))
                    input.Down = true;
                if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A))
                    input.UILeft = true;
                if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D))
                    input.UIRight = true;
                if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
                    input.UIUp = true;
                if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
                    input.UIDown = true;
                if (Raylib.IsKeyDown(KeyboardKey.Space) || Raylib.IsKeyDown(KeyboardKey.K) || Raylib.IsKeyDown(KeyboardKey.LeftAlt))
                    input.Jump = true;
                if (Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsKeyPressed(KeyboardKey.K) || Raylib.IsKeyPressed(KeyboardKey.LeftAlt))
                    input.CancelHook = true;
                if (Raylib.IsKeyPressed(KeyboardKey.E) || Raylib.IsKeyPressed(KeyboardKey.L))
                    input.GrabDrop = true;
                if (Raylib.IsMouseButtonDown(MouseButton.Right) || Raylib.IsKeyDown(KeyboardKey.J))
                    input.Hook = true;
                if (Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsKeyPressed(KeyboardKey.J) || Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsGestureDetected(Gesture.Tap))
                    input.Confirm = true;
                if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                    input.Back = true;
                if (Raylib.IsKeyDown(KeyboardKey.I) || Raylib.IsMouseButtonDown(MouseButton.Left))
                    input.FireWeapon = true;
                if (Raylib.IsKeyDown(KeyboardKey.Tab))
                    input.Select = true;
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
                    input.JetPack = true;
            }
            else
            {
                // Gamepad
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.LeftFaceLeft) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) < -0.5f)
                    input.Left = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.LeftFaceLeft) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) < -0.5f)
                    input.UILeft = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.LeftFaceRight) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) > 0.5f)
                    input.Right = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.LeftFaceRight) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) > 0.5f)
                    input.UIRight = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.LeftFaceUp) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) < -0.5f)
                    input.Up = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.LeftFaceUp) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) < -0.5f)
                    input.UIUp = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.LeftFaceDown) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) > 0.8f)
                    input.Down = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.LeftFaceDown) || Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) > 0.5f)
                    input.UIDown = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightFaceDown))
                    input.Jump = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.RightFaceDown))
                    input.CancelHook = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightFaceLeft))
                    input.Hook = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.RightFaceDown))
                    input.Confirm = true;
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.RightFaceRight))
                    input.Back = true;
                //if (Raylib.IsGamepadButtonPressed(gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_1))
                if (Raylib.IsGamepadButtonPressed(gamepadId, GamepadButton.RightFaceRight))
                    input.GrabDrop = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightTrigger2))
                    input.FireWeapon = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.Middle))
                    input.Select = true;
                if (Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.LeftTrigger2))
                    input.JetPack = true;

            }
            return input;

        }


        public void UpdateTextUsingKeyboard(ref string text, ref bool isCursorVisible)
        {
            if ((Raylib.IsKeyPressed(KeyboardKey.Backspace) || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace)) && text.Length > 0)
            {
                isCursorVisible = false;
                text = text.Remove(text.Length - 1);
            }
            else if (Raylib.IsKeyDown(KeyboardKey.LeftControl) && Raylib.IsKeyPressed(KeyboardKey.V))
            {
                //text += Raylib.GetClipboardTextAsString();
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
                        //string textPressed = Raylib.CodepointToUTF8String(keyPressed, &codepointSize);
                        string textPressed = "FIXME";
                        if (textPressed.Length > codepointSize)
                            textPressed = textPressed.Remove(textPressed.Length - (textPressed.Length - codepointSize));
                        text += textPressed;
                    }
                }
            }
        }
    }
}
