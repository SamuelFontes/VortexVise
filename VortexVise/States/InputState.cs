namespace VortexVise.States;

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
    public bool Confirm { get; set; } = false;
    public bool Back { get; set; } = false;
    public bool Start { get; set; } = false;
    public bool Select { get; set; } = false;
    public bool UILeft { get; set; } = false;
    public bool UIRight { get; set; } = false;
    public bool UIUp { get; set; } = false;
    public bool UIDown { get; set; } = false;
    public bool GrabDrop { get; set; } = false;
    public bool FireWeapon { get; set; } = false;

    public void ApplyInputBuffer(InputState buffer)
    {
        Left = buffer.Left || Left;
        Right = buffer.Right || Right;
        Up = buffer.Up || Up;
        Down = buffer.Down || Down;
        Jump = buffer.Jump || Jump;
        Hook = buffer.Hook || Hook;
        CancelHook = buffer.CancelHook || CancelHook;
        GrabDrop = buffer.GrabDrop || GrabDrop;
        FireWeapon = buffer.FireWeapon || FireWeapon;
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
        GrabDrop = false;
        FireWeapon = false;
    }

}
