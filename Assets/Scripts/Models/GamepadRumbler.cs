using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class GamepadRumbler
{ 
    public Gamepad Gamepad { get; set; }
    public float LowFrequency = 0f;
    public float HighFrequency = 0f;
    public float TimeUntilStop = 0f;
    public GamepadRumbler(Gamepad gamepad, float lowFrequency, float highFrequency)
    {
        Gamepad = gamepad;
        LowFrequency = lowFrequency;
        HighFrequency = highFrequency;
    }
}
