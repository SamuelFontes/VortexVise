using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // FIXME: change levelborders accoding to level selected
    public Transform target;
    public float smoothTime = 0.25f;

    private Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;

    private float maxLevelBorderX = 41f;
    public float minLevelBorderX = -41f;

    private float maxLevelBorderY = 41f;
    private float minLevelBorderY = -41f;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        var rightTop = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth,cam.pixelHeight,cam.nearClipPlane));  

        var cameraOffsetX = Vector2.Distance(new Vector2(transform.position.x,0),  new Vector2(rightTop.x,0));
        var cameraOffsetY = Vector2.Distance(new Vector2(transform.position.y,0),  new Vector2(rightTop.y,0));

        Vector3 targetPosition = target.position;

        // Check if Camera is outside map, if it is it will autocorrect
        if(targetPosition.x+(cameraOffsetX*-1) < minLevelBorderX)
        {
            targetPosition.x = transform.position.x;
            targetPosition.x += (minLevelBorderX - (targetPosition.x+(cameraOffsetX*-1)));
        } else if(targetPosition.x + cameraOffsetX > maxLevelBorderX)
        {
            targetPosition.x = transform.position.x;
            targetPosition.x += maxLevelBorderX - (targetPosition.x+ cameraOffsetX);
        }

        if(targetPosition.y+(cameraOffsetY*-1) < minLevelBorderY)
        {
            targetPosition.y = transform.position.y;
            targetPosition.y += (minLevelBorderY - (targetPosition.y+(cameraOffsetY*-1)));
        } else if(targetPosition.y + cameraOffsetY > maxLevelBorderY)
        {
            targetPosition.y = transform.position.y;
            targetPosition.y += maxLevelBorderY - (targetPosition.y+ cameraOffsetY);
        }

        targetPosition += offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

}
