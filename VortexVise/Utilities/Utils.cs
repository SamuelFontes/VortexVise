using System.Numerics;
using VortexVise.GameGlobals;
using ZeroElectric.Vinculum;

namespace VortexVise.Utilities;

public static class Utils
{
    public static string DebugText { get; set; } = "Vortex Vise Alpha";
    private static bool _debug = false;
    private static int _fps = 0;

    public static float Roundf(float var)
    {
        // 37.66666 * 100 =3766.66
        // 3766.66 + .5 =3767.16    for rounding off value
        // then type cast to int so value is 3767
        // then divided by 100 so the value converted into 37.67
        float value = (int)(var * 100 + .5);
        return (float)value / 100;
    }

    public static Vector2 GetVector2Direction(Vector2 from, Vector2 to)
    {
        Vector2 direction = new Vector2() { X = to.X - from.X, Y = to.Y - from.Y };
        direction = RayMath.Vector2Normalize(direction);
        return direction;
    }

    public static bool Debug()
    {
        return _debug;
    }

    public static void SwitchDebug()
    {
        _debug = !_debug;
    }

    public static void UnlockFPS()
    {
        if (_fps == 0)
            _fps = 60;
        else
            _fps = 0;
    }

    public static int GetFPS()
    {
        return _fps;
    }

    public static void UpdateTextUsingKeyboard(ref string text)
    {
        // TODO: add other input features
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE) && text.Length > 0)
        {
            GameUserInterface.IsCursorVisible = false;
            text = text.Remove(text.Length - 1);
        }
        else
        {
            int keyPressed = Raylib.GetCharPressed();
            if (keyPressed != 0)
            {
                GameUserInterface.IsCursorVisible = false;
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
