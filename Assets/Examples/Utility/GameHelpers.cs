using System.Collections.Generic;
using UnityEngine;

public static class GameHelpers
{
    public static Vector2Int GetRandomPosition(this RectInt rect, List<Vector2Int> occupiedPositions = null)
    {
        // Keep trying to find a random position until we find one that is not occupied
        while (true)
        {
            var randomX = Random.Range(rect.xMin, rect.xMax); // Inclusive xMin, exclusive xMax
            var randomY = Random.Range(rect.yMin, rect.yMax); // Inclusive yMin, exclusive yMax

            var randomPosition = new Vector2Int(randomX, randomY);

            if (occupiedPositions == null)
            {
                return randomPosition;
            }

            if (!occupiedPositions.Contains(randomPosition))
            {
                occupiedPositions.Add(randomPosition);
                return randomPosition;
            }
        }
    }
}