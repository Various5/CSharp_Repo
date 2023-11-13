using System;

public static class CurrencyManager
{
    public static event Action onCurrencyChanged; // Event to signal currency changes
    public static int Currency { get; private set; }

    // Call this method to add currency and trigger the update event
    public static void AddCurrency(int amount)
    {
        Currency += amount;
        onCurrencyChanged?.Invoke(); // Notify any listeners that the currency has changed
        // Here you can add additional code to update the UI and save the currency value if necessary
    }

    // Call this method when the player wants to spend currency. It returns true if the purchase is successful.
    public static bool SpendCurrency(int amount)
    {
        if (Currency >= amount)
        {
            Currency -= amount;
            onCurrencyChanged?.Invoke(); // Notify any listeners that the currency has changed
            // Here you can add additional code to update the UI and save the currency value if necessary
            return true;
        }
        return false; // Return false if there's not enough currency to spend
    }

    // Use this method to set the currency to a specific value, if needed.
    public static void SetCurrency(int newCurrency)
    {
        Currency = newCurrency;
        onCurrencyChanged?.Invoke(); // Notify any listeners that the currency has changed
        // Here you can add additional code to update the UI and save the currency value if necessary
    }
}
