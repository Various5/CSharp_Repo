using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI; // Required for NavMesh Agent

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
    private NavMeshAgent navMeshAgent; // NavMesh Agent for movement

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
        navMeshAgent = GetComponent<NavMeshAgent>(); // Initialize NavMesh Agent
        timeToChangeDirection = Random.Range(1, 5);

        InitializeHealthBar();
    }

    void Update()
    {
        if (isDying)
        {
            return; // Skip update loop if the zombie is dying
        }

        FindClosestTarget(); // Update target each frame

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
            // Instantiate the health bar prefab and parent it to the current object
            healthBar = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity, transform);

            // Correct the path to just "HealthFill" since "HealthBar" is the root object of the prefab
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

            // Assuming you have a Billboard script that makes the health bar face the camera
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
                else if (navMeshAgent != null)
                {
                    navMeshAgent.ResetPath();
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
            animator.SetTrigger("Attack");

            if (Time.time > lastAttackTime + 1f / attackRate)
            {
                FirstPersonController player = target.GetComponent<FirstPersonController>();
                if (player != null)
                {
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

    private void AdjustPathForAvoidance()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f); // 2 meters radius for detection
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject && hitCollider.CompareTag("Zombie")) // Check for other zombies
            {
                Vector3 directionAway = transform.position - hitCollider.transform.position;
                MoveTowardsTarget(transform.position + directionAway.normalized, moveSpeed);
                break;
            }
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition, float speed)
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > minDistanceToPlayer)
        {
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(targetPosition);
                navMeshAgent.speed = speed;
            }
            else
            {
                rb.MovePosition(transform.position + directionToTarget * speed * Time.deltaTime);
            }
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

    private void FindClosestTarget()
    {
        float closestDistance = detectionRange;
        Transform closestTarget = null;

        foreach (string tag in enemyTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject potentialTarget in targets)
            {
                float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = potentialTarget.transform;
                }
            }
        }

        target = closestTarget;
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

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
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
