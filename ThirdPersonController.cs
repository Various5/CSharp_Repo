using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
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
    public Transform cameraFollowTarget;

    [Header("Camera Orbit Settings")]
    public float cameraDistance = 5.0f;
    public Vector2 cameraRotationSpeed = new Vector2(120.0f, 120.0f); // Speed of rotation
    public Transform cameraLookTarget; // Assign a target on the player for the camera to look at

    [Header("Animation Settings")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Weapon Handling")]
    public Transform rightHandTransform; // Assign this in the inspector to the character's right hand bone

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private float nextHungerDecayTime = 0f;
    private float nextThirstDecayTime = 0f;

    private Animator animator;
    private Vector3 cameraOffset;
    private Vector2 pitchMinMax = new Vector2(-40, 85); // Limits for the camera's up/down rotation

    private float cameraYaw = 0f;
    private float cameraPitch = 0f;

    [Header("Inventory")]
    public List<Item> inventory = new List<Item>();
    public int inventorySize = 10;

    public GameObject inventoryPanel; // Assign this in the Unity Inspector
    private bool isInventoryOpen = false;
    public InventoryUI inventoryUI;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        // Calculate initial camera offset from player at start
        cameraOffset = playerCamera.transform.position - cameraLookTarget.position;
    }

    public void PickupItem(Item item)
    {
        // Check if inventory is full
        if (inventory.Count >= inventorySize) return;

        // Add item to inventory
        if (item.isStackable)
        {
            var existingItem = inventory.Find(i => i.itemID == item.itemID);
            if (existingItem != null)
            {
                existingItem.stackSize += item.stackSize;
            }
            else
            {
                inventory.Add(item);
            }
        }
        else
        {
            inventory.Add(item);
        }

        inventoryUI.UpdateInventory(inventory);
    }


    private void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleHunger();
        HandleThirst();

        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);

            if (isInventoryOpen)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0; // Pauses the game
        Cursor.lockState = CursorLockMode.None; // Frees the cursor
        Cursor.visible = true; // Makes the cursor visible
    }

    void ResumeGame()
    {
        Time.timeScale = 1; // Resumes the game
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center
        Cursor.visible = false; // Hides the cursor
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

        Vector3 moveDirection = new Vector3(x, 0, z).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Rotate player only when moving
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            animator.SetTrigger("jump");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        UpdateAnimations(x, z);
    }


    void UpdateAnimations(float x, float z)
    {
        bool isMoving = x != 0 || z != 0;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        // Debugging to check if the values are being set correctly
       // Debug.Log($"isMoving: {isMoving}, isRunning: {isRunning}");

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isWalking", isMoving && !isRunning);
        animator.SetBool("isIdle", !isMoving);
    }

    void HandleCamera()
    {
        // Camera rotation independent of player movement
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        cameraYaw += mouseX;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, pitchMinMax.x, pitchMinMax.y);

        // Rotate the camera independently
        playerCamera.transform.eulerAngles = new Vector3(cameraPitch, cameraYaw, 0f);
        playerCamera.transform.position = cameraFollowTarget.position - playerCamera.transform.forward * cameraDistance;
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Handle player death here
        }
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
    public void EquipWeapon(Item weapon)
    {
        // Instantiate and position the weapon prefab in the player's hand
        // You may need to deactivate the current weapon if there is one
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


}