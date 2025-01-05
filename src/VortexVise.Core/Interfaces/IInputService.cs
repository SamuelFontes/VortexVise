using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Enums;
using VortexVise.Core.States;

namespace VortexVise.Core.Interfaces
{
    public interface IInputService
    {
        /// <summary>
        /// Read player input for the current frame.
        /// </summary>
        /// <param name="gamepad"> Gamepad number. -1 is mouse and keyboard, 0 to 3 are gamepad slots.</param>
        /// <returns>A input object representing the current inputs pressed this frame for this specific game controller.</returns>
        public InputState ReadPlayerInput(GamepadSlot gamepad);
        /// <summary>
        /// Used on input
        /// </summary>
        /// <param name="text">Reference of string that needs to change</param>
        /// <param name="isCursorVisible">Reference to cursor visibility toggle</param>
        public void UpdateTextUsingKeyboard(ref string text, ref bool isCursorVisible);
    }
}
