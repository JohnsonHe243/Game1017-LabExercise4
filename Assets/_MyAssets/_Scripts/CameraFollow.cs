using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Player.
    private float leftBound;
    private float rightBound;

    void Start()
    {
        // Calculate the boundaries for camera movement.
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
        leftBound = -cameraWidth / 3f;
        rightBound = cameraWidth / 3f;
    }

    void LateUpdate()
    {
        // Get current positions for target and camera.
        Vector3 targetPosition = target.position;
        Vector3 newPosition = transform.position;
        // Calculate new bounds.
        float newLeftBound = newPosition.x + leftBound; // leftBound is negative.
        float newRightBound = newPosition.y + rightBound; 
        // Check if target has moved into the left bound. Camera scroll. Not really a follow.
        if (targetPosition.x < newLeftBound)
        {
            transform.position = new Vector3(targetPosition.x - leftBound, newPosition.y, newPosition.z);
        }
        else if (targetPosition.x >= newRightBound)
        {
            transform.position = new Vector3(targetPosition.x - rightBound, newPosition.y, newPosition.z);
        }
    }
}
