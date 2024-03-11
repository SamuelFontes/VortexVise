using ZeroElectric.Vinculum;
using System.Numerics;
using VortexVise.Utilities;

namespace VortexVise.Logic;

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
            _mapTexture = new Texture2D() { height = 2048, width = 2048 }; // TODO: Get the map size from some place


        // TODO: Load from a json or some shit like that
        _collisions.Clear();
        _collisions.Add(new Rectangle(85, 343, 245, 16));
        _collisions.Add(new Rectangle(13, 448, 38, 16));
        _collisions.Add(new Rectangle(54, 466, 179, 16));
        _collisions.Add(new Rectangle(240, 460, 66, 16));
        _collisions.Add(new Rectangle(305, 501, 71, 16));
        _collisions.Add(new Rectangle(361, 297, 74, 16));
        _collisions.Add(new Rectangle(442, 308, 135, 16));
        _collisions.Add(new Rectangle(570, 149, 252, 16));
        _collisions.Add(new Rectangle(745, 352, 226, 16));
        _collisions.Add(new Rectangle(670, 456, 347, 16));
        _collisions.Add(new Rectangle(415, 690, 258, 16));
        _collisions.Add(new Rectangle(190, 696, 64, 16));
        _collisions.Add(new Rectangle(252, 728, 39, 16));
        _collisions.Add(new Rectangle(111, 813, 281, 16));
        _collisions.Add(new Rectangle(337, 855, 61, 16));
        _collisions.Add(new Rectangle(771, 682, 92, 16));
        _collisions.Add(new Rectangle(712, 806, 129, 16));
        _collisions.Add(new Rectangle(843, 819, 85, 16));
        _collisions.Add(new Rectangle(609, 854, 128, 16));
    }

    public static void Draw()
    {
        Raylib.DrawTextureEx(_mapTexture, new Vector2(0, 0), 0, 1, WHITE);
        if (Utils.Debug())
        {
            foreach (var collision in _collisions) // DEBUG
            {
                Raylib.DrawRectangleRec(collision, BLUE);
            }
        }

    }

    public static List<Rectangle> GetCollisions()
    {
        return _collisions;
    }

    public static Vector2 GetMapSize()
    {
        return new Vector2(_mapTexture.width, _mapTexture.height);
    }

}
