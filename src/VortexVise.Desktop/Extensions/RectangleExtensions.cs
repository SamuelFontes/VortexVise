using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Desktop.Extensions
{
    public static class RectangleExtensions
    {
        public static ZeroElectric.Vinculum.Rectangle ToRaylibRectangle(this System.Drawing.Rectangle rect)
        {
            return new ZeroElectric.Vinculum.Rectangle
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width,
                Height = rect.Height
            };
        }
        public static System.Drawing.Rectangle ToDrawingRectangle(this ZeroElectric.Vinculum.Rectangle rect)
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
