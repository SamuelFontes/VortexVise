using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float _cameraSmoothDelay = 0.35f;

    private Transform _target;
    private Vector3 _cameraOffset = new Vector3(0, 0, -10);
    private Vector3 _velocity = Vector3.zero;
    private Camera _camera;
    private AudioListener _audioListener;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _audioListener = GetComponent<AudioListener>();
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = _target.position;
        targetPosition = CorrectIfOutsideTheMap(targetPosition);
        targetPosition += _cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _cameraSmoothDelay);
    }

    public void StartShake(float dur, float mag)
    {
        StartCoroutine(Shake(dur, mag));
    }

    private IEnumerator Shake(float duration, float magnitude)
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

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetupCamera(Rect rect, float cameraDistance, bool hasAudioListener)
    {
        _audioListener.enabled = hasAudioListener; // Setup audio listener, it should have only one
        _camera.rect = rect;
        _camera.orthographicSize = cameraDistance;
    }
    
    private Vector3 CorrectIfOutsideTheMap(Vector3 targetPosition)
    {
        // Check if Camera is outside map, if it is it will move the camera back to the map
        float maxLevelBorderX = GameState.Instance.CurrentMap.topRight.x;
        float minLevelBorderX = GameState.Instance.CurrentMap.bottomLeft.x;
        float maxLevelBorderY = GameState.Instance.CurrentMap.topRight.y;
        float minLevelBorderY = GameState.Instance.CurrentMap.bottomLeft.y;
        var rightTop = _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth,_camera.pixelHeight,_camera.nearClipPlane));  
        var cameraOffsetX = Vector2.Distance(new Vector2(transform.position.x,0),  new Vector2(rightTop.x,0));
        var cameraOffsetY = Vector2.Distance(new Vector2(transform.position.y,0),  new Vector2(rightTop.y,0));


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
        return targetPosition;
    }
}
