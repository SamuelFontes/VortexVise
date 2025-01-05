namespace VortexVise.Desktop.Extensions
{
    public static class ColorExtensions
    {
        // Convert System.Drawing.Color to ZeroElectric.Vinculum.Color
        public static ZeroElectric.Vinculum.Color ToRaylibColor(this System.Drawing.Color color)
        {
            return new ZeroElectric.Vinculum.Color
            {
                r = color.R,
                g = color.G,
                b = color.B,
                a = color.A
            };
        }

        // Convert ZeroElectric.Vinculum.Color to System.Drawing.Color
        public static System.Drawing.Color ToDrawingColor(this ZeroElectric.Vinculum.Color color)
        {
            return System.Drawing.Color.FromArgb(color.a, color.r, color.g, color.b);
        }
    }
}

