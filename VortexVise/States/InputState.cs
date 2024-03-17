﻿namespace VortexVise.States;

public class InputState
{
    public string Owner { get; set; }
    public bool Left { get; set; } = false;
    public bool Right { get; set; } = false;
    public bool Up { get; set; } = false;
    public bool Down { get; set; } = false;
    public bool Jump { get; set; } = false;
    public bool Hook { get; set; } = false;
    public bool CancelHook { get; set; } = false;

    public void ApplyInputBuffer(InputState buffer)
    {
        Left = buffer.Left || Left;
        Right = buffer.Right || Right;
        Up = buffer.Up || Up;
        Down = buffer.Down || Down;
        Jump = buffer.Jump || Jump;
        Hook = buffer.Hook || Hook;
        CancelHook = buffer.CancelHook || CancelHook;
    }

    public void ClearInputBuffer()
    {
        Left = false;
        Right = false;
        Up = false;
        Down = false;
        Jump = false;
        Hook = false;
        CancelHook = false;
    }

}
