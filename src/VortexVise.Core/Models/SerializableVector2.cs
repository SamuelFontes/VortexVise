﻿using System.Numerics;

namespace VortexVise.Core.Models;

public class SerializableVector2
{
    public SerializableVector2(Vector2 vector)
    {
        X = vector.X;
        Y = vector.Y;
    }
    public SerializableVector2(float X, float Y)
    {
        this.X = X;
        this.Y = Y;
    }
    public SerializableVector2()
    {
    }
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2 Deserialize()
    {
        return new Vector2(X, Y);
    }
}
