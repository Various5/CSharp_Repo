using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TankController : MonoBehaviour
{
    // Tank movement parameters
    public float maxSpeed = 12f;
    public float turnSpeed = 180f;

    // Audio components
    public AudioSource movementAudio;
    public AudioClip engineIdling;
    public AudioClip engineDriving;
    public float pitchRange = 0.2f;

    // Health and armor parameters
    public float armor = 100f;
    public float health = 100f;

    // Shooting fields
    public GameObject artilleryShellPrefab; // Prefab of the ArtilleryShell
    public Transform shellSpawnPoint; // Point where the shell is spawned
    public float shellLaunchForce = 20f; // Force applied to the shell when fired


    // UI components
    public TMP_Text healthText;
    public TMP_Text armorText;
    public Image healthFill;
    public Image armorFill;

    // Other components
    public GameObject explosionEffect;

    // Movement values
    private float movementInputValue;
    private float turnInputValue;
    private float originalPitch;

    // Rigidbody for movement
    private Rigidbody rigidbodyComponent;

    // Barrel control fields
    public Transform barrelTransform; // Reference to the barrel transform
    public float barrelRotationSpeed = 10f; // Speed at which the barrel rotates
    public float maxElevationAngle = 30f; // Max elevation angle
    public float minDepressionAngle = -5f; // Max depression angle

    private float currentBarrelRotation = 0f; // Current rotation of the barrel

    public AudioClip shootingSound;
    private AudioSource shootingAudioSource;

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        shootingAudioSource = gameObject.AddComponent<AudioSource>();
        shootingAudioSource.clip = shootingSound;
        originalPitch = movementAudio.pitch;
    }

    private void OnEnable()
    {
        rigidbodyComponent.isKinematic = false;
        ResetInputValues();
    }

    private void OnDisable()
    {
        rigidbodyComponent.isKinematic = true;
    }

    private void Update()
    {
        movementInputValue = Input.GetAxis("Vertical");
        turnInputValue = Input.GetAxis("Horizontal");

        EngineAudio();
        UpdateUI();

        HandleBarrelRotation();

        if (Input.GetButtonDown("Fire1")) // Replace "Fire1" with your fire button
        {
            Fire();
        }

    }

    private void HandleBarrelRotation()
    {
        // Get input for barrel rotation from mouse Y-axis
        float barrelInput = Input.GetAxis("Mouse Y");
        currentBarrelRotation += barrelInput * barrelRotationSpeed * Time.deltaTime; // Adjust axis if necessary
        currentBarrelRotation = Mathf.Clamp(currentBarrelRotation, 0, maxElevationAngle); // Prevents barrel from going below initial position

        // Apply the rotation to the barrel
        barrelTransform.localEulerAngles = new Vector3(-currentBarrelRotation, 0f, 0f);
    }
    private void EngineAudio()
    {
        // Check if the tank is moving significantly in either forward/backward or turning
        if (Mathf.Abs(movementInputValue) < 0.1f && Mathf.Abs(turnInputValue) < 0.1f)
        {
            // If the tank is stationary and the engine driving clip is currently playing
            if (movementAudio.clip == engineDriving)
            {
                // Change the audio clip to the engine idling
                movementAudio.clip = engineIdling;
                movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
        else
        {
            // If the tank is moving and the engine idling clip is currently playing
            if (movementAudio.clip == engineIdling)
            {
                // Change the audio clip to the engine driving
                movementAudio.clip = engineDriving;
                movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
    }


    private void Fire()
    {
        shootingAudioSource.Play();
        // Instantiate the shell and set its orientation
        GameObject shellInstance = Instantiate(artilleryShellPrefab, shellSpawnPoint.position, barrelTransform.rotation);

        ArtilleryShell shellComponent = shellInstance.GetComponent<ArtilleryShell>();
        if (shellComponent != null)
        {
            shellComponent.SetTerrainDeformer(TerrainDeformer.Instance);
        }
    }
    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        Vector3 movement = transform.forward * movementInputValue * maxSpeed * Time.deltaTime;
        rigidbodyComponent.MovePosition(rigidbodyComponent.position + movement);
    }

    private void Turn()
    {
        float turn = turnInputValue * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rigidbodyComponent.MoveRotation(rigidbodyComponent.rotation * turnRotation);
    }

    public void TakeDamage(float damage)
    {
        if (armor > 0)
        {
            armor -= damage;
            if (armor < 0)
            {
                health += armor; // Subtract any excess damage from health
                armor = 0;
            }
        }
        else
        {
            health -= damage;
        }

        if (health <= 0)
        {
            Explode();
        }
    }
    private void UpdateUI()
    {
        healthText.text = $"Health: {health}";
        armorText.text = $"Armor: {armor}";
        healthFill.fillAmount = health / 100f;
        armorFill.fillAmount = armor / 100f;
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void ResetInputValues()
    {
        movementInputValue = 0f;
        turnInputValue = 0f;
    }
}
