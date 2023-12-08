using UnityEngine;

public class TerrainDeformer : MonoBehaviour
{
    public Terrain terrain;
    private TerrainData terrainData;
    private float[,] originalHeights; // Store the original heightmap

    public float craterSize = 5f;
    public float craterDepth = 1f;
    public static TerrainDeformer Instance { get; private set; }
    void Start()
    {
        terrainData = terrain.terrainData;
        originalHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void DeformTerrain(Vector3 hitPoint, ArtilleryShell shell)
    {
        // Convert hit point to terrain's local space
        hitPoint = terrain.transform.InverseTransformPoint(hitPoint);
        // Normalize the coordinates
        Vector3 normalizedPos = new Vector3(hitPoint.x / terrainData.size.x, 0, hitPoint.z / terrainData.size.z);
        int x = (int)(normalizedPos.x * terrainData.heightmapResolution);
        int z = (int)(normalizedPos.z * terrainData.heightmapResolution);
        int craterSizeInPixels = (int)(craterSize / terrainData.size.x * terrainData.heightmapResolution);

        float[,] heights = terrainData.GetHeights(x - craterSizeInPixels / 2, z - craterSizeInPixels / 2, craterSizeInPixels, craterSizeInPixels);

        for (int i = 0; i < craterSizeInPixels; i++)
        {
            for (int j = 0; j < craterSizeInPixels; j++)
            {
                float distanceToCenter = Vector2.Distance(new Vector2(i, j), new Vector2(craterSizeInPixels / 2, craterSizeInPixels / 2));
                if (distanceToCenter < craterSizeInPixels / 2)
                {
                    heights[i, j] -= craterDepth / terrainData.size.y;
                }
            }
        }

        terrainData.SetHeights(x - craterSizeInPixels / 2, z - craterSizeInPixels / 2, heights);
    }

    void OnDisable() // This will be called when the game stops
    {
        // Restore the original heightmap when the game stops
        terrainData.SetHeights(0, 0, originalHeights);
    }
}
