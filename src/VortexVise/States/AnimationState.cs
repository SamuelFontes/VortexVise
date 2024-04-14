using System.Numerics;
using VortexVise.Models;

namespace VortexVise.States;

/// <summary>
/// Used to animate sprites when players are walking around. Also used to animate things like explosions
/// </summary>
public class AnimationState // This is only client side
{
    // Using this for character animation and sprite animation is probably a bad idea, but I should just keep going.
    public float AnimationTimer = 0f;
    public int State = 0;
    public float Rotation = 0;
    public bool IsDashing = false;
    public bool IsDashFacingRight = false;
    public bool Loop = false;
    public bool ShouldDisappear = false;
    public Vector2 Position = new(0, 0);
    public Animation? Animation { get; set; }

    /// <summary>
    /// This is used to process the player animation
    /// </summary>
    /// <param name="vVelocity">Player velocity</param>
    /// <param name="input">Player input state</param>
    /// <param name="deltaTime">Delta time</param>
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

    /// <summary>
    /// Summer salt animation
    /// </summary>
    /// <param name="deltaTime"></param>
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

    /// <summary>
    /// Cast rotation to int 
    /// </summary>
    /// <returns>Rotation</returns>
    public int GetAnimationRotation()
    {
        return (int)Rotation;
    }

    public void Animate(float deltaTime)
    {
        if (Animation == null) return;

        AnimationTimer += deltaTime;
        if (AnimationTimer > Animation.FrameTime) 
        {
            AnimationTimer = 0;
            if (State < Animation.StateAmount - 1)
            {
                State++;
            }
            else if (Loop)
            {
                State = 0;
            }
            else
            {
                ShouldDisappear = true;
            }
        }
    }
}
