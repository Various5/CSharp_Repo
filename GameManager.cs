using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnZombieKilled();
    public static event OnZombieKilled onZombieKilled;

    public static int ZombieKillCount { get; private set; }

    public static void IncrementKillCount()
    {
        ZombieKillCount++;
        Debug.Log("Zombie killed. Total kills: " + ZombieKillCount);

        // Call the event
        onZombieKilled?.Invoke();
    }

    // Optionally, you can have a method to reset the kill count if needed
    public static void ResetKillCount()
    {
        ZombieKillCount = 0;
    }
}
