using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class PlayerInteraction : MonoBehaviour
{
    public CarController carController;
    public Text interactionText; // Reference to the UI Text component

    private bool isNearCar = false;

    void Update()
    {
        if (isNearCar && Input.GetKeyDown(KeyCode.E))
        {
            if (carController.isPlayerInCar)
            {
                Debug.Log("Exiting Car");
                carController.ExitCar();
            }
            else
            {
                Debug.Log("Entering Car");
                carController.EnterCar();
            }
        }

        // Update the interaction text based on the player's proximity to the car
        if (interactionText != null)
        {
            interactionText.enabled = isNearCar;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car") && other.gameObject == carController.gameObject)
        {
            Debug.Log("Player is near the Car");
            isNearCar = true;
            // Optionally, update the text here if you want different messages for different vehicles or states
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car") && other.gameObject == carController.gameObject)
        {
            Debug.Log("Player is no longer near the Car");
            isNearCar = false;
        }
    }
}
