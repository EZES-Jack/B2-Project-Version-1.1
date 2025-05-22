using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{

    private Vector3 _offset;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 levelMinBounds; // Minimum x and z of the level
    [SerializeField] private Vector2 levelMaxBounds; // Maximum x and z of the level
    private Vector3 _currentVelocity = Vector3.zero;
    private Camera _camera;
    
    

    private void Awake()
    {
        _offset = transform.position - target.position;
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + _offset;

        // Calculate the camera's half-width and half-height
        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;

        // Clamp the target position to keep the level edges within the camera's view
        float clampedX = Mathf.Clamp(targetPosition.x, levelMinBounds.x + halfWidth, levelMaxBounds.x - halfWidth);
        float clampedZ = Mathf.Clamp(targetPosition.z, levelMinBounds.y + halfHeight, levelMaxBounds.y - halfHeight);

        // Smoothly move the camera to the clamped position
        Vector3 clampedPosition = new Vector3(clampedX, transform.position.y, clampedZ);
        transform.position = Vector3.SmoothDamp(transform.position, clampedPosition, ref _currentVelocity, smoothTime);
    }
    
}