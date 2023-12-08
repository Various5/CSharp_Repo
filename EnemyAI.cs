using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public float attackDistance = 5f; // Distance within which to attack the player
    public float chaseDistance = 15f; // Distance within which to start chasing the player
    public float attackRate = 1f; // How often the enemy can attack (seconds)
    public float health = 100f; // Enemy health
    public float wanderRadius = 10f; // Radius for wandering behavior

    private IAstarAI ai;
    private Transform player; // Reference to the player
    private float lastAttackTime = 0;
    private Vector3 wanderTarget;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        if (ai == null)
        {
            Debug.LogError("IAstarAI component not found on " + gameObject.name);
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < attackDistance)
        {
            ai.isStopped = true; // Stop moving when close enough to attack
            if (Time.time - lastAttackTime >= attackRate)
            {
                lastAttackTime = Time.time;
                AttackPlayer(); // Attack the player
            }
        }
        else if (distanceToPlayer < chaseDistance)
        {
            ai.isStopped = false;
            ai.destination = player.position; // Chase the player
        }
        else
        {
            Wander(); // Wander around
        }
    }

    void Wander()
    {
        if (ai == null || ai.pathPending || !ai.reachedEndOfPath)
        {
            return;
        }

        // Calculate a new wander target
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        wanderTarget = new Vector3(randomDirection.x, transform.position.y, randomDirection.z);

        Debug.Log("Wander target: " + wanderTarget); // For debugging

        ai.destination = wanderTarget;
        ai.SearchPath();
    }

    void AttackPlayer()
    {
        // Implement attack logic, e.g., reducing player's health
        // Assuming the player has a method TakeDamage(float damage)
        player.GetComponent<PlayerController>().TakeDamage(CalculateDamage());
    }

    float CalculateDamage()
    {
        // Implement damage calculation based on enemy's state
        // Example: return Random.Range(5f, 15f);
        return Random.Range(5f, 15f); // Random damage for demonstration
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy if health drops to 0
        }
    }
}
