using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameArea))]
public class GameAreaEditor : Editor
{
    private SerializedProperty areaProperty;
    private SerializedProperty showGizmoProperty;

    private void OnEnable()
    {
        areaProperty = serializedObject.FindProperty("Area");
        showGizmoProperty = serializedObject.FindProperty("ShowGizmo");
    }

    private void OnSceneGUI()
    {
        var gameArea = (GameArea)target;

        if (!gameArea.ShowGizmo)
            return;

        // Retrieve the RectInt area
        var area = gameArea.Area;

        // Convert RectInt to world positions
        var bottomLeft = new Vector3(area.xMin, area.yMin, 0);
        var topLeft = new Vector3(area.xMin, area.yMax, 0);
        var bottomRight = new Vector3(area.xMax, area.yMin, 0);
        var topRight = new Vector3(area.xMax, area.yMax, 0);

        // Draw the rectangle outline
        Handles.color = Color.green;
        Handles.DrawAAPolyLine(3, bottomLeft, bottomRight, topRight, topLeft, bottomLeft);

        // Add position handles to adjust the bottom-left and top-right corners
        EditorGUI.BeginChangeCheck();
        var newBottomLeft = Handles.PositionHandle(bottomLeft, Quaternion.identity);
        var newTopRight = Handles.PositionHandle(topRight, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gameArea, "Adjust GameArea Rect");

            // Calculate the new RectInt values based on updated positions
            var newX = Mathf.RoundToInt(newBottomLeft.x);
            var newY = Mathf.RoundToInt(newBottomLeft.y);
            var newWidth = Mathf.RoundToInt(newTopRight.x - newBottomLeft.x);
            var newHeight = Mathf.RoundToInt(newTopRight.y - newBottomLeft.y);

            gameArea.Area = new RectInt(newX, newY, newWidth, newHeight);
            EditorUtility.SetDirty(gameArea);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(showGizmoProperty);
        EditorGUILayout.PropertyField(areaProperty);

        serializedObject.ApplyModifiedProperties();
    }
}