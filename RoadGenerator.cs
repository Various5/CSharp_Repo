using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class RoadGenerator : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public GameObject roadPrefab; // Your road prefab or mesh
    public float roadWidth = 3f;
    public Terrain terrain; // Reference to the terrain

    // Undo functionality
    private Stack<List<Vector3>> undoStack = new Stack<List<Vector3>>();

    // Function to add waypoints in the editor
    public void AddWaypoint(Vector3 position)
    {
        GameObject waypointObj = new GameObject("Waypoint " + waypoints.Count);
        waypointObj.transform.position = position;
        waypoints.Add(waypointObj.transform);
    }

    private void OnDrawGizmos()
    {
        // Draw waypoints and path
        Gizmos.color = Color.red;
        foreach (var waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint.position, 1f); // Draw waypoint
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position); // Draw line between waypoints
        }
    }

    // Function to create roads
    public void CreateRoads()
    {
        // Save the current state for undo functionality
        List<Vector3> currentWaypoints = new List<Vector3>();
        foreach (var waypoint in waypoints)
        {
            currentWaypoints.Add(waypoint.position);
        }
        undoStack.Push(currentWaypoints);

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            CreateRoadSegment(waypoints[i].position, waypoints[i + 1].position);
        }

        AdjustTerrain();
        HandleIntersections();
    }

    private void CreateRoadSegment(Vector3 start, Vector3 end)
    {
        Vector3 midPoint = (start + end) / 2;
        GameObject road = Instantiate(roadPrefab, midPoint, Quaternion.identity);
        road.transform.LookAt(end);

        // Adjust texture tiling
        float length = Vector3.Distance(start, end);
        Material roadMaterial = road.GetComponent<Renderer>().material;
        roadMaterial.mainTextureScale = new Vector2(length / roadWidth, 1);

        // Scale road
        road.transform.localScale = new Vector3(roadWidth, 1, length);
    }

    // Function to adjust terrain
    private void AdjustTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        float[,] allHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        foreach (var waypoint in waypoints)
        {
            Vector3 terrainPos = waypoint.position - terrain.transform.position;
            int x = (int)(terrainPos.x / terrainData.size.x * terrainData.heightmapResolution);
            int z = (int)(terrainPos.z / terrainData.size.z * terrainData.heightmapResolution);
            allHeights[z, x] = waypoint.position.y / terrainData.size.y;
        }

        terrainData.SetHeights(0, 0, allHeights);
        RemoveObstacles();
    }

    private void RemoveObstacles()
    {
        // Iterate over the terrain's tree and detail data to remove obstacles
        TerrainData terrainData = terrain.terrainData;
        List<TreeInstance> newTrees = new List<TreeInstance>();

        foreach (TreeInstance tree in terrainData.treeInstances)
        {
            Vector3 treeWorldPos = Vector3.Scale(tree.position, terrainData.size) + terrain.transform.position;
            bool keepTree = true;

            foreach (var waypoint in waypoints)
            {
                if (Vector3.Distance(treeWorldPos, waypoint.position) < roadWidth)
                {
                    keepTree = false;
                    break;
                }
            }

            if (keepTree) newTrees.Add(tree);
        }

        terrainData.treeInstances = newTrees.ToArray();
        terrainData.RefreshPrototypes();
    }

    private void CreateCurvedRoadSegment(Vector3 start, Vector3 controlPoint, Vector3 end)
    {
        // Approximate the Bezier curve with a number of straight segments
        int segmentCount = 20;
        Vector3 lastPoint = start;
        Vector3 currentPoint = Vector3.zero;

        for (int i = 1; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            currentPoint = CalculateBezierPoint(t, start, controlPoint, end);
            CreateStraightRoadSegment(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }
    }
    // Bezier curve calculation
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; //first term
        p += 2 * u * t * p1; //second term
        p += tt * p2; //third term

        return p;
    }
    // Function to handle intersections
    private void HandleIntersections()
    {
        // Detect and create intersections based on overlapping waypoints
        // Basic implementation: Adjusting terrain height for intersection areas
        // More advanced implementations can involve custom intersection prefabs or road shaping
    }

    // Function to undo the last road creation
    public void UndoLastRoad()
    {
        if (undoStack.Count > 0)
        {
            List<Vector3> lastWaypoints = undoStack.Pop();
            waypoints.Clear();
            foreach (var position in lastWaypoints)
            {
                AddWaypoint(position);
            }
            // Additional steps to remove the last created road and adjust the terrain
        }
    }
}
