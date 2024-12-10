using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject OutsideWallContainer;
    public GameObject OutsideWall;
    public List<GameObject> PowerUps;
    public List<GameObject> Enemies;

    public void ClearOutsideWalls()
    {
        // Loop through all child transforms and destroy them
        while (OutsideWallContainer.transform.childCount > 0)
        {
            foreach (Transform child in OutsideWallContainer.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public void DrawOutsideWalls()
    {
        ClearOutsideWalls();
        
        // Get the main camera
        var cam = MainCamera ?? Camera.main;

        if (!cam || !OutsideWall)
        {
            Debug.LogError("Camera or OutsideWall prefab not set!");
            return;
        }

        // Calculate visible screen size
        var screenHeight = (int)(cam.orthographicSize * 2);
        var screenWidth = (int)(screenHeight * cam.aspect);

        // Half dimensions
        var halfWidth = (int)Mathf.Floor(screenWidth / 2f);
        var halfHeight = (int)Mathf.Floor(screenHeight / 2f);

        // Draw walls along all 4 edges
        DrawWallRow(new Vector3(-halfWidth + 1, halfHeight - 3, 0f), Vector3.right, screenWidth - 2, 180); // Top (North)
        DrawWallRow(new Vector3(-halfWidth + 1, -halfHeight + 1, 0f), Vector3.right, screenWidth - 2, 0); // Bottom (South)
        DrawWallRow(new Vector3(-halfWidth, -halfHeight + 2, 0f), Vector3.up, screenHeight - 5, 270); // Left (West)
        DrawWallRow(new Vector3(+halfWidth, -halfHeight + 2, 0f), Vector3.up, screenHeight - 5, 90); // Right (East)
    }

    private void DrawWallRow(Vector3 startPosition, Vector3 direction, float totalLength, float rotationZ)
    {
        var numberOfSegments = totalLength;

        for (var i = 0; i < numberOfSegments; i++)
        {
            // Calculate position of each wall segment
            var position = startPosition + direction * i;

            // Instantiate wall segment
            var wall = Instantiate(OutsideWall, position, Quaternion.Euler(0, 0, rotationZ), OutsideWallContainer.transform);
            wall.name = $"OutsideWall_{rotationZ}_{i}";
            wall.transform.parent = OutsideWallContainer.transform;
        }
    }
}