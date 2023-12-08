using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public enum Quality { Legendary, Epic, PrettyOK, Normal, Mehh, Trash }
    public Quality weaponQuality;
    public float baseDamage = 10f;
    public float value;
    public float durability;
    public float ergonomics;

    public float CalculateDamage()
    {
        // Example calculation: baseDamage * qualityMultiplier + ergonomics bonus
        float qualityMultiplier = 1f;
        switch (weaponQuality)
        {
            case Quality.Legendary: qualityMultiplier = 2f; break;
            case Quality.Epic: qualityMultiplier = 1.5f; break;
            case Quality.PrettyOK: qualityMultiplier = 1.2f; break;
            case Quality.Normal: qualityMultiplier = 1f; break;
            case Quality.Mehh: qualityMultiplier = 0.8f; break;
            case Quality.Trash: qualityMultiplier = 0.5f; break;
        }
        return baseDamage * qualityMultiplier + ergonomics;
    }
}
