using System.Collections;
using Monads;
using NUnit.Framework;
using UnityEngine;

public class SimpleTests : MonoBehaviour
{
    public GameObject SomeGameObject;
    public GameObject EmptyGameObject;

    private Color color;

    // for readability, any LogError() is a condition that should never happen
    // Any Log() is expected, despite what message contains
    private void Start()
    {
        // some / none, Option<T>
        GameObject go = null;
        var result = go.ToResult();
        result.DoWhenFailure(_ => Debug.Log("null is always a failure condition"));
        Assert.AreEqual(result, NullReference.Failure);
        
        // this code doesn't crash even if the SpriteRenderer doesn't exist
        var spriteRenderer = gameObject.ToResult().GetSafeComponent<SpriteRenderer>();
        spriteRenderer.Do(
            render =>
            {
                render.color = Color.red;
                Debug.LogError($"{gameObject.name} made a sprite red without a SpriteRenderer!");
            });

        // we want to find the component on our own GameObject first,
        // then optionally check our children when that fails.
        // let's use fluent (chaining) syntax from here on
        gameObject
            .ToResult()
            .GetSafeComponent<SpriteRenderer>()
            .DefaultWith(_ => gameObject.ToResult().GetSafeComponentInChildren<SpriteRenderer>())
            .Do(render =>
                {
                    render.color = Color.blue;
                    color = render.color; // also, we want to set some member field or property now, so let's do that
                }
            );
        if (color == Color.blue)
        {
            Debug.Log($"SpriteRenderer Found in children of {gameObject.name}");
        }

        // note: I'm not suggesting checking for components on this GameObject,
        // then checking for its existence in all children, is a great approach in Unity.
        // 
        // if it is, and it's something we want to do often, we could make a new extension method for that
        // including accepting a gameObject -or- an IResult<GameObject> as the source
        // and logging a warning when the desired BoxCollider2D isn't found
        gameObject
            .ToResult()
            .WithSafeComponent<AudioSource>(
            audioSource =>
            {
                audioSource.enabled = false;
                Debug.LogError("We should never get here, no AudioSource on this GameObject");
            });

        // we don't have a collider, so the code inside the 'Do' never happens 
        gameObject
            .ToResult()
            .GetSafeComponent<Collider2D>()
            .Do(collide => Debug.LogError($"We hit {collide.gameObject.name} and we don't even have a collider!"))
            .DoWhenFailure(fail => Debug.Log($"Expected Collider Failure - {fail}"));

        // this GameObject exists and has a SpriteRenderer, so it turns green
        SomeGameObject
            .ToResult()
            .GetSafeComponent<SpriteRenderer>()
            .Do(render => render.color = Color.green)
            .Do(_ => Debug.Log($"SpriteRenderer found on {SomeGameObject.name}."));

        // doesn't exist, so we don't get a black sprite anywhere in the game view
        EmptyGameObject
            .ToResult()
            .GetSafeComponent<SpriteRenderer>()
            .Do(render => render.color = Color.black)
            .DoWhenFailure(_ => Debug.Log("EmptyGameObject is null and fails, as expected."));
        
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        // test the Unity "fake null" issue against the Result constructor
        var clone = new GameObject("Fake Null Test");
        var result = clone.ToResult();
        result.Switch(
            success =>
            {
                Destroy(success); // clone won't actually go away until the next frame

                var shouldSucceed = success.ToResult();
                shouldSucceed.DoWhenFailure(fail => Debug.LogError($"Unity Object shouldn't have been a failure {fail} yet"));
            },
            failure => Debug.LogError($"Initial Clone ToResult failed unexpectedly. {failure}")
        );
        yield return null; // wait for the next frame
        
        var shouldFailNow = clone.ToResult();
        shouldFailNow.Switch(
            _ =>
            {
                Debug.LogError("Clone should have been null and a failure, but was not.");
            },
            _ => Debug.Log("Clone was destroyed next frame and failed appropriately.")
        );
        yield return null; // wait for the next frame
        
        clone = Instantiate(gameObject); // clone is OK again
        DestroyImmediate(clone); 
        var shouldFailImmediately = clone.ToResult();
        shouldFailImmediately.Switch(
            _ =>
            {
                Debug.LogError("Clone should have been null and a failure immediately, but was not.");
            },
            _ => Debug.Log("Clone was destroyed immediately and failed appropriately.")
        );
    }
}