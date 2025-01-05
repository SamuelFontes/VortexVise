namespace VortexVise.Web.Extensions
{
    public static class RectangleExtensions
    {
        public static Raylib_cs.Rectangle ToRaylibRectangle(this System.Drawing.Rectangle rect)
        {
            return new Raylib_cs.Rectangle
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width,
                Height = rect.Height
            };
        }
        public static System.Drawing.Rectangle ToDrawingRectangle(this Raylib_cs.Rectangle rect)
        {
            return new System.Drawing.Rectangle
            {
                X = (int)rect.X,
                Y = (int)rect.Y,
                Width = (int)rect.Width,
                Height = (int)rect.Height
            };
        }
    }
}
