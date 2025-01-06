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
            if (alpha < 0.0f) alpha = 0;
            else if (alpha > 1.0f)
                alpha = 1.0f; 
            int alphaValue = (int)(255 * alpha);
            if(alphaValue < 0) alphaValue = 0;
            return Color.FromArgb(alphaValue, color.R, color.G, color.B);
        }
    }
}
