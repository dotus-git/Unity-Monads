using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(LevelSystem))]
public class LevelSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default Inspector
        DrawDefaultInspector();

        // Add a custom button
        LevelSystem levelSystem = (LevelSystem)target;
        if (GUILayout.Button("Clear Walls"))
        {
            levelSystem.ClearOutsideWalls();
        }
        
        if (GUILayout.Button("Draw Walls"))
        {
            levelSystem.DrawOutsideWalls();
        }
    }
}