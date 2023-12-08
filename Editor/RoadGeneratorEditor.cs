using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RoadGenerator))]
public class RoadGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoadGenerator roadGenerator = (RoadGenerator)target;

        if (GUILayout.Button("Add Waypoint"))
        {
            // Add a new waypoint at the center of the scene view
            Vector3 sceneCenter = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            roadGenerator.AddWaypoint(sceneCenter);
        }

        if (GUILayout.Button("Create Roads"))
        {
            roadGenerator.CreateRoads();
        }

        if (GUILayout.Button("Undo Last Road"))
        {
            roadGenerator.UndoLastRoad();
        }
    }
}
