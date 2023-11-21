using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Stamina Parameters")]
    public float staminaDecayRate = 10f;
    public float staminaRecoveryRate = 5f;

    [Header("UI Elements")]
    public Image healthFillImage;
    public Image staminaFillImage;
    public Image hungerFillImage;
    public Image thirstFillImage;

    public Text healthText;
    public Text staminaText;
    public Text hungerText;
    public Text thirstText;

    [Header("First Person Camera Settings")]
    public bool isFirstPerson = false;
    public Transform firstPersonCameraPosition;

    [Header("Camera Orbit Settings")]
    public float cameraDistance = 5.0f;
    public Vector2 cameraRotationSpeed = new Vector2(120.0f, 120.0f);
    public Transform cameraLookTarget;

    [Header("Animation Settings")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Weapon Handling")]
    public Transform rightHandTransform;
    public GameObject bulletPrefab;
    public float bulletSpeed = 100f;
    public float bulletDamage = 25f;
    public Transform bulletSpawnPoint;

    [Header("Inventory")]
    public List<Item> inventory = new List<Item>();
    public int inventorySize = 10;
    public GameObject inventoryPanel;
    public GameObject WeaponInventoryPanel;
    private bool isInventoryOpen = false;
    public InventoryUI inventoryUI;

    [Header("Death Settings")]
    public Image deathScreenOverlay; // Assign a UI Image with red color
    public bool isDead = false;

    [Header("Respawn Settings")]
    public Transform spawnPoint; // Assign this in the Unity editor
    public GameObject respawnButton; // Assign a UI button in the Unity editor
    
    [Header("Respawn Settings")]
    public GameObject respawnPointPrefab; // Assign this in the Unity editor
    private GameObject currentRespawnPoint;

    public WeaponType currentWeaponType = WeaponType.None;
   

    private CharacterController characterController;
    private Animator animator;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private Vector3 cameraOffset;
    private float cameraYaw = 0f;
    private float cameraPitch = 0f;
    private Vector2 pitchMinMax = new Vector2(-40, 85);
    private float nextStaminaRecoveryTime = 0f;
    private float nextHungerDecayTime = 0f;
    private float nextThirstDecayTime = 0f;
    public float crouchHeight = 1.0f;
    private bool isAiming = false;
    public Transform carStopPoint;
    public enum WeaponType
    {
        None,
        Pistol,
        Shotgun,
        Rifle,
        Sword,
        Axe
    }
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraOffset = playerCamera.transform.position - cameraFollowTarget.position;
        Cursor.lockState = CursorLockMode.Locked;
        UpdateUI();
    }
    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleCamera();
        UpdateStatus();
        UpdateUI();
        HandleShooting();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();
        if (Input.GetKeyDown(KeyCode.V)) ToggleCameraView();
        if (Input.GetKeyDown(KeyCode.C)) ToggleCrouch();

        // Weapon equip inputs
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeaponFromInventory(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeaponFromInventory(1);

        // Aiming input
        if (Input.GetMouseButton(1)) // Right-click hold for aiming
        {
            isAiming = true;
            animator.SetBool($"{currentWeaponType}Aim", isAiming);
        }
        else
        {
            isAiming = false;
            animator.SetBool($"{currentWeaponType}Aim", isAiming);
        }
    }


    private bool IsWeaponEquipped()
    {
        // Implement logic to check if a weapon is equipped
        // Example: return inventory.Exists(item => item is Weapon);
        // For now, returning true for testing purposes
        return true;
    }
    private void ToggleCrouch()
    {
        // Implement crouching logic here
        // For example, modify the character's height or position
    }
    private void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        WeaponInventoryPanel.SetActive(!isActive);
        if (isInventoryOpen) PauseGame();
        else ResumeGame();
    }

    private void UpdateUI()
    {
        UpdateBar(health, 100f, healthFillImage, healthText);
        UpdateBar(stamina, 100f, staminaFillImage, staminaText);
        UpdateBar(hunger, 100f, hungerFillImage, hungerText);
        UpdateBar(thirst, 100f, thirstFillImage, thirstText);
    }

    private void UpdateBar(float currentValue, float maxValue, Image fillImage, Text valueText)
    {
        fillImage.fillAmount = currentValue / maxValue;
        valueText.text = $"{currentValue:0}/{maxValue:0}";
    }

    public void PickupItem(Item item)
    {
        if (inventory.Count >= inventorySize) return;

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
        if (inventoryUI != null)
        {
            inventoryUI.UpdateInventory(inventory);
        }
        else
        {
            Debug.LogError("InventoryUI is not set in ThirdPersonController.");
        }
        inventoryUI.UpdateInventory(inventory);
        if (inventory != null)
        {
            inventoryUI.UpdateInventory(inventory);
        }
        else
        {
            Debug.LogError("Inventory list is null in ThirdPersonController.");
        }
    }

    public void SwapItems(int index1, int index2)
    {
        if (index1 >= 0 && index1 < inventory.Count && index2 >= 0 && index2 < inventory.Count)
        {
            Item temp = inventory[index1];
            inventory[index1] = inventory[index2];
            inventory[index2] = temp;
            inventoryUI.UpdateInventory(inventory);
        }
    }

    public void RemoveItemFromInventory(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            inventory.RemoveAt(index);
            inventoryUI.UpdateInventory(inventory);
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void EquipWeaponFromInventory(int weaponSlotIndex)
    {
        // Simplified example, adjust according to your inventory logic
        if (inventory.Count > weaponSlotIndex)
        {
            Item item = inventory[weaponSlotIndex];
            switch (item.itemType)
            {
                case ItemType.Weapon:
                    currentWeaponType = WeaponType.Pistol; // Example, change based on item
                    break;
                    // Handle other item types if necessary
            }
        }
    }

    void HandleStamina()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && isGrounded && (characterController.velocity.magnitude > 0);
        if (isSprinting)
        {
            stamina -= staminaDecayRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 100);
            nextStaminaRecoveryTime = Time.time + 1f;
        }
        else if (Time.time >= nextStaminaRecoveryTime)
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 100);
        }
    }

    void HandleHunger()
    {
        if (Time.time >= nextHungerDecayTime)
        {
            hunger -= hungerDecayRate;
            nextHungerDecayTime = Time.time + 60f;
            // Add effects of hunger if needed
        }
    }

    void HandleThirst()
    {
        if (Time.time >= nextThirstDecayTime)
        {
            thirst -= thirstDecayRate;
            nextThirstDecayTime = Time.time + 60f;
            // Add effects of thirst if needed
        }
    }
    private void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0) playerVelocity.y = 0f;

        Vector3 move = GetInputMoveDirection();
        Vector3 moveDirection;
        if (isFirstPerson)
            moveDirection = GetFPSMove(move); // First-person movement logic
        else
            moveDirection = GetTPSMove(move); // Third-person movement logic

        if (Input.GetButtonDown("Jump") && isGrounded && stamina > 0) Jump();

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        moveDirection.y = playerVelocity.y;

        // Apply movement, reduce speed if stamina is 0
        float currentSpeed = stamina > 0 ? movementSpeed : movementSpeed * 0.5f; // Reduced speed
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        UpdateAnimations(move);
    }
    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        deathScreenOverlay.gameObject.SetActive(true);
        respawnButton.SetActive(true); // Show respawn button
        // Disable player input or other gameplay elements as needed

        // Drop a respawn point
        if (currentRespawnPoint != null)
        {
            Destroy(currentRespawnPoint); // Remove previous respawn point if it exists
        }
        currentRespawnPoint = Instantiate(respawnPointPrefab, transform.position, Quaternion.identity);
        PauseGame();
    }

    // Call this method when the respawn button is clicked
    public void Respawn()
    {
        ResumeGame();

        isDead = false;
        transform.position = spawnPoint.position; // Reset player position
        transform.rotation = spawnPoint.rotation; // Reset player rotation

        if (currentRespawnPoint != null)
        {
            transform.position = currentRespawnPoint.transform.position;
            transform.rotation = currentRespawnPoint.transform.rotation;
        }
        else
        {
            // Fallback to a default spawn point or initial position
        }
        // Reset player status
        health = 100f;
        stamina = 100f;
        hunger = 100f;
        thirst = 100f;

        // Reset UI
        deathScreenOverlay.gameObject.SetActive(false);
        respawnButton.SetActive(false);
        UpdateUI();

        // Enable player input or other gameplay elements as needed
        animator.ResetTrigger("Die");
        animator.SetTrigger("Respawn"); // If you have a respawn animation
    }

    private Vector3 GetFPSMove(Vector3 move)
    {
        // First-person movement logic
        move = transform.TransformDirection(move);
        move *= movementSpeed;
        return move;
    }
    private Vector3 GetInputMoveDirection()
    {
        // This method should return a Vector3 based on WASD input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return new Vector3(horizontal, 0, vertical);
    }
    private Vector3 GetTPSMove(Vector3 move)
    {
        move = transform.TransformDirection(move);
        move *= movementSpeed;

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        return move;
    }

    private void Jump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
    }
    private void HandleCamera()
    {
        if (isFirstPerson)
        {
            HandleFirstPersonCamera();
        }
        else
        {
            HandleThirdPersonCamera();
        }
    }

    private void HandleFirstPersonCamera()
    {
        // First-person camera logic
        cameraYaw += Input.GetAxis("Mouse X") * lookSensitivity;
        cameraPitch -= Input.GetAxis("Mouse Y") * lookSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, pitchMinMax.x, pitchMinMax.y);

        // Update the camera's rotation
        playerCamera.transform.eulerAngles = new Vector3(cameraPitch, cameraYaw, 0f);

        // Align the player's rotation with the camera's yaw
        transform.eulerAngles = new Vector3(0f, cameraYaw, 0f);
    }

    private void HandleThirdPersonCamera()
    {
        // Refine third-person camera logic for smooth orbiting
        cameraYaw += Input.GetAxis("Mouse X") * lookSensitivity;
        cameraPitch -= Input.GetAxis("Mouse Y") * lookSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, pitchMinMax.x, pitchMinMax.y);

        // Adjust the camera position and rotation for orbiting
        Quaternion cameraRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        playerCamera.transform.position = cameraLookTarget.position - (cameraRotation * Vector3.forward * cameraDistance);
        playerCamera.transform.LookAt(cameraLookTarget.position);
    }

    public void SetPlayerActive(bool isActive)
    {
        // Enable or disable player control and reset position if exiting a car
        this.enabled = isActive;
        characterController.enabled = isActive;

        if (!isActive)
        {
            // When player enters a car, disable character controller, etc.
            // Additional logic like hiding the player model can be added here
        }
        else
        {
            // When player exits a car, reset to car stop point
            transform.position = carStopPoint.position;
            transform.rotation = carStopPoint.rotation;
        }
    }
    private void ToggleCameraView()
    {
        isFirstPerson = !isFirstPerson;
        if (isFirstPerson)
        {
            // First-person camera settings
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraOffset = firstPersonCameraPosition.localPosition;
            playerCamera.transform.position = firstPersonCameraPosition.position;
            playerCamera.transform.rotation = firstPersonCameraPosition.rotation;
        }
        else
        {
            // Third-person camera settings
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraOffset = cameraFollowTarget.position - playerCamera.transform.position;
        }
    }

    private void HandleShooting()
    {
        if (isAiming && Input.GetMouseButtonDown(0)) // Left mouse button for shooting
        {
            ShootBullet();
            // Trigger shooting animation
            animator.SetTrigger($"{currentWeaponType}Attack");
        }
    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;

        RaycastHit hit;
        if (Physics.Raycast(bulletSpawnPoint.position, bulletSpawnPoint.forward, out hit))
        {
            ZombieController zombie = hit.collider.GetComponent<ZombieController>();
            if (zombie != null)
            {
                zombie.ApplyDamage(bulletDamage);
            }
        }
    }

    void HandleAiming()
    {
        isAiming = Input.GetMouseButton(1); // Right-click hold
        animator.SetBool($"{currentWeaponType}Aim", isAiming);
    }

    private void UpdateStatus()
    {
        // Handle stamina, hunger, and thirst
        HandleStamina();
        HandleHunger();
        HandleThirst();
        
        if (hunger <= 0 || thirst <= 0)
        {
            health -= 0.5f * Time.deltaTime; // Adjust the rate as needed
        }

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }
    private void UpdateAnimations(Vector3 move)
    {
        bool isMoving = move.magnitude > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        ResetAnimationParameters();

        switch (currentWeaponType)
        {
            case WeaponType.Pistol:
            case WeaponType.Shotgun:
            case WeaponType.Rifle:
                SetWeaponAnimationParameters(isMoving, isRunning, currentWeaponType.ToString());
                break;
            case WeaponType.Sword:
            case WeaponType.Axe:
                if (isAiming) // Replace with your melee attack logic
                {
                    animator.SetBool($"{currentWeaponType}Attack", true);
                }
                else
                {
                    animator.SetBool("isRunning", isRunning);
                    animator.SetBool("isWalking", isMoving && !isRunning);
                }
                break;
            default:
                animator.SetBool("isIdle", !isMoving);
                break;
        }
    }
    private void SetWeaponAnimationParameters(bool isMoving, bool isRunning, string weaponType)
    {
        if (isAiming)
        {
            animator.SetBool($"{weaponType}Aim", true);
        }
        else if (isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if (isMoving)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isIdle", true);
        }
    }

    private void ResetAnimationParameters()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            if (type == WeaponType.None) continue;
            animator.SetBool($"{type}Aim", false);
            animator.SetBool($"{type}Attack", false);
        }
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, 100f);
    }
}
