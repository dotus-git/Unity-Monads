using System;
using System.Collections;
using Monads;
using UnityEngine;
using UnityEngine.U2D;

// ReSharper disable UnusedVariable
// ReSharper disable IteratorNeverReturns

// we're not testing the actual functionality of the Result monad or its extensions
// we're testing that no heap garbage is generated
public class GarbageTest : MonoBehaviour
{
    public GameObject LeaveEmpty;
    public BoxCollider2D InitBoxCollider2D;
    public static BoxCollider2D EmptyBoxCollider2D;

    // when writing critical code that runs every frame,
    // the caller is also responsible for ensuring no garbage is generated
    private static readonly Action<GameObject> ToggleMagentaDelegate = ToggleMagenta;
    private static readonly Action<GameObject> NoopDelegate = Noop;
    private static readonly Action<SpriteRenderer> ToggleSpriteDelegate = ToggleVisible;
    private static readonly Action<SpriteRenderer> ToggleFlipXDelegate = ToggleFlipX;

    private static readonly Action<Failure> SimpleFailureDelegate = NoopFailure;
    private static readonly Action<Failure> LogFailureDelegate = LogFailure;

    private static readonly Func<GameObject, Result<GameObject>> MapToFailureDelegate = MapToFailure;
    private static readonly Func<GameObject, SpriteRenderer> GetSpriteRendererDelegate = GetSpriteRenderer;
    private static readonly Func<GameObject, BoxCollider2D> GetBoxCollider2DDelegate = GetBoxCollider2D;

    private static readonly Func<Failure, SpriteRenderer> LogSpriteFailureDelegate = LogSpriteFailure;
    private static readonly Func<Failure, BoxCollider2D> FallbackToColliderDelegate = FallbackToCollider;

    private void Start()
    {
        EmptyBoxCollider2D = InitBoxCollider2D;

        // by running these coroutines every frame
        // any garbage will show in the profiler
        StartCoroutine(nameof(TestNewResult));
        StartCoroutine(nameof(TestNewFailure));
        StartCoroutine(nameof(TestResultSwitchSuccess));
        StartCoroutine(nameof(TestResultSwitchFailure));
        StartCoroutine(nameof(TestResultMatchSuccess));
        StartCoroutine(nameof(TestResultMatchFailure));

        // test extensions
        StartCoroutine(nameof(TestToResult));
        StartCoroutine(nameof(TestFailureToResult));
        StartCoroutine(nameof(TestMapSuccess));
        StartCoroutine(nameof(TestMapFailure));
        StartCoroutine(nameof(TestDoSuccess));
        StartCoroutine(nameof(TestDoFailure));
        StartCoroutine(nameof(TestDefaultWith));
        
        // test Unity extensions
        StartCoroutine(nameof(TestGetSafeComponent));
        StartCoroutine(nameof(TestGetSafeComponentInChildren));
        StartCoroutine(nameof(TestWithSafeComponent));
    }

    #region Result Methods
    
