using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float distanceFromPlayer = 10f;
    public float rotationSpeed = 5f;
    public float zoomSpeed = 500f;
    public LayerMask groundLayer;
    public float groundCheckOffset = 10f; // Height offset for ground check

    private float currentYAngle = 0f;
    private float currentXAngle = 0f;
    private Vector3 previousPosition;
    public float verticalOffset = 2f;
    public float smoothSpeed = 0.125f;
    void Update()
    {
        // Zoom with mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distanceFromPlayer -= scroll * zoomSpeed * Time.deltaTime;
        distanceFromPlayer = Mathf.Clamp(distanceFromPlayer, 5f, 80f);

        if (Input.GetMouseButtonDown(1)) // Right-click pressed
        {
            previousPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // Right-click held down
        {
            Vector3 direction = previousPosition - Input.mousePosition;

            // Calculating new angles
            currentXAngle += direction.x * rotationSpeed * Time.deltaTime;
            currentYAngle -= direction.y * rotationSpeed * Time.deltaTime;
            currentYAngle = Mathf.Clamp(currentYAngle, -30f, 80f); // Limit vertical angle

            Quaternion potentialRotation = Quaternion.Euler(currentYAngle, currentXAngle, 0);
            Vector3 potentialPosition = player.position - (potentialRotation * Vector3.forward * distanceFromPlayer);

            // Smooth position transition
            transform.position = Vector3.Lerp(transform.position, potentialPosition, smoothSpeed * Time.deltaTime);

            // Smooth rotation transition
            transform.rotation = Quaternion.Slerp(transform.rotation, potentialRotation, smoothSpeed * Time.deltaTime);

            // Raycast to check ground position
            Vector3 groundCheckPoint = player.position + Vector3.up * groundCheckOffset;
            RaycastHit hit;
            if (Physics.Raycast(groundCheckPoint, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                float groundHeight = hit.point.y;
                potentialPosition.y = Mathf.Max(potentialPosition.y, groundHeight + 1.0f) + verticalOffset; // Adding vertical offset here

                transform.rotation = potentialRotation;
                transform.position = potentialPosition;
            }

            previousPosition = Input.mousePosition;
        }

        // Ensure the camera is looking at the player
        transform.LookAt(player.position);
    }


}
