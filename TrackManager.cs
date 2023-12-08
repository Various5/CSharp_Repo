using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TrackManager : MonoBehaviour
{
    public List<GameObject> waypoints = new List<GameObject>();
    public GameObject waypointPrefab;
    public GameObject trackPrefab;
    public Material lineMaterial;
    public int curveResolution = 20; // Determines how smooth the curve will be

    private void OnDrawGizmos()
    {
        if (waypoints.Count > 1)
        {
            Vector3 prevPoint = waypoints[0].transform.position;

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    for (int j = 0; j <= curveResolution; j++)
                    {
                        float t = j / (float)curveResolution;
                        Vector3 curvePoint = GetBezierPoint(waypoints[i].transform.position, waypoints[i + 1].transform.position, t);
                        Gizmos.DrawLine(prevPoint, curvePoint);
                        prevPoint = curvePoint;
                    }

                    Gizmos.DrawLine(prevPoint, waypoints[i + 1].transform.position);
                    prevPoint = waypoints[i + 1].transform.position;
                }
            }
        }
    }

    public void AddWaypoint()
    {
        GameObject waypoint = Instantiate(waypointPrefab, Vector3.zero, Quaternion.identity);
        waypoint.transform.SetParent(transform);
        waypoints.Add(waypoint);
        Selection.activeGameObject = waypoint;
    }

    public void CreateTrack()
    {
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3 prevPoint = waypoints[i].transform.position;

            for (int j = 0; j <= curveResolution; j++)
            {
                float t = j / (float)curveResolution;
                Vector3 curvePoint = GetBezierPoint(waypoints[i].transform.position, waypoints[i + 1].transform.position, t);
                Quaternion rotation = Quaternion.LookRotation(curvePoint - prevPoint);
                Instantiate(trackPrefab, curvePoint, rotation);

                prevPoint = curvePoint;
            }
        }
    }

    private Vector3 GetBezierPoint(Vector3 start, Vector3 end, float t)
    {
        // Simple linear interpolation for now, can be expanded to more complex Bezier curves
        return Vector3.Lerp(start, end, t);
    }
}

[CustomEditor(typeof(TrackManager))]
public class TrackManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrackManager trackManager = (TrackManager)target;

        if (GUILayout.Button("Add Waypoint"))
        {
            trackManager.AddWaypoint();
        }

        if (GUILayout.Button("Create Track"))
        {
            trackManager.CreateTrack();
        }
    }
}
