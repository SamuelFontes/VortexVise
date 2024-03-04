using System.Numerics;
using Raylib_cs;
using VortexVise.Utilities;

namespace VortexVise;

public class AnimationState
{
    public float AnimationTimer = 0f;
    public int State = 0;
    public int Rotation = 0;

    public int GetRotationAnimation(Vector2 vVelocity)
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
                // _skin.transform.Rotate(new Vector3(0, 0, 9));
                // _skin.transform.localPosition = _skin.transform.localPosition + Vector3.up * 0.1f;
                Rotation = 9;
                State = 1;
            }
            else if (State == 1)
            {
                // _skin.transform.localRotation = Quaternion.identity;
                // _skin.transform.localPosition = Vector3.zero;
                Rotation = 0;
                State = 2;
            }
            else if (State == 2)
            {
                // _skin.transform.localPosition = _skin.transform.localPosition + Vector3.up * 0.1f;
                // _skin.transform.Rotate(new Vector3(0, 0, -9));
                Rotation = -9;
                State = 3;
            }
            else if (State == 3)
            {
                // _skin.transform.localRotation = Quaternion.identity;
                // _skin.transform.localPosition = Vector3.zero;
                Rotation = 0;
                State = 0;
            }
            AnimationTimer = 0f;
        }

        if (velocity > 20)
        {
            AnimationTimer += Raylib.GetFrameTime();
        }
        else
        {
            AnimationTimer = 0;
            // _skin.transform.Rotate(new Vector3(0, 0, 0));
            Rotation = 0;
            State = 0;
            // _skin.transform.localRotation = Quaternion.identity;
            // _skin.transform.localPosition = Vector3.zero;
        }
        //AnimationBounce();
        Utils.SetDebugString($"v{(int)velocity}s:{State},r:{Rotation},t:{AnimationTimer},");
        return Rotation;
    }

    // void AnimationBounce()
    // {
    //     var defaultScaleX = 0.5f;
    //     var defaultScaleY = 0.5f;
    //     var amount = Raylib.GetFrameTime() * _bounciness;
    //     if(_horizontalMovement != 0)
    //     {
    //         if (_animationState == 0 || _animationState == 2)
    //         {
    //             _skin.transform.localScale += new Vector3(amount,0f);
    //             if(_skin.transform.localScale.y >= defaultScaleY)
    //                 _skin.transform.localScale += new Vector3(0,-(amount * 2));
    //         }
    //         else if (_animationState == 1 || _animationState == 3)
    //         {
    //             if(_skin.transform.localScale.x >= defaultScaleX)
    //                 _skin.transform.localScale += new Vector3(-(amount * 2),0f);
    //             _skin.transform.localScale += new Vector3(0,amount);
    //         }
    //     }
    //     else if(_skin.transform.localScale != new Vector3(defaultScaleX,defaultScaleY))
    //     {
    //         _skin.transform.localScale = new Vector3(defaultScaleX,defaultScaleY);
    //     }

    // }

}
