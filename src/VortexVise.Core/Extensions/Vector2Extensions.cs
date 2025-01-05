using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(MathF.Min(max.X, MathF.Max(min.X, v.X)), MathF.Min(max.Y, MathF.Max(min.Y, v.Y)));
        }
    }
}
