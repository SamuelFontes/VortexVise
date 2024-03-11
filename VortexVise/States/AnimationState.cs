using ZeroElectric.Vinculum;
using System.Numerics;

namespace VortexVise.States;

public class AnimationState
{
    public float AnimationTimer = 0f;
    public int State = 0;
    public int Rotation = 0;
    Vector2 Deformation = new Vector2(1, 1);

    public int GetAnimationRotation(Vector2 vVelocity, InputState input)
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
        //Utils.SetDebugString($"v{(int)velocity}s:{State},r:{Rotation},t:{AnimationTimer},");
        return Rotation;
    }
}
