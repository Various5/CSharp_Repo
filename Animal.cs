using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using TMPro;

public enum Gender { Male, Female }
public enum DietType { Herbivore, Carnivore }

public class Animal : MonoBehaviour
{
    public Gender gender;
    public DietType dietType;
    public float health = 100f;
    public float hunger = 0f;
    public float age = 0f;
    public Animal packLeader;
    public List<Animal> packMembers;
    public float roamRadius = 10f;
    public float hungerThreshold = 5f;
    public float reproductionCooldown = 30f;
    public float attackDamage = 10f;
    public float eatingTime = 5f;
    public float roamSpeed;
    public float pregnancyTime = 10f;
    public GameObject babyAnimalPrefab;
    public float healthDecayRate = 5f;
    public static int MaleCount { get; private set; }
    public static int FemaleCount { get; private set; }
    public static event System.Action<string> OnAnimalBirth;
    public static event System.Action<string> OnAnimalDeath;
    public static event System.Action<Animal> OnAnimalDied;
    public static Dictionary<string, int> KillCounts = new Dictionary<string, int>();
    public float MaxAge = 20f;
    public static Dictionary<string, int> SpeciesCount = new Dictionary<string, int>();
    public AnimalSpawner spawner;

    private IAstarAI pathfinder;
    private float nextReproductionTime = 0f;
    private float eatingCounter = 0f;
    private float pregnancyCounter = 0f;
    private bool isPregnant = false;
    private Animal targetPrey = null;
    private int landLayerMask;

    void Start()
    {
        pathfinder = GetComponent<IAstarAI>();
        InitializeAnimal();
        landLayerMask = LayerMask.GetMask("Ground"); // Replace "Land" with the name of your land layer
    }
    void Awake()
    {
        // Update counts based on gender
        if (gender == Gender.Male) MaleCount++;
        else FemaleCount++;

        // Update species count
        string species = gameObject.tag;
        if (!SpeciesCount.ContainsKey(species))
        {
            SpeciesCount[species] = 0;
        }
        SpeciesCount[species]++;
    }
    void Update()
    {
        if (eatingCounter <= 0)
        {
            Roam();
            CheckHunger();
            HandleReproduction();
            HandleHealth();
            IncrementAge();
        }
        else
        {
            eatingCounter -= Time.deltaTime;
        }
    }

    void InitializeAnimal()
    {
        nextReproductionTime = Time.time + reproductionCooldown;
        pregnancyCounter = pregnancyTime;
    }

