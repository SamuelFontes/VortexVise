using System.Drawing;

namespace VortexVise.Core.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Get color with alpha applied, alpha goes from 0.0f to 1.0f
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color Fade(this Color color, float alpha)
        {
            if (alpha < 0.0f) alpha = 0.0f;
            else if (alpha > 1.0f)
                alpha = 1.0f; return Color.FromArgb((int)(255 * alpha), color.R, color.G, color.B);
        }
    }
}
