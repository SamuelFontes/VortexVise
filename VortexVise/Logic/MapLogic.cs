using Raylib_cs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Utilities;

namespace VortexVise.GameObjects;

public class MapLogic
{
    string _mapName;
    string _mapDescription;
    string _texturePath;
    Texture2D _mapTexture; // This is the whole map baked into an image
    List<Rectangle> _collisions = new List<Rectangle>();


    public void LoadMap(string mapName)
    {
        // TODO: Load collisions and image from a file
        string mapFolder = "Resources/Sprites/Maps/";
        _mapTexture = Raylib.LoadTexture(mapFolder + mapName + ".png");


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

    public void Draw()
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

    public List<Rectangle> GetCollisions()
    {
        return _collisions;
    }

    public Vector2 GetMapSize()
    {
        return new Vector2((float)_mapTexture.Width, (float)_mapTexture.Height);
    }

}
