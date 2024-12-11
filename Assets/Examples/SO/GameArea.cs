using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameArea", menuName = "SO/Game Area")]
public class GameArea : ScriptableObject
{
    [Header("Game Area Configuration")]
    [Tooltip("Defines the area as an integer rectangle (x, y, width, height).")]
    public RectInt Area = new(0, 0, 10, 10);

    [Tooltip("Show the gizmo in the editor for visualization.")]
    public bool ShowGizmo = true;

#if UNITY_EDITOR

    /// <summary>
    ///     Draws the gizmo in the Scene view when this GameArea is being inspected.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!ShowGizmo) return;

        // Set gizmo color
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Semi-transparent green

        // Draw a rectangle using the Area property
        var position = new Vector3(Area.x, Area.y, 0);
        var size = new Vector3(Area.width, Area.height, 0);

        Gizmos.DrawCube(position + size / 2, size);

        // Outline for better visibility
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position + size / 2, size);
    }
#endif
}