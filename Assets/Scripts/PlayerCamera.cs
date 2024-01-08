using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // TODO: Create camera shake method
    public Transform target;
    public float smoothTime = 0.25f;

    public Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;

    Vector3 OriginalPos; 

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        OriginalPos = transform.position;
    }

    private void FixedUpdate()
    {
        float maxLevelBorderX = GameState.CurrentMap.topRight.x;
        float minLevelBorderX = GameState.CurrentMap.bottomLeft.x;

        float maxLevelBorderY = GameState.CurrentMap.topRight.y;
        float minLevelBorderY = GameState.CurrentMap.bottomLeft.y;

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

        targetPosition.z = OriginalPos.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void StartShake(float dur, float mag)
    {
        StartCoroutine(Shake(dur, mag));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position += new Vector3(x, y, 0);
                
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
