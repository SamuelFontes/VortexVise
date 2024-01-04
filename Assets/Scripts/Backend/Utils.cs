//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
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
    static List<GamepadRumbler> gamepadsVibrating = new List<GamepadRumbler>();
    public static void GamepadRumble(Gamepad gamepad,float lowFrequency, float highFrequency, float duration) {
        if (gamepad == null) 
            return; // If is keyboard and mouse there is no gamepad

        // Check if it is already vibrating
        var gv = gamepadsVibrating.Where(_ => _.Gamepad == gamepad).FirstOrDefault();
        if (gv == null) 
        {
            gv = new GamepadRumbler(gamepad, lowFrequency, highFrequency);
            gamepadsVibrating.Add(gv);
        }

        // If it is already vibrating it will add more vibration
        gv.LowFrequency+= lowFrequency;
        gv.HighFrequency+= highFrequency;
        if(gv.LowFrequency > 1f) gv.LowFrequency= 1f; // 1f is the max vibration
        if(gv.HighFrequency > 1f) gv.HighFrequency= 1f;
        gv.TimeUntilStop += duration;

        // Make it go bruummmmmmmmmmmmmmmmmm
        gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
    } 
    
    public static void UpdateGamepadRumble()
    {
        foreach (var g in gamepadsVibrating)
        {
            if(g.TimeUntilStop <= 0)
                continue;

            g.TimeUntilStop -= Time.deltaTime;
            if(g.TimeUntilStop < 0f)
            {
                g.Gamepad.SetMotorSpeeds(0f, 0f);
                g.TimeUntilStop = 0f;
            }

        }
    }
    #endregion
}
