using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;
    public float attackDistance = 2f; // Distance to trigger attack animation
    public float health = 100f; // Player health for death animation
    public LayerMask enemyLayer;

    public Transform handTransform; // Assign the hand transform to attach the weapon
    private MeleeWeapon equippedWeapon;

    private Rigidbody rb;
    private bool isGrounded;
    private Animator animator;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // WASD movement with Shift for running
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 movementDirection = Camera.main.transform.TransformDirection(inputDirection);
        movementDirection.y = 0; // Keep the movement in the horizontal plane

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (movementDirection.magnitude > 0)
        {
            float speed = isRunning ? runSpeed : walkSpeed;
            Vector3 newPosition = transform.position + movementDirection * speed * Time.deltaTime;
            rb.MovePosition(newPosition);
            transform.forward = movementDirection;

            // Animation control
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", isRunning);
        }
        else
        {
            // Reset walking and running animations
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            animator.SetBool("isJumping", true);
        }

        // Attack
        if (Input.GetKeyDown(KeyCode.Mouse0)) // Left-click for attack
        {
            animator.SetTrigger("Attack");
            // Additional attack logic can be implemented here
        }

        // Check for player health for death animation
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            // Additional death logic can be implemented here
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f) // Ensure collision is mostly upwards
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }

    }
    void EquipWeapon(GameObject weaponObj)
    {
        equippedWeapon = weaponObj.GetComponent<MeleeWeapon>();
        weaponObj.transform.SetParent(handTransform);
        weaponObj.transform.localPosition = Vector3.zero; // Adjust this as needed
        weaponObj.transform.localRotation = Quaternion.identity; // Adjust this as needed
    }

    void Attack()
    {
        if (equippedWeapon != null)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackDistance, enemyLayer); // Define enemyLayer as needed
            foreach (var enemy in hitEnemies)
            {
                float damage = equippedWeapon.CalculateDamage();
                enemy.GetComponent<EnemyAI>().TakeDamage(damage);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MeleeWeapon") && equippedWeapon == null)
        {
            EquipWeapon(other.gameObject);
        }
    }
    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    // Call this method to apply damage to the player
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Trigger death animation and any other death logic
            animator.SetTrigger("Die");
        }
    }
}
