using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Profiling.RawFrameDataView;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;
    public int spawnCount = 5;

    [Header("Randomized Properties")]
    public Vector2 roamSpeedRange = new Vector2(1, 5);
    public Vector2 healthRange = new Vector2(50, 100);
    public Vector2 hungerRange = new Vector2(0, 5);
    public Vector2 roamRadiusRange = new Vector2(5, 15);
    public Vector2 reproductionCooldownRange = new Vector2(20, 40);
    public Vector2 attackDamageRange = new Vector2(5, 15);
    public Vector2 eatingTimeRange = new Vector2(3, 7);
    public static List<AnimalSpawner> AllSpawners = new List<AnimalSpawner>();
    void Awake()
    {
        AllSpawners.Add(this);
    }

    void Start()
    {
        Animal.OnAnimalBirth += LogEvent;
        Animal.OnAnimalDeath += LogEvent;
        Animal.OnAnimalDied += HandleAnimalDeath;

        SpawnAnimals();
    }
    private void SpawnAnimals()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnAnimal(animalPrefab, (Gender)Random.Range(0, 2)); // Random gender
        }
    }
    public void RespawnExtinctSpecies(GameObject animalPrefab)
    {
        // Respawn a male and a female
        SpawnAnimal(animalPrefab, Gender.Male);
        SpawnAnimal(animalPrefab, Gender.Female);
    }
    void OnDestroy()
    {
        Animal.OnAnimalBirth -= LogEvent;
        Animal.OnAnimalDeath -= LogEvent;
        Animal.OnAnimalDied -= HandleAnimalDeath;
    }
    public void CheckAndRespawnSpeciesIfNeeded()
    {
        if (Animal.MaleCount == 0 && Animal.FemaleCount == 0)
        {
            RespawnExtinctSpecies(animalPrefab);
        }
    }
    void LogEvent(string message)
    {
        Debug.Log(message);
    }
    private void HandleAnimalDeath(Animal animal)
    {
        CheckAndRespawnSpeciesIfNeeded();
    }
    private void SpawnAnimal(GameObject animalPrefab, Gender gender)
    {
        GameObject newAnimalObj = Instantiate(animalPrefab, transform.position, Quaternion.identity);
        Animal newAnimal = newAnimalObj.GetComponent<Animal>();

        Animal packLeader = null;
        for (int i = 0; i < spawnCount; i++)
        {


            if (newAnimal != null)
            {
                // Randomize properties
                newAnimal.gender = gender;
                newAnimal.gender = (Gender)Random.Range(0, 2); // Randomly choose between 0 (Male) and 1 (Female)
                newAnimal.health = Random.Range(healthRange.x, healthRange.y);
                newAnimal.hunger = Random.Range(hungerRange.x, hungerRange.y);
                newAnimal.roamRadius = Random.Range(roamRadiusRange.x, roamRadiusRange.y);
                newAnimal.reproductionCooldown = Random.Range(reproductionCooldownRange.x, reproductionCooldownRange.y);
                newAnimal.attackDamage = Random.Range(attackDamageRange.x, attackDamageRange.y);
                newAnimal.eatingTime = Random.Range(eatingTimeRange.x, eatingTimeRange.y);
                newAnimal.roamSpeed = Random.Range(roamSpeedRange.x, roamSpeedRange.y);
                // Assign the first animal as the pack leader or add to existing pack
                if (i == 0)
                {
                    packLeader = newAnimal;
                    newAnimal.packLeader = newAnimal; // The leader is its own leader
                }
                else if (packLeader != null)
                {
                    newAnimal.packLeader = packLeader;
                    packLeader.packMembers.Add(newAnimal);
                }
            }
        }
    }
}
