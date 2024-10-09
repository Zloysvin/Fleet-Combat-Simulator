using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;   // Speed of camera movement
    public float zoomSpeed = 5f;   // Speed of camera zoom
    public Vector2 minBounds;      // Minimum bounds for the camera
    public Vector2 maxBounds;      // Maximum bounds for the camera

    void Update()
    {
        // Camera movement
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        Vector3 moveDelta = moveInput * moveSpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position + moveDelta;

        // Clamp target position to stay within specified bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        // Smoothly interpolate between current and target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);

        // Camera zoom
        float zoomDelta = Mathf.Clamp(Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime, 1f, 10f);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomDelta, 0.4f);
    }
}