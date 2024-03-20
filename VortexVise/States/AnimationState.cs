﻿using System.Numerics;
using ZeroElectric.Vinculum;

namespace VortexVise.States;

public class AnimationState // This is only client side
{
    public float AnimationTimer = 0f;
    public int State = 0;
    public int Rotation = 0;

    public void ProcessAnimationRotation(Vector2 vVelocity, InputState input)
    {
        float velocity = 0;
        if (vVelocity.X >= 0)
            velocity += vVelocity.X;
        else
            velocity -= vVelocity.X;

        var animationVelocity = 0.1f;
        if (velocity > 500) animationVelocity = 0.05f;
        if (AnimationTimer > animationVelocity && velocity > 20)
        {
            if (State == 0)
            {
                Rotation = 8;
                State = 1;
            }
            else if (State == 1)
            {
                Rotation = 0;
                State = 2;
            }
            else if (State == 2)
            {
                Rotation = -8;
                State = 3;
            }
            else if (State == 3)
            {
                Rotation = 0;
                State = 0;
            }
            AnimationTimer = 0f;
        }
        else if (velocity <= 20)
        {
            Rotation = 0;
        }

        if (input.Left || input.Right)
        {
            AnimationTimer += Raylib.GetFrameTime();
        }
        else
        {
            AnimationTimer = 0;
            Rotation = 0;
            State = 0;
        }
    }

    public int GetAnimationRotation()
    {
        return Rotation;
    }

}
