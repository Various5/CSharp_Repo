using UnityEngine;

public class DynamicCarCamera : MonoBehaviour
{
    public Transform target; // The car to follow

    [Header("Camera Mode Settings")]
    public Vector3 followOffset = new Vector3(0, 5.0f, -10f); // Offset for the follow view
    public Vector3 interiorOffset; // Offset for the interior view
    public Vector3 tireOffset; // Offset for the tire view
    public Vector3 backViewOffset = new Vector3(0, 2.0f, 5.0f); // Offset for the back view

    [Header("Free Camera Settings")]
    public float zoomSpeed = 10f; // Zoom sensitivity
    public float minZoom = 1f; // Minimum zoom distance
    public float maxZoom = 10f; // Maximum zoom distance
    public float pitch = 2f; // The angle at which the camera will pitch down
    public float pitchSpeed = 100f; // Pitch sensitivity
    public float yawSpeed = 100f; // Yaw sensitivity
    public float minPitch = -20f; // The min pitch angle you can look at the target
    public float maxPitch = 80f; // The max pitch angle you can look at the target
    public LayerMask groundLayer; // Ground layer mask to check for ground collision

    private enum CameraMode { Follow, Free, Interior, Tire, BackView }
    private CameraMode cameraMode = CameraMode.Follow;

    private float currentZoom = 5f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraMode = (CameraMode)(((int)cameraMode + 1) % 5);
        }

        if (cameraMode == CameraMode.Free)
        {
            currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            currentYaw += Input.GetAxis("Mouse X") * yawSpeed * Time.deltaTime;
            currentPitch -= Input.GetAxis("Mouse Y") * pitchSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }
    }

    private void LateUpdate()
    {
        switch (cameraMode)
        {
            case CameraMode.Follow:
                FollowCamera();
                break;
            case CameraMode.Free:
                FreeCamera();
                break;
            case CameraMode.Interior:
                SetCameraOffset(interiorOffset);
                break;
            case CameraMode.Tire:
                SetCameraOffset(tireOffset);
                break;
            case CameraMode.BackView:
                SetCameraOffset(backViewOffset);
                break;
        }
    }

    private void FollowCamera()
    {
        SetCameraOffset(followOffset);
    }

    private void FreeCamera()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch + pitch, currentYaw, 0);
        Vector3 positionOffset = rotation * new Vector3(0, 0, -currentZoom);
        transform.position = target.position + positionOffset;
        transform.LookAt(target.position + Vector3.up * pitch);
        PreventGroundClip();
    }

    private void SetCameraOffset(Vector3 offset)
    {
        transform.position = target.TransformPoint(offset);
        transform.LookAt(target);
    }

    private void PreventGroundClip()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, groundLayer))
        {
            if (hit.distance < pitch + 1f)
            {
                Vector3 pos = transform.position;
                pos.y = target.position.y + (pitch + 1f);
                transform.position = pos;
            }
        }
    }

    // Draw Gizmos in the editor to help with setup
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(target.TransformPoint(followOffset), 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.TransformPoint(interiorOffset), 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(target.TransformPoint(tireOffset), 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.TransformPoint(backViewOffset), 0.5f);
        }
    }
}
