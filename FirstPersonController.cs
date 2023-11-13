using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Player Attributes")]
    public float health = 100f;
    public float stamina = 100f;
    public float hunger = 100f;
    public float thirst = 100f;

    [Header("Attribute Decay Rates")]
    public float hungerDecayRate = 1f;
    public float thirstDecayRate = 1.5f;

    [Header("Movement Parameters")]
    public float movementSpeed = 5.0f;
    public float sprintSpeed = 7.5f;
    public float jumpHeight = 2.0f;
    public float gravityValue = -9.81f;

    [Header("Camera Settings")]
    public float lookSensitivity = 2f;
    public float lookSmoothDamp = 0.1f;
    public Camera playerCamera;

    [Header("Inventory")]
    public GameObject[] inventory = new GameObject[2]; // 0: Melee, 1: Ranged

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private float rotationX = 0f;
    private float currentCameraRotationX = 0f;
    private float cameraRotationVelocity = 0f;

    private float nextHungerDecayTime = 0f;
    private float nextThirstDecayTime = 0f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleHunger();
        HandleThirst();
        HandleInventorySelection();
        HandleAttacks();
    }

    void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : movementSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        currentCameraRotationX = Mathf.SmoothDamp(currentCameraRotationX, rotationX, ref cameraRotationVelocity, lookSmoothDamp);
        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }
    public void ApplyDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Handle player death here
        }
    }
    public void PickupWeapon(GameObject weaponPrefab, bool isMelee)
    {
        int index = isMelee ? 0 : 1;

        GameObject newWeapon = Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity); // Adjust the position and rotation as needed
        newWeapon.SetActive(false);

        if (inventory[index] != null)
        {
            Destroy(inventory[index]); // Remove the current weapon
        }

        inventory[index] = newWeapon;
    }
    void HandleHunger()
    {
        if (Time.time >= nextHungerDecayTime)
        {
            hunger -= hungerDecayRate;
            nextHungerDecayTime = Time.time + 60f;

            if (hunger <= 0)
            {
                // Apply effects of hunger
            }
        }
    }

    void HandleThirst()
    {
        if (Time.time >= nextThirstDecayTime)
        {
            thirst -= thirstDecayRate;
            nextThirstDecayTime = Time.time + 60f;

            if (thirst <= 0)
            {
                // Apply effects of thirst
            }
        }
    }

    void HandleInventorySelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory[0] != null)
        {
            ActivateWeapon(0); // Activate melee weapon
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && inventory[1] != null)
        {
            ActivateWeapon(1); // Activate ranged weapon
        }
    }

    void ActivateWeapon(int index)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
                inventory[i].SetActive(i == index);
        }
    }

    void HandleAttacks()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            if (inventory[0] != null && inventory[0].activeSelf) // Check if melee weapon is active
            {
                MeleeAttack();
            }
            else if (inventory[1] != null && inventory[1].activeSelf) // Check if ranged weapon is active
            {
                RangedAttack();
            }
        }
    }

    void MeleeAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f)) // 2 meters range
        {
            ZombieController zombie = hit.transform.GetComponent<ZombieController>();
            if (zombie != null)
            {
                zombie.ApplyDamage(25); // Example damage amount
            }
        }
    }

    void RangedAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f)) // 100 meters range
        {
            ZombieController zombie = hit.transform.GetComponent<ZombieController>();
            if (zombie != null)
            {
                zombie.ApplyDamage(50); // Example damage amount
            }
        }
    }
}