    void IncrementAge()
    {
        age += Time.deltaTime;
    }
    void RecordKill(string animalType)
    {
        if (!KillCounts.ContainsKey(animalType))
        {
            KillCounts[animalType] = 0;
        }
        KillCounts[animalType]++;

        // Find UIManager and request UI update
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateKillsScoreboard();
        }
    }


    void Roam()
    {
        if (Random.value < 0.5f) // Animals have a 50% chance to move each update
        {
            Vector3 roamTarget = transform.position;
            bool isTargetOnLand;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                if (attempts++ > maxAttempts) break;
                roamTarget = CalculateRoamTarget();
                isTargetOnLand = CheckIfLand(roamTarget);
            }
            while (!isTargetOnLand);

            pathfinder.destination = roamTarget;
            pathfinder.SearchPath();
        }
    }

    Vector3 CalculateRoamTarget()
    {
        if (this == packLeader || packLeader == null || dietType == DietType.Herbivore)
        {
            return transform.position + new Vector3(Random.Range(-roamRadius, roamRadius), 0, Random.Range(-roamRadius, roamRadius));
        }
        else
        {
            return packLeader.transform.position + new Vector3(Random.Range(-roamRadius, roamRadius), 0, Random.Range(-roamRadius, roamRadius));
        }
    }

    bool CheckIfLand(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 100, Vector3.down, out hit, Mathf.Infinity, landLayerMask))
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("Land");
        }
        return false;
    }

    void CheckHunger()
    {
        hunger += Time.deltaTime;
        if (hunger >= hungerThreshold)
        {
            if (dietType == DietType.Herbivore)
            {
                FindPlants();
            }
            else if (dietType == DietType.Carnivore)
            {
                HuntHerbivores();
            }

            // Health decays over time when hungry
            health -= healthDecayRate * Time.deltaTime;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    void FindPlants()
    {
        // Herbivores find plants to eat
        Plant nearestPlant = FindNearestPlant();
        if (nearestPlant != null)
        {
            pathfinder.destination = nearestPlant.transform.position;
            pathfinder.SearchPath();

            if (Vector3.Distance(transform.position, nearestPlant.transform.position) < 1f) // Close enough to eat
            {
                nearestPlant.Consume(10f); // Consume part of the plant
                hunger = 0; // Reset hunger after eating
            }
        }
    }

    Plant FindNearestPlant()
    {
        Plant[] plants = FindObjectsOfType<Plant>();
        return plants.OrderBy(p => Vector3.Distance(this.transform.position, p.transform.position)).FirstOrDefault();
    }

    void HuntHerbivores()
    {
        // Carnivores have a sense of where herbivores are
        if (targetPrey == null || !targetPrey.gameObject.activeInHierarchy)
        {
            targetPrey = FindNearestHerbivore();
            if (targetPrey != null)
            {
                // Implement logic for carnivores to have a sense of prey's location
                pathfinder.destination = targetPrey.transform.position;
                pathfinder.SearchPath();
            }
        }
        else if (Vector3.Distance(transform.position, targetPrey.transform.position) < 1f)
        {
            Attack(targetPrey);
        }
    }

    void Attack(Animal target)
    {
        target.health -= attackDamage;
        if (target.health <= 0)
        {
            RecordKill(this.GetType().Name); // Assuming animal type is distinguished by class
            OnAnimalDeath?.Invoke(gameObject.name + " killed " + target.gameObject.name);
            Eat(target);
        }
    }

    void Eat(Animal target)
    {
        // Gain a random amount of food between 20 to 100 based on the age of the target
        float foodGained = Mathf.Lerp(20, 100, target.age / MaxAge);
        hunger = Mathf.Max(0, hunger - foodGained);

        eatingCounter = eatingTime; // Set the eating counter
    }

    void HandleReproduction()
    {
        if (isPregnant)
        {
            pregnancyCounter -= Time.deltaTime;
            if (pregnancyCounter <= 0)
            {
                GiveBirth();
            }
        }
        else if (Time.time > nextReproductionTime)
        {
            Reproduce();
        }
    }

    void Reproduce()
    {
        // Find a mate and reproduce logic
        nextReproductionTime = Time.time + reproductionCooldown;
        isPregnant = true; // For simplicity, assuming immediate pregnancy
        pregnancyCounter = pregnancyTime;
    }

    void GiveBirth()
    {
        Instantiate(babyAnimalPrefab, transform.position, Quaternion.identity);
        isPregnant = false;
        pregnancyCounter = pregnancyTime;
        OnAnimalBirth?.Invoke(gameObject.name + " gave birth");
    }

    void Die()
    {
        // Logging death and updating counts
        OnAnimalDeath?.Invoke(gameObject.name + " died");
        if (gender == Gender.Male) MaleCount--;
        else FemaleCount--;

        string species = gameObject.tag;
        if (SpeciesCount.ContainsKey(species))
        {
            SpeciesCount[species]--;
            spawner.CheckAndRespawnSpeciesIfNeeded();
        }

        gameObject.SetActive(false);
    }
    void HandleHealth()
    {
        if (health <= 0)
        {
            Die();
        }
    }
    Animal FindNearestHerbivore()
    {
        var herbivores = FindObjectsOfType<Animal>().Where(a => a.dietType == DietType.Herbivore && a != this).ToList();
        return herbivores.OrderBy(h => Vector3.Distance(this.transform.position, h.transform.position)).FirstOrDefault();
    }
}
