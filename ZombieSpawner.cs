using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Assign this in the inspector with your zombie prefab
    public float spawnTime = 5f; // Time in seconds between each spawn
    public static int zombieCount = 0; // Static counter to keep track of the number of zombies
    public int maxZombieCount = 20; // Maximum number of zombies allowed

    private float timer; // Keep track of the time since the last spawn

    void Start()
    {
        // Start the timer
        timer = spawnTime;
    }

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Check if the timer has reached the spawn time and the number of zombies is less than the max
        if (timer >= spawnTime && zombieCount < maxZombieCount)
        {
            SpawnZombie();
            timer = 0f; // Reset the timer
        }
    }

    void SpawnZombie()
    {
        // Instantiate a new zombie at the spawner's position and rotation
        GameObject newZombie = Instantiate(zombiePrefab, transform.position, transform.rotation);
        zombieCount++; // Increment the zombie count
        // Assign a callback to the zombie's OnDestroy event, if it exists
        Zombie zombieComponent = newZombie.GetComponent<Zombie>();
        if (zombieComponent != null)
        {
            zombieComponent.OnDestroyEvent += () => { zombieCount--; };
        }
    }
}

// Assuming you have a Zombie script attached to your zombie prefab
public class Zombie : MonoBehaviour
{
    // Delegate type for the event
    public delegate void OnDestroyHandler();
    // Event triggered when the zombie is destroyed
    public event OnDestroyHandler OnDestroyEvent;

    void OnDestroy()
    {
        // Trigger the event when the zombie is destroyed
        OnDestroyEvent?.Invoke();
    }
}
