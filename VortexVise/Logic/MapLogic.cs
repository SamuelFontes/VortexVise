using Raylib_cs;
using System.Numerics;
using VortexVise.Utilities;

namespace VortexVise.GameObjects;

public static class MapLogic
{
    static string _mapName;
    static string _mapDescription;
    static string _texturePath;
    public static Texture2D _mapTexture; // This is the whole map baked into an image
    static List<Rectangle> _collisions = new List<Rectangle>();


    public static void LoadMap(string mapName, bool isServer)
    {
        // TODO: Load collisions and image from a file
        string mapFolder = "Resources/Sprites/Maps/";
        if (!isServer)
            _mapTexture = Raylib.LoadTexture(mapFolder + mapName + ".png");
        else
            _mapTexture = new Texture2D() { Height = 2048, Width = 2048 } ; // TODO: Get the map size from some place


        // TODO: Load from a json or some shit like that
        _collisions.Clear();
        _collisions.Add(new Rectangle(170, 687, 491, 32));
        _collisions.Add(new Rectangle(27, 896, 76, 32));
        _collisions.Add(new Rectangle(109, 932, 359, 32));
        _collisions.Add(new Rectangle(479, 922, 133, 32));
        _collisions.Add(new Rectangle(611, 1003, 143, 32));
        _collisions.Add(new Rectangle(723, 594, 149, 32));
        _collisions.Add(new Rectangle(885, 616, 270, 32));
        _collisions.Add(new Rectangle(1154, 298, 504, 33));
        _collisions.Add(new Rectangle(1491, 705, 453, 36));
        _collisions.Add(new Rectangle(1335, 912, 694, 39));
        _collisions.Add(new Rectangle(830, 1380, 516, 33));
        _collisions.Add(new Rectangle(380, 1392, 127, 32));
        _collisions.Add(new Rectangle(504, 1456, 78, 32));
        _collisions.Add(new Rectangle(222, 1626, 563, 32));
        _collisions.Add(new Rectangle(674, 1711, 122, 32));
        _collisions.Add(new Rectangle(1542, 1365, 184, 32));
        _collisions.Add(new Rectangle(1425, 1612, 259, 32));
        _collisions.Add(new Rectangle(1687, 1639, 173, 32));
        _collisions.Add(new Rectangle(1219, 1708, 267, 34));
    }

    public static void Draw()
    {
        Raylib.DrawTextureEx(_mapTexture, new Vector2(0, 0), 0, 1, Color.White);
        if (Utils.Debug())
        {
            foreach (var collision in _collisions) // DEBUG
            {
                Raylib.DrawRectangleRec(collision, Color.Blue);
            }
        }

    }

    public static List<Rectangle> GetCollisions()
    {
        return _collisions;
    }

    public static Vector2 GetMapSize()
    {
        return new Vector2((float)_mapTexture.Width, (float)_mapTexture.Height);
    }

}
