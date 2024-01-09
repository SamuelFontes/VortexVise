using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Utils
{
    public static Vector2 GetMousePosition()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }

    #region GamepadHelper
    // This method allows us to have multiple vibrations scheduled for diffent durations
    private static List<GamepadRumbler> _gamepadsVibrating = new List<GamepadRumbler>();
    public static void GamepadRumble(Gamepad gamepad,float lowFrequency, float highFrequency, float duration) {
        if (gamepad == null) 
            return; // If is keyboard and mouse there is no gamepad

        // Check if it is already vibrating
        var gamepadVibrating = _gamepadsVibrating.Where(_ => _.Gamepad == gamepad).FirstOrDefault();
        if (gamepadVibrating == null) 
        {
            gamepadVibrating = new GamepadRumbler(gamepad, lowFrequency, highFrequency);
            _gamepadsVibrating.Add(gamepadVibrating);
        }
        gamepadVibrating.AddFrequency(lowFrequency, highFrequency, duration);    

        // Make it go bruummmmmmmmmmmmmmmmmm
        gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
    } 
    
    public static void UpdateGamepadRumble()
    {
        for(var i = 0; i < _gamepadsVibrating.Count; i++)
        {
            _gamepadsVibrating[i].UpdateGamepadVibration();
        }
    }
    #endregion

    #region Animation
    #endregion
}
