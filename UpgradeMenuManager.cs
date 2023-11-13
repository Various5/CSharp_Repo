using UnityEngine;

public class UpgradeMenuManager : MonoBehaviour
{
    public static UpgradeMenuManager Instance { get; private set; }

    [SerializeField] private GameObject upgradeMenuUI; // Assign in inspector
    private CarController carController; // Reference to the car controller

    void Awake()
    {
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

    void Update()
    {
        // Listen for the ESC key to exit the garage
        if (upgradeMenuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGarage();
        }
    }

    public void OpenMenu(CarController controller)
    {
        upgradeMenuUI.SetActive(true);
        Time.timeScale = 0; // Pause the game

        carController = controller; // Store the reference to the car controller
        if (carController != null)
        {
            carController.enabled = false; // Disable car controls
        }
    }

    public void CloseMenu()
    {
        upgradeMenuUI.SetActive(false);
        Time.timeScale = 1; // Resume the game

        if (carController != null)
        {
            carController.enabled = true; // Re-enable car controls
        }
    }

    // This method is used to exit the garage
    public void ExitGarage()
    {
        CloseMenu();
        carController = null; // Clear the reference
    }

    // Call this from button OnClick events
    public void UpgradeTurret()
    {
        // The upgrade logic goes here
    }
}
