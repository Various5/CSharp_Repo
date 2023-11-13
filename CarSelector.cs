using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public GameObject[] carPrefabs; // Array of car prefabs to select from
    public Button nextButton; // Button to select the next car
    public Button previousButton; // Button to select the previous car
    public Button confirmButton; // Button to confirm car selection and start the game
    public Transform carSpawnPoint; // The point in the scene where the car will be spawned

    private int currentCarIndex = 0; // Index of the currently selected car
    private GameObject currentCarInstance; // Reference to the current instantiated car instance

    void Start()
    {
        // Add listeners for the next, previous, and confirm buttons
        nextButton.onClick.AddListener(SelectNextCar);
        previousButton.onClick.AddListener(SelectPreviousCar);
        confirmButton.onClick.AddListener(ConfirmSelectionAndStartGame);

        // Instantiate the first car
        currentCarInstance = Instantiate(carPrefabs[currentCarIndex], carSpawnPoint.position, carSpawnPoint.rotation);
    }

    public void SelectNextCar()
    {
        ChangeCarSelection((currentCarIndex + 1) % carPrefabs.Length);
    }

    public void SelectPreviousCar()
    {
        ChangeCarSelection((currentCarIndex - 1 + carPrefabs.Length) % carPrefabs.Length);
    }

    private void ChangeCarSelection(int newIndex)
    {
        // Destroy the current car instance
        if (currentCarInstance != null)
        {
            Destroy(currentCarInstance);
        }

        // Update the index and instantiate the new car
        currentCarIndex = newIndex;
        currentCarInstance = Instantiate(carPrefabs[currentCarIndex], carSpawnPoint.position, carSpawnPoint.rotation);
    }

    private void ConfirmSelectionAndStartGame()
    {
        // Here you could save the selected car information for use in the game
        // For example, you might save a reference to the selected car prefab
        PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        PlayerPrefs.Save(); // Don't forget to save PlayerPrefs

        // Then load the game scene
        SceneManager.LoadScene("GameScene"); // Replace "GameScene" with the actual name of your game scene
    }

    // ... Any additional methods or logic can be added here ...
}
