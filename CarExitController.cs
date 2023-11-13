using UnityEngine;

public class CarExitController : MonoBehaviour
{
    public CarController carController; // Reference to the CarController script

    void Update()
    {
        // Check for the 'E' key press to exit the car
        if (Input.GetKeyDown(KeyCode.E) && carController.isPlayerInCar)
        {
            Debug.Log("Exiting Car");
            carController.ExitCar();
        }
    }
}
