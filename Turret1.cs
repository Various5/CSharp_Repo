using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform partToRotate; // The part of the turret that rotates horizontally
    public Transform barrel; // The part of the turret that pitches up and down
    public Transform firePoint; // The point from which bullets are fired
    public GameObject bulletPrefab; // The bullet prefab to shoot
    public float range = 15f; // Range within which the turret will engage targets
    public float turnSpeed = 10f; // Speed at which the turret turns
    public float fireRate = 1f; // Number of bullets fired per second
    public float bulletSpeed = 20f; // Speed of the bullet

    public int upgradeCostRange = 50;
    public int upgradeCostTurnSpeed = 50;
    public int upgradeCostFireRate = 50;
    public int upgradeCostBulletSpeed = 50;

    public float upgradeIncrementRange = 5f;
    public float upgradeIncrementTurnSpeed = 2f;
    public float upgradeIncrementFireRate = 0.2f;
    public float upgradeIncrementBulletSpeed = 5f;

    private float fireCountdown = 0f;
    private Transform target;

    void Update()
    {
        UpdateTarget();
        if (target != null)
        {
            LockOnTarget();
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (barrel != null)
        {
            // Handle the pitch of the barrel
            float pitchAngle = Quaternion.LookRotation(dir).eulerAngles.x;
            barrel.rotation = Quaternion.Euler(pitchAngle, rotation.y, 0f);
        }
    }

    void Shoot()
    {
        GameObject bulletGo = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bulletGo.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            bulletRb.velocity = firePoint.forward * bulletSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void UpgradeRange()
    {
        if (CurrencyManager.SpendCurrency(upgradeCostRange))
        {
            range += upgradeIncrementRange;
        }
    }

    public void UpgradeTurnSpeed()
    {
        if (CurrencyManager.SpendCurrency(upgradeCostTurnSpeed))
        {
            turnSpeed += upgradeIncrementTurnSpeed;
        }
    }

    public void UpgradeFireRate()
    {
        if (CurrencyManager.SpendCurrency(upgradeCostFireRate))
        {
            fireRate += upgradeIncrementFireRate;
        }
    }

    public void UpgradeBulletSpeed()
    {
        if (CurrencyManager.SpendCurrency(upgradeCostBulletSpeed))
        {
            bulletSpeed += upgradeIncrementBulletSpeed;
        }
    }
}
