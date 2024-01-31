﻿using Raylib_cs;

namespace VortexVise.Utilities
{
    public class SerializableRectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public SerializableRectangle(Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width; 
            Height = rectangle.Height;
        }
        public Rectangle ToRectangle()
        {
            var rec = new Rectangle(X, Y, Width, Height);
            return rec;
        }
    }
}
