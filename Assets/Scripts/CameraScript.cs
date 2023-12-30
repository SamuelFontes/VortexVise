using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // FIXME: Make the level camera size less hacky
    public Transform target;
    public float smoothTime = 0.25f;

    private Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;

    private float maxLevelBorderX = 22f;
    private float minLevelBorderX = -16f;

    private float maxLevelBorderY = 36f;
    private float minLevelBorderY = -21f;

    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        if(targetPosition.x < minLevelBorderX)
            targetPosition.x = minLevelBorderX;

        if (targetPosition.x > maxLevelBorderX)
            targetPosition.x = maxLevelBorderX;

        if (targetPosition.y < minLevelBorderY)
            targetPosition.y = minLevelBorderY;

        if(targetPosition.y > maxLevelBorderY)
            targetPosition.y = maxLevelBorderY;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

}
