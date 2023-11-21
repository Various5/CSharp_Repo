using UnityEngine;
using Pathfinding; // Import the A* Pathfinding namespace
using UnityEngine.UI;

public class ZombieController : MonoBehaviour
{
    [Header("Visual Effects")]
    public GameObject[] bloodDecalPrefabs; // Blood decals for visual effects upon death
    public GameObject[] bodySplatterParts; // Body parts for splatter effect on death
    public GameObject healthBarPrefab; // Prefab for the health bar UI

    [Header("Detection and Movement")]
    public LayerMask whatIsCar;
    public LayerMask whatIsGround;
    public float health = 100f;
    public float maxHealth = 100f;
    public float detectionRange = 10f; // Detection range for chasing the player
    public float moveSpeed = 2f; // Movement speed while wandering or chasing
    public float runSpeed = 4f; // Speed when chasing the player
    public float attackRange = 2f; // Range for attacking the player
    public float attackDamage = 10f; // Damage inflicted on the player
    public float attackRate = 1f; // Rate of attack

    [Header("Target Settings")]
    public string[] enemyTags; // Array to hold tags of entities that can be attacked

    private Transform target; // The target (usually the player)
    private Rigidbody rb;
    private Animator animator; // Animator for controlling zombie animations
    private IAstarAI astarAI; // A* Pathfinding AI interface

    private GameObject healthBar;
    private Image healthFillImage; // Image component of the health bar

    private float timeToChangeDirection;
    private float timeSinceLastSawPlayer = Mathf.Infinity; // Timer for losing interest in the player
    private float loseInterestTime = 10f; // Time after which zombie loses interest in chasing
    private float lastAttackTime;
    private Vector3 wanderTarget = Vector3.zero; // Current wander target

    private enum State { Idle, Wandering, Chasing, Attacking }
    private State currentState = State.Wandering;

    private bool isDying = false;

    public float minDistanceToPlayer = 1.5f; // Minimum distance to keep from the player

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        astarAI = GetComponent<IAstarAI>(); // Initialize A* Pathfinding AI
        timeToChangeDirection = Random.Range(1, 5);

        InitializeHealthBar();
    }

    void Update()
    {
        if (isDying)
        {
            return; // Skip update loop if the zombie is dying
        }

        // FindClosestTarget(); // This line can be removed if AIDestinationSetter is used

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Wandering:
                HandleWanderingState();
                break;
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                AttackPlayer();
                break;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void InitializeHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBar = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity, transform);
            Transform healthFillTransform = healthBar.transform.Find("HealthBarBackground/HealthFill");

            if (healthFillTransform != null)
            {
                healthFillImage = healthFillTransform.GetComponent<Image>();
                if (healthFillImage != null)
                {
                    healthFillImage.fillAmount = health / maxHealth;
                }
                else
                {
                    Debug.LogError("HealthFill Image component not found");
                }
            }
            else
            {
                Debug.LogError("HealthFill GameObject not found");
            }

            Billboard billboard = healthBar.AddComponent<Billboard>();
            if (Camera.main != null)
            {
                billboard.cam = Camera.main;
            }
            else
            {
                Debug.LogError("Main Camera not found");
            }
        }
        else
        {
            Debug.LogError("HealthBar prefab is not assigned");
        }
    }

    private void HandleIdleState()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
    }

    private void HandleWanderingState()
    {
        animator.SetBool("IsWalking", true);

        if (target != null && Vector3.Distance(transform.position, target.position) <= detectionRange)
        {
            currentState = State.Chasing;
        }
        else
        {
            Wandering();
        }
    }

    private void Wandering()
    {
        if (timeToChangeDirection <= 0)
        {
            wanderTarget = GetNewDirection();
            timeToChangeDirection = Random.Range(1, 5);
        }
        else
        {
            timeToChangeDirection -= Time.deltaTime;
            MoveTowardsTarget(transform.position + wanderTarget, moveSpeed);
        }
    }

    private void ChasePlayer()
    {
        if (target != null && IsEnemy(target.gameObject) && Vector3.Distance(transform.position, target.position) <= detectionRange)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= detectionRange)
            {
                timeSinceLastSawPlayer = 0;
                animator.SetBool("IsRunning", true);

                if (distanceToPlayer > minDistanceToPlayer)
                {
                    MoveTowardsTarget(target.position, runSpeed);
                }
                else if (astarAI != null)
                {
                    astarAI.destination = transform.position; // Stop the AI
                }

                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                }
            }
            else
            {
                timeSinceLastSawPlayer += Time.deltaTime;
                if (timeSinceLastSawPlayer > loseInterestTime)
                {
                    animator.SetBool("IsRunning", false);
                    currentState = State.Wandering;
                }
            }
        }
    }

    private bool IsPlayerInLineOfSight()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (target.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            return hit.transform == target;
        }

        return false;
    }

    private void AttackPlayer()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange && IsPlayerInLineOfSight())
        {
            Debug.Log("Attempting to attack player");
            animator.SetTrigger("Attack");

            if (Time.time > lastAttackTime + 1f / attackRate)
            {
                ThirdPersonController player = target.GetComponent<ThirdPersonController>();
                if (player != null)
                {
                    Debug.Log($"Attacking player for {attackDamage} damage");
                    player.ApplyDamage(attackDamage);
                }
                lastAttackTime = Time.time;
            }
        }
        else
        {
            currentState = State.Chasing;
        }
    }


    void MoveTowardsTarget(Vector3 targetPosition, float speed)
    {
        if (targetPosition != null && astarAI != null)
        {
            astarAI.destination = targetPosition; // Set the destination for A* AI
            astarAI.maxSpeed = speed; // Set the speed
        }
    }

    private bool IsEnemy(GameObject obj)
    {
        foreach (var tag in enemyTags)
        {
            if (obj.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetNewDirection()
    {
        float angle = Random.Range(0, 360);
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    public void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = health / maxHealth;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isDying)
        {
            isDying = true;

            // Disable the AIPath, RichAI, or AILerp component instead of IAstarAI
            var aiComponent = GetComponent<AIPath>(); // or GetComponent<RichAI>() or GetComponent<AILerp>()
            if (aiComponent != null)
            {
                aiComponent.enabled = false;
            }

            rb.isKinematic = true;
            currentState = State.Idle;

            animator.SetTrigger("Die");

            SpawnBloodDecals(transform.position);
            SpawnBodyParts(transform.position);

            if (healthBar != null)
            {
                Destroy(healthBar);
            }

            Destroy(gameObject, 3f);
        }
    }


    private void SpawnBloodDecals(Vector3 position)
    {
        foreach (var decalPrefab in bloodDecalPrefabs)
        {
            Instantiate(decalPrefab, position, Quaternion.identity);
        }
    }

    private void SpawnBodyParts(Vector3 position)
    {
        foreach (var partPrefab in bodySplatterParts)
        {
            Instantiate(partPrefab, position, Quaternion.identity);
        }
    }
}
