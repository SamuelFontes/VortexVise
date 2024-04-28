using System.Numerics;
using VortexVise.GameGlobals;
using ZeroElectric.Vinculum;

namespace VortexVise.Utilities;

/// <summary>
/// Lazy ass global functions to help create this damn game.
/// </summary>
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
        Vector2 direction = new() { X = to.X - from.X, Y = to.Y - from.Y };
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

    public static Vector2 GetRecCenter(int x, int y, int width, int height)
    {
        var pos = new Vector2(x + width * 0.5f, y + height * 0.5f);
        return pos;
    }

    public static void DrawTextCentered(string text, Vector2 textPosition, int textSize, Color color)
    {
        var textBoxSize = Raylib.MeasureTextEx(GameAssets.Misc.Font, text, textSize, 0);
        var pos = new Vector2(textPosition.X - textBoxSize.X * 0.5f, textPosition.Y - textBoxSize.Y * 0.5f); // Centers text
        Raylib.DrawTextEx(GameAssets.Misc.Font, text, pos, textSize, 0, color);
    }

    public static int GetNumberOfLocalPlayers()
    {
        var players = 0;
        if (GameCore.PlayerOneProfile.Gamepad != -9) players++;
        if (GameCore.PlayerTwoProfile.Gamepad != -9) players++;
        if (GameCore.PlayerThreeProfile.Gamepad != -9) players++;
        if (GameCore.PlayerFourProfile.Gamepad != -9) players++;
        return players;
    }

    public static float MIN(float a, float b) { return ((a) < (b) ? (a) : (b)); }

    /// <summary>
    /// Will add the velocity, it will not subtract if its negative. Also can multiply the velocity being added, I have no idea how that works but doubling the added velocity works nice when throwing things from an entity in movement.
    /// </summary>
    /// <param name="velocity">Original velocity</param>
    /// <param name="velocityToAdd">Velocity to add</param>
    /// <param name="multiplier">Optional: Will multiply the velocity to add before adding</param>
    /// <returns>The added velocity</returns>
    public static Vector2 OnlyAddVelocity(Vector2 velocity, Vector2 velocityToAdd, float multiplier = 1)
    {
        Vector2 newVelocity = velocity;
        if ((velocity.X <= 0 && velocityToAdd.X <= 0) || (velocity.X >= 0 && velocityToAdd.X >= 0))
            newVelocity += new Vector2(velocityToAdd.X * multiplier, 0);
        if ((velocity.Y <= 0 && velocityToAdd.Y <= 0) || (velocity.Y >= 0 && velocityToAdd.Y >= 0))
            newVelocity += new Vector2(0, velocityToAdd.Y * multiplier);
        return newVelocity;
    }

}
