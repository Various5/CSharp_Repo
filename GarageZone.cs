using UnityEngine;

public class GarageZone : MonoBehaviour
{
    public GameObject upgradeMenuUI; // Assign this in the Inspector
    private CarController carController; // To hold a reference to the CarController

    private void Awake()
    {
        if (upgradeMenuUI == null)
        {
            Debug.LogError("UpgradeMenuUI is not assigned in the inspector!");
        }
    }

    private void Update()
    {
        // Check for the ESC key and if the upgrade menu is currently active
        if (Input.GetKeyDown(KeyCode.Escape) && upgradeMenuUI.activeSelf)
        {
            ExitGarage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Garage");

        if (other.CompareTag("Player")) // Make sure your car's tag is "Player"
        {
            carController = other.GetComponent<CarController>();
            if (carController != null)
            {
                carController.enabled = false; // Disable the car controller to stop movement
                carController.StopCar();

                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                else
                {
                    Debug.LogError("Rigidbody component not found on the player object!");
                }

                if (upgradeMenuUI != null)
                {
                    upgradeMenuUI.SetActive(true);
                    Time.timeScale = 0f; // Pause the game
                }
                else
                {
                    Debug.LogError("Upgrade menu UI GameObject is not assigned!");
                }
            }
            else
            {
                Debug.LogError("CarController component not found on the player object!");
            }
        }
        else
        {
            Debug.Log("The object that entered the garage does not have the Player tag.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitGarage();
        }
    }

    private void ExitGarage()
    {
        if (carController != null)
        {
            carController.enabled = true; // Re-enable the car controller to allow movement

            if (upgradeMenuUI != null)
            {
                upgradeMenuUI.SetActive(false);
                Time.timeScale = 1f; // Resume the game
            }
            else
            {
                Debug.LogError("Upgrade menu UI GameObject is not assigned!");
            }
        }
        else
        {
            Debug.LogError("CarController component not found on the player object!");
        }
    }
}
