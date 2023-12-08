using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ArtilleryAiming : MonoBehaviour
{
    // Existing public variables
    public GameObject artilleryShellPrefab;
    public Transform firePoint;
    public GameObject muzzleFlashPrefab;
    public float reloadTime = 5f;
    public int maxAmmo = 10;
    public TMP_Text ammoText;
    public TMP_Text healthText;
    public Image reloadBar;
    public int currentHealth = 100;
    public float maxYawAngle = 90f;
    public float maxPitchAngle = 65f;
    public float minYawAngle = -90f;
    public float minPitchAngle = -5f;
    public float rotationSmoothness = 0.1f;
    public Transform rotationOffsetTransform;

    private float lastShotTime;
    private bool isReloading = false;
    private int currentAmmo;
    private Quaternion targetBarrelRotation;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateUI();
        targetBarrelRotation = rotationOffsetTransform.localRotation;
    }

    void Update()
    {
        Aim();

        if (!isReloading)
        {
            HandleInput();
        }

        ApplyRotation();
    }

    void Aim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetDirection = hit.point - rotationOffsetTransform.position;
            targetDirection.y = 0;
            Quaternion yRotation = Quaternion.LookRotation(targetDirection);
            float yAngle = Mathf.Clamp(AngleDifference(yRotation.eulerAngles.y, rotationOffsetTransform.eulerAngles.y), minYawAngle, maxYawAngle);

            float zAngle = Mathf.Clamp(AngleToTarget(hit.point, rotationOffsetTransform), minPitchAngle, maxPitchAngle);

            targetBarrelRotation = Quaternion.Euler(-zAngle, yAngle, 0); // Note the inversion of zAngle for correct up/down aiming
        }
    }

    float AngleToTarget(Vector3 targetPoint, Transform referenceTransform)
    {
        Vector3 direction = targetPoint - referenceTransform.position;
        direction = referenceTransform.InverseTransformDirection(direction);
        return Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastShotTime >= reloadTime && currentAmmo > 0)
        {
            FireArtillery();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    void ApplyRotation()
    {
        rotationOffsetTransform.rotation = Quaternion.Lerp(rotationOffsetTransform.rotation, targetBarrelRotation, rotationSmoothness * Time.deltaTime);
    }

    void FireArtillery()
    {
        GameObject shellInstance = Instantiate(artilleryShellPrefab, firePoint.position, firePoint.rotation);
        ArtilleryShell shellScript = shellInstance.GetComponent<ArtilleryShell>();

        if (shellScript != null)
        {
            TerrainDeformer deformer = FindObjectOfType<TerrainDeformer>();
            if (deformer != null)
            {
                shellScript.SetTerrainDeformer(deformer);
            }
        }
    }

        IEnumerator Reload()
    {
        float reloadStartTime = Time.time;
        while (Time.time - reloadStartTime < reloadTime)
        {
            UpdateReloadBar((Time.time - reloadStartTime) / reloadTime);
            yield return null;
        }
        isReloading = false;
        reloadBar.gameObject.SetActive(false);
        UpdateUI();
    }


    void UpdateReloadBar(float progress)
    {
        if (reloadBar != null)
        {
            reloadBar.fillAmount = progress;
        }
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo;
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateUI();
        if (currentHealth <= 0)
        {
            // Handle artillery destruction
        }
    }

    float AngleDifference(float angle1, float angle2)
    {
        float diff = (angle1 - angle2 + 180) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }

    private void UpdateUI()
    {
        UpdateAmmoText();
        UpdateHealthText();
        UpdateReloadBar(isReloading ? (Time.time - lastShotTime) / reloadTime : 0);
    }
}
