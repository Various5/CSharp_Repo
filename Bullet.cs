using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("The damage the bullet does")]
    public float damage = 10f;

    [Tooltip("How long the bullet lives before being destroyed")]
    public float lifeTime = 5f;

    [Tooltip("Speed of the bullet")]
    public float bulletSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        // Give the bullet an initial velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * bulletSpeed;
        }
        else
        {
            Debug.LogError("Bullet is missing a Rigidbody component", this);
        }

        // Destroy the bullet after 'lifeTime' seconds
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Attempt to get the ZombieController component on the object we collided with
        ZombieController zombie = collision.collider.GetComponent<ZombieController>();

        // If we found a ZombieController, call its ApplyDamage method
        if (zombie != null)
        {
            zombie.ApplyDamage(damage);
        }

        // Destroy the bullet on impact
        Destroy(gameObject);
    }
}
