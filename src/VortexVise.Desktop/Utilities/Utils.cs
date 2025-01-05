using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Models;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Models;

namespace VortexVise.Desktop.Utilities;

/// <summary>
/// Useful global functions.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Set this string to anything to easilly show text on the screen
    /// </summary>
    public static string DebugText { get; set; } = "Vortex Vise";

    private static bool _debug = false;

    /// <summary>
    /// Used to do some internal calculations
    /// 37.66666 * 100 =3766.66
    /// 3766.66 + .5 =3767.16    for rounding off value
    /// then type cast to int so value is 3767
    /// then divided by 100 so the value converted into 37.67
    /// </summary>
    /// <param name="var"></param>
    /// <returns></returns>
    public static float Roundf(float var)
    {
        float value = (int)(var * 100 + .5);
        return (float)value / 100;
    }

    /// <summary>
    /// Return vector 2 direction
    /// </summary>
    /// <param name="from">Source</param>
    /// <param name="to">Target</param>
    /// <returns></returns>
    public static Vector2 GetVector2Direction(Vector2 from, Vector2 to)
    {
        Vector2 direction = new() { X = to.X - from.X, Y = to.Y - from.Y };
        direction = Vector2.Normalize(direction);
        return direction;
    }

    /// <summary>
    /// Check if game is on debug mode
    /// </summary>
    /// <returns></returns>
    public static bool Debug()
    {
        return _debug;
    }

    /// <summary>
    /// Switch game to debug mode
    /// </summary>
    public static void SwitchDebug()
    {
        _debug = !_debug;
    }

    public static void UpdateTextUsingKeyboard(ref string text)
    {
    }

    /// <summary>
    /// Get center of rectangle
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Vector2 GetRecCenter(int x, int y, int width, int height)
    {
        var pos = new Vector2(x + width * 0.5f, y + height * 0.5f);
        return pos;
    }

    /// <summary>
    /// Check how many players are connected to the game locally
    /// </summary>
    /// <returns></returns>
    public static int GetNumberOfLocalPlayers()
    {
        var players = 0;
        if (GameCore.PlayerOneProfile.Gamepad != GamepadSlot.Disconnected) players++;
        if (GameCore.PlayerTwoProfile.Gamepad != GamepadSlot.Disconnected) players++;
        if (GameCore.PlayerThreeProfile.Gamepad != GamepadSlot.Disconnected) players++;
        if (GameCore.PlayerFourProfile.Gamepad != GamepadSlot.Disconnected) players++;
        return players;
    }

    /// <summary>
    /// Have no idea what the fuck is this
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
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

    public static string GetFileChecksum(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public static List<PlayerProfile> GetAllLocalPlayerProfiles()
    {
        List<PlayerProfile> list = new List<PlayerProfile>();
        if (GameCore.PlayerOneProfile.Gamepad != GamepadSlot.Disconnected) list.Add(GameCore.PlayerOneProfile);
        if (GameCore.PlayerTwoProfile.Gamepad != GamepadSlot.Disconnected) list.Add(GameCore.PlayerTwoProfile);
        if (GameCore.PlayerThreeProfile.Gamepad != GamepadSlot.Disconnected) list.Add(GameCore.PlayerThreeProfile);
        if (GameCore.PlayerFourProfile.Gamepad != GamepadSlot.Disconnected) list.Add(GameCore.PlayerFourProfile);
        return list;
    }

    public static byte[] GetTcpResponse(ref TcpClient tcpClient)
    {
        var data = new List<byte>();
        var buffer = new byte[512]; //size can be different, just an example
        var terminatorReceived = false;
        while (!terminatorReceived)
        {
            var bytesReceived = tcpClient.Client.Receive(buffer);
            if (bytesReceived > 0)
            {
                data.AddRange(buffer.Take(bytesReceived));
                terminatorReceived = data.Contains(0xFD);
            }
        }
        return data.ToArray();
    }

}
