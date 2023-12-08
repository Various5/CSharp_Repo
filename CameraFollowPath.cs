using UnityEngine;

public class DynamicCameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform[] pathPoints;
    private int currentPointIndex = 0;

    public float speed = 5.0f;
    public float rotationSpeed = 50.0f;
    public float freeFlightSpeed = 5.0f;
    public float mouseSensitivity = 100.0f;
    public float pauseDuration = 2.0f;

    private float rotationTimer;
    private float pauseTimer;
    private bool isPaused = false;
    private bool inFreeFlightMode = false;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void Start()
    {
        SetRandomRotationTimer();
        pauseTimer = pauseDuration;
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            inFreeFlightMode = !inFreeFlightMode;
        }

        if (inFreeFlightMode)
        {
            HandleFreeFlightMode();
        }
        else
        {
            HandleFollowMode();
        }
    }

    void HandleFollowMode()
    {
        if (target == null || pathPoints.Length == 0) return;

        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                isPaused = false;
                SetRandomRotationTimer();
                pauseTimer = pauseDuration;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, speed * Time.deltaTime);

            rotationTimer -= Time.deltaTime;
            if (rotationTimer <= 0)
            {
                transform.Rotate(0, Random.Range(-rotationSpeed, rotationSpeed), 0);
                SetRandomRotationTimer();
            }

            if (transform.position == pathPoints[currentPointIndex].position)
            {
                currentPointIndex++;
                if (currentPointIndex >= pathPoints.Length)
                {
                    currentPointIndex = 0;
                }
                isPaused = true;
            }
        }

        transform.LookAt(target);
    }

    void HandleFreeFlightMode()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        direction = transform.TransformDirection(direction);
        transform.position += direction * freeFlightSpeed * Time.deltaTime;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    void SetRandomRotationTimer()
    {
        rotationTimer = Random.Range(1f, 5f);
    }
}
