namespace VortexVise.Web.Extensions
{
    public static class ColorExtensions
    {
        // Convert System.Drawing.Color to ZeroElectric.Vinculum.Color
        public static Raylib_cs.Color ToRaylibColor(this System.Drawing.Color color)
        {
            return new Raylib_cs.Color
            {
                R = color.R,
                G = color.G,
                B = color.B,
                A = color.A
            };
        }

        // Convert ZeroElectric.Vinculum.Color to System.Drawing.Color
        public static System.Drawing.Color ToDrawingColor(this Raylib_cs.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}

