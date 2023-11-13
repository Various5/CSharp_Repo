using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text killCountText; // Assign this in the inspector with your Text component for kill count
    public Text currencyText; // Assign this in the inspector with your Text component for currency

    private void OnEnable()
    {
        GameManager.onZombieKilled += UpdateKillCount;
        CurrencyManager.onCurrencyChanged += UpdateCurrencyDisplay; // Subscribe to the currency changed event
    }

    private void OnDisable()
    {
        GameManager.onZombieKilled -= UpdateKillCount;
        CurrencyManager.onCurrencyChanged -= UpdateCurrencyDisplay; // Unsubscribe to avoid memory leaks
    }

    private void Start()
    {
        UpdateKillCount(); // Initial update on start
        UpdateCurrencyDisplay(); // Initial update on start
    }

    private void UpdateKillCount()
    {
        killCountText.text = "Zombies Killed: " + GameManager.ZombieKillCount.ToString();
    }

    private void UpdateCurrencyDisplay()
    {
        currencyText.text = "Currency: " + CurrencyManager.Currency.ToString();
    }
}
