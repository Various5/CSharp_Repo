using UnityEngine;
using System.Collections;
public class DecalRemover : MonoBehaviour
{
    // Duration before the decal is removed
    private float removalDelay = 30.0f;

    void Start()
    {
        // Start the coroutine to remove the decal
        StartCoroutine(RemoveAfterDelay());
    }

    private IEnumerator RemoveAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(removalDelay);

        // Remove the decal
        Destroy(gameObject);
    }
}
