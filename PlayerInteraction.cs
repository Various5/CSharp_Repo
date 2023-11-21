using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public CarController carController;
    public Text interactionText;

    private bool isNearCar = false;

    void Update()
    {
        if (isNearCar && Input.GetKeyDown(KeyCode.E))
        {
            carController.ToggleCarState();
        }

        if (interactionText != null)
        {
            interactionText.enabled = isNearCar;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car") && other.gameObject == carController.gameObject)
        {
            isNearCar = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car") && other.gameObject == carController.gameObject)
        {
            isNearCar = false;
        }
    }
}
