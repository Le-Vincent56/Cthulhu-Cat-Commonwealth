using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FOWVisionEmitter))]
public class FOWVisualizationEditor : Editor
{
    private void OnSceneGUI()
    {
        FOWVisionEmitter visionEmitter = (FOWVisionEmitter)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(visionEmitter.transform.position, Vector3.back, Vector3.up, 360, visionEmitter.VisionRadius);

        Handles.color = Color.red;
        foreach (var t in visionEmitter.VisibleTargets)
        {
            Handles.DrawLine(visionEmitter.transform.position, t.position);
        }
    }
}
