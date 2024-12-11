// easily add your own failure types specific to your game

using Monads;
using UnityEngine;

public record PathBlocked(Vector2Int Position) : Failure;

public record LimitExceeded(int Max) : Failure
{
    public const int DEFAULT_MAX = 100;
    public static readonly LimitExceeded Failure = new(DEFAULT_MAX);
}

public record MissingComponent(string ComponentName, string GameObjectName) : Failure;

public record SceneNotLoaded(string SceneName) : Failure;

public record InvalidPrefab(string PrefabName) : Failure;

public record OutOfBounds(string ObjectName, string BoundsName) : Failure;

public record AssetLoadFailure(string AssetPath) : Failure;

public record AnimationFailure(string AnimatorName, string StateName) : Failure;

public record AudioFailure(string AudioClipName) : Failure;

public record PhysicsFailure(string ObjectName) : Failure;

public record NullTransform(string GameObjectName) : Failure;

public record InvalidOperation(string OperationName) : Failure;

public record SpawnFailure(string PrefabName, string Position) : Failure;

public record InputFailure(string InputName, string Reason) : Failure;