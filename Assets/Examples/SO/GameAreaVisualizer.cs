using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class GameAreaVisualizer : MonoBehaviour
{
    public GameArea gameArea; // Reference to the ScriptableObject

    private void OnDrawGizmos()
    {
        if (gameArea == null || !gameArea.ShowGizmo)
            return;

        DrawGameAreaGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (gameArea == null || !gameArea.ShowGizmo)
            return;

        DrawGameAreaGizmo();
        DrawAndEditHandles();
    }

    private void DrawGameAreaGizmo()
    {
        Gizmos.color = Color.green;

        // Convert RectInt area to world-space positions
        RectInt area = gameArea.Area;
        Vector3 bottomLeft = new Vector3(area.xMin, area.yMin, 0);
        Vector3 topLeft = new Vector3(area.xMin, area.yMax, 0);
        Vector3 bottomRight = new Vector3(area.xMax, area.yMin, 0);
        Vector3 topRight = new Vector3(area.xMax, area.yMax, 0);

        // Draw the rectangular outline
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

    private void DrawAndEditHandles()
    {
        RectInt area = gameArea.Area;

        // Define initial positions for the handles
        Vector3 bottomLeft = new Vector3(area.xMin, area.yMin, 0);
        Vector3 topRight = new Vector3(area.xMax, area.yMax, 0);

        // Handle size and type
        float handleSize = HandleUtility.GetHandleSize(bottomLeft) * 0.1f;

        EditorGUI.BeginChangeCheck();

        // Use Handles.Slider to allow moving corners
        bottomLeft = Handles.Slider2D(
            bottomLeft, Vector3.forward, Vector3.right, Vector3.up, handleSize, Handles.RectangleHandleCap, Vector2.zero);

        topRight = Handles.Slider2D(
            topRight, Vector3.forward, Vector3.right, Vector3.up, handleSize, Handles.RectangleHandleCap, Vector2.zero);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gameArea, "Move Game Area Handles");

            // Update RectInt based on adjusted handles
            gameArea.Area = new RectInt(
                Mathf.RoundToInt(bottomLeft.x),
                Mathf.RoundToInt(bottomLeft.y),
                Mathf.RoundToInt(topRight.x - bottomLeft.x),
                Mathf.RoundToInt(topRight.y - bottomLeft.y)
            );
        }

        // Draw the updated rectangle with Handles
        Handles.color = Color.red;
        Handles.DrawLine(bottomLeft, new Vector3(topRight.x, bottomLeft.y, 0)); // bottom edge
        Handles.DrawLine(bottomLeft, new Vector3(bottomLeft.x, topRight.y, 0)); // left edge
        Handles.DrawLine(topRight, new Vector3(topRight.x, bottomLeft.y, 0));   // right edge
        Handles.DrawLine(topRight, new Vector3(bottomLeft.x, topRight.y, 0));   // top edge
    }
}
