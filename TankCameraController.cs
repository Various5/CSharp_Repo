using UnityEngine;

public class TankCameraController : MonoBehaviour
{
    public GameObject insideTankCamera;
    public GameObject outsideTankCamera;
    public GameObject freeRoamCamera; // Add this line for the free roam camera
    public Transform tankTransform;

    private GameObject currentCamera;
    private float freeRoamDistance = 10f;
    private float minDistance = 5f;
    private float maxDistance = 15f;
    private Vector3 freeRoamOffset;
    private float rotationSpeed = 5.0f;
    private float zoomSpeed = 10.0f;

    void Start()
    {
        // Initialize the camera with the inside view
        SwitchToInsideView();
    }

    void Update()
    {
        // Switching camera views based on key presses
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToInsideView();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToOutsideView();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToFreeRoamView();
        }

        // Update the free roam camera position if it's the active camera
        if (currentCamera == freeRoamCamera)
        {
            UpdateFreeRoamCameraPosition();
        }
    }

    void SwitchToInsideView()
    {
        insideTankCamera.SetActive(true);
        outsideTankCamera.SetActive(false);
        freeRoamCamera.SetActive(false); // Add this line
        currentCamera = insideTankCamera;
    }

    void SwitchToOutsideView()
    {
        insideTankCamera.SetActive(false);
        outsideTankCamera.SetActive(true);
        freeRoamCamera.SetActive(false); // Add this line
        currentCamera = outsideTankCamera;
    }

    void SwitchToFreeRoamView()
    {
        insideTankCamera.SetActive(false);
        outsideTankCamera.SetActive(false);
        freeRoamCamera.SetActive(true); // Modify this line
        currentCamera = freeRoamCamera; // Modify this line
        freeRoamOffset = new Vector3(0, 2, -freeRoamDistance);
    }

    void UpdateFreeRoamCameraPosition()
    {
        // Ensure the camera is the free roam camera
        if (currentCamera != freeRoamCamera)
            return;

        // Control the orbit around the tank
        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            float h = rotationSpeed * Input.GetAxis("Mouse X");
            float v = rotationSpeed * Input.GetAxis("Mouse Y");
            freeRoamOffset = Quaternion.AngleAxis(h, Vector3.up) * Quaternion.AngleAxis(v, Vector3.right) * freeRoamOffset;
        }

        // Control the zoom
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        freeRoamDistance = Mathf.Clamp(freeRoamDistance - zoomDelta, minDistance, maxDistance);
        freeRoamOffset = freeRoamOffset.normalized * freeRoamDistance;

        // Calculate desired position with collision detection
        Vector3 desiredPosition = tankTransform.position + freeRoamOffset;
        RaycastHit hit;
        if (Physics.Linecast(tankTransform.position, desiredPosition, out hit))
        {
            desiredPosition = hit.point;
        }

        // Move the camera smoothly to the desired position
        freeRoamCamera.transform.position = Vector3.Lerp(freeRoamCamera.transform.position, desiredPosition, Time.deltaTime * 5);
        freeRoamCamera.transform.LookAt(tankTransform.position);
    }
}
