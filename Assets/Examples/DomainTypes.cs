using Monads;
using UniMediator;
using UnityEngine;

// simplify typing for requests and responses
public record SingleResult : ISingleMessage<Result>;
public record SingleResult<T> : ISingleMessage<Result<T>>;

// game level events
public record NewGame : IMulticastMessage;

// game-specific requests and responses
public record MoveUnit(GameObject Unit, Vector2Int Destination) : SingleResult<MoveUnitResponse>;
public record MoveUnitResponse(Vector2Int LandingPosition, int ActionPoints) : SingleResult;
public record RegisterUnit(GameObject Unit) : SingleResult;
public record DetectUnit(Vector2Int Position) : SingleResult<GameObject>;

public record RegisterLoot(GameObject Unit) : SingleResult;
public record GetLoot(GameObject Unit, GameObject Loot) : IMulticastMessage;
public record DetectLoot(Vector2Int Position) : SingleResult<GameObject>;

public record RegisterObstacle(GameObject Obstacle) : ISingleMessage<Result>;
public record DetectObstacle(Vector2Int Position) : SingleResult<GameObject>;

public record AddGold(int Amount) : IMulticastMessage;
public record AddScore(int Amount) : IMulticastMessage;

// easily add your own failure types specific to your game
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