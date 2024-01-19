using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadRumbler
{ 
    public Gamepad Gamepad { get; private set; }
    public float LowFrequency { get; private set; }
    public float HighFrequency { get; private set; }
    public float TimeUntilStop { get; private set; }
    public GamepadRumbler(Gamepad gamepad, float lowFrequency, float highFrequency)
    {
        Gamepad = gamepad;
        LowFrequency = lowFrequency;
        HighFrequency = highFrequency;
    }

    public void AddFrequency(float lowFrequency, float highFrequency, float duration)
    {
        // If it is already vibrating it will add more vibration
        LowFrequency+= lowFrequency;
        HighFrequency+= highFrequency;
        if(LowFrequency > 1f) LowFrequency= 1f; // 1f is the max vibration
        if(HighFrequency > 1f) HighFrequency= 1f;
        if(TimeUntilStop < duration) TimeUntilStop = duration;
    }

    public void UpdateGamepadVibration()
    {
        if(TimeUntilStop <= 0)
            return;

        TimeUntilStop -= Time.deltaTime;
        if(TimeUntilStop <= 0f)
        {
            Gamepad.SetMotorSpeeds(0f, 0f);
            TimeUntilStop = 0f;
        }

    }
}
