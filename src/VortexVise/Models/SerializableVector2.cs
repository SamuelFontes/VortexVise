using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Models;

public class SerializableVector2
{
    public SerializableVector2(Vector2 vector)
    {
        X = vector.X; 
        Y = vector.Y;
    }
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2 Deserialize()
    {
        return new Vector2(X, Y);
    }
}
