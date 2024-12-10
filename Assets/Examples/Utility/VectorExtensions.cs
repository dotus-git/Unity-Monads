using System.Runtime.CompilerServices;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Dotus.Core
{
    public static class VectorExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 V2(float x, float y) => new Vector2(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int V2I(int x, int y) => new Vector2Int(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 V3(float x, float y) => new Vector3(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 V3(float x, float y, float z) => new Vector3(x, y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int V3I(int x, int y) => new Vector3Int(x, y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int V3I(int x, int y, int z) => new Vector3Int(x, y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithX(this Vector3 copy, float x) => new(x, copy.y, copy.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithY(this Vector3 copy, float y) => new(copy.x, y, copy.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithZ(this Vector3 copy, float z) => new(copy.x, z, copy.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToV3(this Vector2 vector2, float z = 0f) =>
            new(vector2.x, vector2.y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToV2I(this Vector2 vector2) =>
            new(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToV3I(this Vector2 vector2, int z = 0) =>
            new(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y), z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToV2(this Vector2Int vector2Int) =>
            new(vector2Int.x, vector2Int.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToV3(this Vector2Int vector2Int, float z = 0f) =>
            new(vector2Int.x, vector2Int.y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToV3I(this Vector2Int vector2Int, int z = 0) =>
            new(vector2Int.x, vector2Int.y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToV2(this Vector3 vector3) =>
            new(vector3.x, vector3.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToV2I(this Vector3 vector3) =>
            new(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToV3I(this Vector3 vector3) =>
            new(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToV2(this Vector3Int vector3Int) => 
            new(vector3Int.x, vector3Int.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToV2I(this Vector3Int vector3Int) => 
            new(vector3Int.x, vector3Int.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToV3(this Vector3Int vector3Int) => 
            new(vector3Int.x, vector3Int.y, vector3Int.z);
        
        public static int DistanceTo(this Vector2Int from, Vector2Int to) =>
            Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }
}
