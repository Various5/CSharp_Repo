using UnityEngine;

public class ArtilleryShell : MonoBehaviour
{
    public float speed = 30f;
    public GameObject explosionEffectPrefab;
    public GameObject smokeEffectPrefab;
    public GameObject decalPrefab;
    public AudioClip impactSound;
    public float destructionRange = 10f; // Range within which objects will be destroyed

    private AudioSource impactAudioSource;
    private TerrainDeformer terrainDeformer;
    private Rigidbody shellRigidbody;

    private void Awake()
    {
        shellRigidbody = GetComponent<Rigidbody>();
        impactAudioSource = gameObject.AddComponent<AudioSource>();
        impactAudioSource.clip = impactSound;
    }

    void Start()
    {
        shellRigidbody.velocity = transform.forward * speed;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetTerrainDeformer(TerrainDeformer deformer)
    {
        terrainDeformer = deformer;
    }

    void OnCollisionEnter(Collision collision)
    {
        impactAudioSource.Play();
        Vector3 hitPoint = collision.contacts[0].point;

        if (collision.gameObject.CompareTag("Terrain"))
        {
            TerrainDeformer.Instance.DeformTerrain(hitPoint, this);
        }

        DestroyObjectsAround(hitPoint);
        InstantiateEffects(hitPoint, collision.contacts[0].normal);
        float delayTime = impactAudioSource.clip.length;
        Destroy(gameObject, delayTime);
    }

    private void InstantiateEffects(Vector3 position, Vector3 normal)
    {
        Instantiate(explosionEffectPrefab, position, Quaternion.identity);
        Instantiate(smokeEffectPrefab, position, Quaternion.identity);
        Instantiate(decalPrefab, position, Quaternion.FromToRotation(Vector3.up, normal));
    }

    private void DestroyObjectsAround(Vector3 center)
    {
        int foliageLayer = LayerMask.NameToLayer("Foliage");
        Collider[] hitColliders = Physics.OverlapSphere(center, destructionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == foliageLayer)
            {
                Destroy(hitCollider.gameObject);
            }
        }
    }
}
