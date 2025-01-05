using System.Numerics;

namespace VortexVise.Core.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(MathF.Min(max.X, MathF.Max(min.X, v.X)), MathF.Min(max.Y, MathF.Max(min.Y, v.Y)));
        }

        public static Vector2 Scale(this Vector2 v, float scale)
        {
            Vector2 result = new(v.X * scale, v.Y * scale);
            return result;
        }
    }
}
