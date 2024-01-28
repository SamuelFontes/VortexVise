using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Utilities
{
    public class SerializableVector2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public SerializableVector2(Vector2 vec)
        {
            X = vec.X;
            Y = vec.Y;
        }
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
