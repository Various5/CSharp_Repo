using UnityEngine;
using System.Collections;
public class Plant : MonoBehaviour
{
    public float health = 50f; // Represents the amount of food the plant can provide
    public float replenishTime = 10f; // Time in seconds for the plant to replenish

    public void Consume(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            StartCoroutine(Replenish());
        }
    }

    IEnumerator Replenish()
    {
        // Make the plant 'disappear' or change its state to 'consumed'
        gameObject.SetActive(false);

        // Wait for replenishTime seconds
        yield return new WaitForSeconds(replenishTime);

        // Replenish the plant and make it active again
        health = 50f; // Reset the health to its initial value
        gameObject.SetActive(true);
    }
}
