using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Define statuses
    public int health;
    public int karma;
    public int influence;

    // Singleton instance
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure there is only one GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to handle progression
    public void CompleteLevel()
    {
        // Implement level completion logic
    }

    // Method to handle game over
    public void GameOver()
    {
        // Implement game over logic
    }

    // Update statuses
    public void UpdateStatuses(int healthChange, int karmaChange, int influenceChange)
    {
        health += healthChange;
        karma += karmaChange;
        influence += influenceChange;
    }
}
