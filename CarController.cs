using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{


    public enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }

    [Header("Car Health")]
    public float carHealth = 100f;
    public Image healthBarFill; // Assign this in the inspector

    public DriveType driveType;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    public float maxTorque = 1500f;
    public float maxSteerAngle = 40f;
    public float maxBrakeTorque = 8000f;
    public float downforce = 200f;
    public float topSpeed = 200f;

    public float spring = 35000f;
    public float damper = 4500f;
    public float wheelBase;
    public float rearTrack;
    public float frontTrack;

    public Skidmarks skidmarksController;
    private int[] lastSkidId = new int[4];
    private bool[] wheelInBlood = new bool[4];

    public AnimationCurve powerCurve = new AnimationCurve(
        new Keyframe(0, 1),
        new Keyframe(0.5f, 0.8f),
        new Keyframe(1, 0.6f));

    private Rigidbody rb;
    private float currentSpeed;
    private float driftFactor = 0.5f;

    public Text speedometerText;

    private Color normalSkidColor = Color.black;
    private Color bloodSkidColor = Color.red;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // Player interaction variable
    public bool isPlayerInCar = false;

    public Camera carCamera; // Reference to the car's camera
    public Camera playerCamera; // Reference to the player's camera
    public GameObject player; // Reference to the player GameObject



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.9f, 0);
        rb.mass = 1500;
        ConfigureSuspension();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void ConfigureSuspension()
    {
        JointSpring springSetting = new JointSpring
        {
            spring = this.spring,
            damper = this.damper
        };

        frontLeftWheelCollider.suspensionSpring = springSetting;
        frontRightWheelCollider.suspensionSpring = springSetting;
        rearLeftWheelCollider.suspensionSpring = springSetting;
        rearRightWheelCollider.suspensionSpring = springSetting;
    }

    void Update()
    {
        if (isPlayerInCar)
        {
            UpdateWheelMeshesPositions();
            UpdateSpeedometer();

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCarPosition();
            }
        }
    }

    void FixedUpdate()
    {
        if (isPlayerInCar)
        {
            float steer = Input.GetAxis("Horizontal");
            float accelerate = Input.GetAxis("Vertical");
            bool handbrakePressed = Input.GetKey(KeyCode.Space);
            float brake = Input.GetAxis("Brake");

            HandleSteering(steer);
            HandleMotor(accelerate);
            HandleBraking(brake);
            HandleHandbrake(handbrakePressed);

            rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);

            Vector3 driftForce = -transform.right * steer * currentSpeed * driftFactor;
            rb.AddForce(driftForce, ForceMode.Force);

            CheckForSkidmarks();
        }
        else
        {
            ApplyTorqueToAllWheels(0); // Stop car if the player is not in control
        }
    }

    private void HandleSteering(float steer)
    {
        float steerAngle = steer * maxSteerAngle;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }

    private void HandleMotor(float accelerate)
    {
        float torque = accelerate * maxTorque * powerCurve.Evaluate(currentSpeed / topSpeed);

        switch (driveType)
        {
            case DriveType.AllWheelDrive:
                ApplyTorqueToAllWheels(torque);
                break;
            case DriveType.RearWheelDrive:
                ApplyTorqueToRearWheels(torque);
                break;
            case DriveType.FrontWheelDrive:
                ApplyTorqueToFrontWheels(torque);
                break;
        }

        currentSpeed = rb.velocity.magnitude * 3.6f; // Convert m/s to km/h
        if (currentSpeed > topSpeed)
        {
            rb.velocity = (topSpeed / 3.6f) * rb.velocity.normalized;
        }
    }

    private void ApplyTorqueToAllWheels(float torque)
    {
        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;
        rearLeftWheelCollider.motorTorque = torque;
        rearRightWheelCollider.motorTorque = torque;
    }

    private void ApplyTorqueToFrontWheels(float torque)
    {
        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;
    }

    private void ApplyTorqueToRearWheels(float torque)
    {
        rearLeftWheelCollider.motorTorque = torque;
        rearRightWheelCollider.motorTorque = torque;
    }

    private void HandleHandbrake(bool handbrakePressed)
    {
        if (handbrakePressed)
        {
            float handbrakeTorque = maxBrakeTorque;
            rearLeftWheelCollider.brakeTorque = handbrakeTorque;
            rearRightWheelCollider.brakeTorque = handbrakeTorque;
            driftFactor = 1.0f;
        }
        else
        {
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;
            driftFactor = 0.5f;
        }
    }

    private void HandleBraking(float brake)
    {
        float brakeTorque = brake * maxBrakeTorque;
        frontLeftWheelCollider.brakeTorque = brakeTorque;
        frontRightWheelCollider.brakeTorque = brakeTorque;
        rearLeftWheelCollider.brakeTorque = brakeTorque;
        rearRightWheelCollider.brakeTorque = brakeTorque;
    }

    private void UpdateWheelMeshesPositions()
    {
        UpdateWheelPosition(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPosition(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPosition(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelPosition(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheelPosition(WheelCollider collider, Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;
    }

    private void UpdateSpeedometer()
    {
        if (speedometerText != null)
        {
            speedometerText.text = "Speed: " + Mathf.RoundToInt(currentSpeed) + " km/h";
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name); // Debugging

        if (collision.gameObject.CompareTag("Enemy"))
        {
            float speed = rb.velocity.magnitude;
            ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();

            if (zombie != null)
            {
                float damage = CalculateDamage(speed);
                zombie.ApplyDamage(damage);
                ApplyDamageToCar(damage); // Apply some damage to the car as well
            }
        }
    }

    private void ApplyDamageToCar(float damage)
    {
        carHealth -= damage;
        UpdateHealthBar();
        if (carHealth <= 0)
        {
            // Handle the destruction of the car, game over, etc.
        }
    }
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = carHealth / 100f;
        }
        else
        {
            Debug.LogWarning("Health bar fill image is not assigned in the inspector.");
        }
    }
    private float CalculateDamage(float speed)
    {
        // You can adjust these values and add more complexity if you want
        if (speed > 20 / 3.6f) // Speed in m/s (20 km/h)
        {
            return 100; // Instant kill
        }
        else if (speed > 10 / 3.6f)
        {
            return carHealth * 0.03f; // 3% damage
        }
        else if (speed > 5 / 3.6f)
        {
            return carHealth * 0.01f; // 1% damage
        }

        return 0;
    }

    public void StopCar()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    IEnumerator TemporarilyColorSkidmarks(int wheelIndex, Color color, float duration)
    {
        wheelInBlood[wheelIndex] = true;
        yield return new WaitForSeconds(duration);
        wheelInBlood[wheelIndex] = false;
        lastSkidId[wheelIndex] = -1;
    }

    public void ResetCarPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        StopCar();
    }

    private void CheckForSkidmarks()
    {
        CheckWheelSkid(frontLeftWheelCollider, 0);
        CheckWheelSkid(frontRightWheelCollider, 1);
        CheckWheelSkid(rearLeftWheelCollider, 2);
        CheckWheelSkid(rearRightWheelCollider, 3);
    }

    private void CheckWheelSkid(WheelCollider wheel, int index)
    {
        WheelHit wheelHit;
        if (wheel.GetGroundHit(out wheelHit))
        {
            float slip = Mathf.Abs(wheelHit.sidewaysSlip);
            if (slip > 0.4f) // Adjust the slip threshold as needed
            {
                Vector3 skidPoint = wheel.transform.position - (wheel.transform.up * wheel.radius);
                float intensity = Mathf.Clamp01(slip / 1.0f); // Example calculation for intensity
                lastSkidId[index] = skidmarksController.AddSkidMark(skidPoint, wheelHit.normal, intensity, lastSkidId[index]);
            }
            else
            {
                lastSkidId[index] = -1;
            }
        }
    }

    public void EnterCar()
    {
        isPlayerInCar = true;

        // Enable car controls and camera
        this.enabled = true;
        if (carCamera != null)
        {
            carCamera.enabled = true;
            carCamera.GetComponent<AudioListener>().enabled = true; // Enable the car's AudioListener
        }

        // Disable player controls and camera
        if (player != null)
        {
            player.SetActive(false);
            if (playerCamera != null)
            {
                playerCamera.GetComponent<AudioListener>().enabled = false; // Disable the player's AudioListener
            }
        }
    }

    public void ExitCar()
    {
        isPlayerInCar = false;

        // Disable car controls and camera
        this.enabled = false;
        if (carCamera != null)
        {
            carCamera.enabled = false;
            carCamera.GetComponent<AudioListener>().enabled = false; // Disable the car's AudioListener
        }

        // Enable player controls and camera
        if (player != null)
        {
            player.SetActive(true);
            if (playerCamera != null)
            {
                playerCamera.GetComponent<AudioListener>().enabled = true; // Enable the player's AudioListener
            }
            player.transform.position = this.transform.position + this.transform.right * 2; // Adjust as needed
        }
    }
}
