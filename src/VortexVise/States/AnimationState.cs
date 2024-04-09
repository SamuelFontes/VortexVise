﻿using System.Numerics;

namespace VortexVise.States;

/// <summary>
/// Used to animate sprites when players are walking around.
/// </summary>
public class AnimationState // This is only client side
{
    public float AnimationTimer = 0f;
    public int State = 0;
    public float Rotation = 0;
    public bool IsDashing = false;
    public bool IsDashFacingRight = false;

    public void ProcessAnimationRotation(Vector2 vVelocity, InputState input, float deltaTime)
    {
        if (IsDashing) return;

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
            AnimationTimer += deltaTime;
        }
        else
        {
            AnimationTimer = 0;
            Rotation = 0;
            State = 0;
        }
    }

    public void ProcessDash(float deltaTime)
    {
        var rotationForce = deltaTime * 1800;
        if (!IsDashing) return;
        if (Rotation < -360 || Rotation > 360)
        {
            AnimationTimer = 0;
            IsDashing = false;
            Rotation = 0;
        }
        else
        {
            if (IsDashFacingRight)
                Rotation += rotationForce;
            else
                Rotation -= rotationForce;
        }

    }

    public int GetAnimationRotation()
    {
        return (int)Rotation;
    }

}