    [GarbageFree]
    private IEnumerator TestToResult()
    {
        while (true)
        {
            var go = gameObject.ToResult();

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestFailureToResult()
    {
        while (true)
        {
            var go = Failure.Default.ToResult();

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestNewResult()
    {
        while (true)
        {
            var go = new Result<GameObject>(gameObject);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestNewFailure()
    {
        while (true)
        {
            var go = new Result<GameObject>(Failure.Default);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestResultSwitchSuccess()
    {
        while (true)
        {
            gameObject
                .ToResult()
                .Switch(ToggleMagentaDelegate);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestResultSwitchFailure()
    {
        while (true)
        {
            gameObject
                .ToResult()
                .Map(MapToFailureDelegate)
                .Switch(NoopDelegate, SimpleFailureDelegate);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestResultMatchSuccess()
    {
        while (true)
        {
            gameObject
                .ToResult()
                .Match(
                    GetSpriteRendererDelegate,
                    LogSpriteFailureDelegate
                );

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestResultMatchFailure()
    {
        while (true)
        {
            gameObject
                .ToResult()
                .Map(MapToFailureDelegate)
                .Match(
                    GetSpriteRendererDelegate,
                    LogSpriteFailureDelegate
                );

            yield return null;
        }
    }
    
    #endregion

    #region Result Extensions 
    
    [GarbageFree]
    private IEnumerator TestMapSuccess()
    {
        while (true)
        {
            var go = gameObject
                .ToResult()
                .Map(GetSpriteRendererDelegate);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestMapFailure()
    {
        while (true)
        {
            var go = gameObject
                .ToResult()
                .Map(MapToFailureDelegate)
                .Map(GetSpriteRendererDelegate);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestDoSuccess()
    {
        while (true)
        {
            gameObject
                .ToResult()
                .Map(GetSpriteRendererDelegate)
                .OnSuccess(ToggleSpriteDelegate);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestDoFailure()
    {
        while (true)
        {
            LeaveEmpty
                .ToResult()
                .Map(GetSpriteRendererDelegate)
                .OnFailure(LogFailureDelegate);

            yield return null;
        }
    }

    [GarbageFree]
    private IEnumerator TestDefaultWith()
    {
        while (true)
        {
            gameObject
                .ToResult()
                .Map(GetBoxCollider2DDelegate)
                .DefaultWith(FallbackToColliderDelegate);

            yield return null;
        }
    }
    
    #endregion

    #region Unity Extensions
    
    [GarbageFree]
    private IEnumerator TestGetSafeComponent()
    {
        while (true)
        {
            var sprite = gameObject
                .ToResult()
                .GetSafeComponent<SpriteRenderer>();

            sprite.OnSuccess(ToggleSpriteDelegate);

            yield return null;
        }
    }
    
    [GarbageFree]
    private IEnumerator TestGetSafeComponentInChildren()
    {
        while (true)
        {
            var sprite = gameObject
                .ToResult()
                .GetSafeComponentInChildren<SpriteRenderer>();

            sprite.OnSuccess(ToggleSpriteDelegate);
            
            yield return null;
        }
    }
    
    [GarbageFree]
    private IEnumerator TestWithSafeComponent()
    {
        while (true)
        {
            gameObject
                .ToResult()
                .WithSafeComponent(ToggleFlipXDelegate);

            yield return null;
        }
    }
    
    #endregion
    
    #region Support Methods
    
    private static BoxCollider2D FallbackToCollider(Failure failure)
        => EmptyBoxCollider2D;

    private static SpriteRenderer LogSpriteFailure(Failure failure)
        => default;

    private static void Noop(GameObject obj) { }

    private static void NoopFailure(Failure failure) { }

    private static Result<GameObject> MapToFailure(GameObject go)
        => Failure.Default;

    private static void LogFailure(Failure failure) { }

    private static SpriteRenderer GetSpriteRenderer(GameObject go)
        => go.GetComponent<SpriteRenderer>();

    private static BoxCollider2D GetBoxCollider2D(GameObject go)
        => go.GetComponent<BoxCollider2D>();

    private static void ToggleMagenta(GameObject go)
    {
        var sprite = go.GetComponent<SpriteRenderer>();
        if (!sprite)
            return;

        var everyOtherSecond = Time.time % 2 < 1;
        sprite.color = everyOtherSecond
            ? Color.magenta
            : Color.white; // change the sprite color every seconds
    }

    private static void ToggleVisible(SpriteRenderer behavior)
    {
        var everyHalfSecond = Time.time * 2 % 2 < 1;
        behavior.enabled = everyHalfSecond; // flash the sprite on and off every half-second
    }
    
    private static void ToggleFlipX(SpriteRenderer sprite)
    {
        sprite.flipX = !sprite.flipX;
    }
    
    #endregion
}