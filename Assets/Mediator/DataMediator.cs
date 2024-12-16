using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityUtils;

/// <summary>
/// DataMediator is a garbage-free messaging system for Unity that facilitates communication
/// between components and systems using a mediator pattern. It uses reflection to automatically
/// register methods decorated with [MediatorHandler] as handlers for specific message types.
/// 
/// Supports:
/// - "Send" for single handler requests with a response.
/// - "Publish" for multicast messages to multiple subscribers.
/// 
/// This implementation ensures minimal overhead while offering flexibility for both MonoBehaviour 
/// and regular C# classes.
/// </summary>
public class DataMediator
{
    private static DataMediator _instance;

    private const string UserCodeAssembly = "Assembly-CSharp";

    /// <summary>
    /// Stores single-request handlers mapped as:
    /// (RequestType, ResponseType) -> Delegate
    /// Used for Send().
    /// </summary>
    private readonly Dictionary<(Type, Type), Delegate> _singleHandlers = new();

    /// <summary>
    /// Stores multicast handlers for a request type:
    /// RequestType -> List of Delegates
    /// Used for Publish().
    /// </summary>
    private readonly Dictionary<Type, List<Delegate>> _multiHandlers = new();
    
    /// <summary>
    /// Private constructor to initialize the mediator and register all handlers using reflection.
    /// </summary>
    private DataMediator()
    {
        RegisterHandlers();
    }
    
    /// <summary>
    /// Provides access to the singleton instance, creating it if necessary.
    /// </summary>
    public static DataMediator Instance => _instance ??= new DataMediator();

    /// <summary>
    /// Cleans up the static instance when exiting Play Mode in the Unity Editor.
    /// Ensures proper re-initialization on subsequent runs.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlayMode()
    {
        Debug.Log("DataMediator: Resetting instance on play mode reload.");
        _instance = null;
    }
    
    /// <summary>
    /// Sends a message to a single handler and gets a response.
    /// </summary>
    /// <typeparam name="TRequest">Struct representing the request message type.</typeparam>
    /// <typeparam name="TResponse">Struct representing the response message type.</typeparam>
    /// <param name="message">The request message.</param>
    /// <returns>The response from the registered handler.</returns>
    public TResponse Send<TRequest, TResponse>(in TRequest message)
        where TRequest : struct
        where TResponse : struct
    {
        var key = (typeof(TRequest), typeof(TResponse));

        if (!_singleHandlers.TryGetValue(key, out var handler))
            throw new InvalidOperationException($"No handler registered for {typeof(TRequest)} -> {typeof(TResponse)}");

        // Cast and invoke the handler with the request message
        var func = (Func<TRequest, TResponse>)handler;
        return func.Invoke(message);
    }

    /// <summary>
    /// Publishes a message to all registered multicast subscribers.
    /// </summary>
    /// <typeparam name="TRequest">Struct representing the request message type.</typeparam>
    /// <param name="message">The message to be published.</param>
    public void Publish<TRequest>(in TRequest message)
        where TRequest : struct
    {
        var key = typeof(TRequest);

        if (!_multiHandlers.TryGetValue(key, out var handlers))
            return; // No handlers registered, simply return.

        foreach (var handler in handlers)
        {
            var action = (Action<TRequest>)handler;
            action.Invoke(message);
        }
    }
    
    /// <summary>
    /// Registers handler methods using reflection.
    /// Scans all types for methods decorated with [MediatorHandler].
    /// </summary>
    private void RegisterHandlers()
    {
        var methods = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.FullName.StartsWith(UserCodeAssembly)) 
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsPublic) // Skip interfaces, abstracts, and non-public types
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(method => method.GetCustomAttribute<MediatorHandlerAttribute>() != null);

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                throw new InvalidOperationException($"Handler '{method.Name}' must have exactly one parameter.");

            var requestType = parameters[0].ParameterType;
            var responseType = method.ReturnType;

            if (!requestType.IsValueType || !responseType.IsValueType)
                throw new InvalidOperationException($"Handler '{method.Name}' must use structs for both request and response types.");

            if (responseType == typeof(void))
            {
                // Multicast handler registration (for Publish)
                RegisterMulticastHandler(method, requestType);
            }
            else
            {
                // Single handler registration (for Send)
                RegisterSingleHandler(method, requestType, responseType);
            }
        }
    }

    /// <summary>
    /// Registers a single-handler method (for Send).
    /// </summary>
    private void RegisterSingleHandler(MethodInfo method, Type requestType, Type responseType)
    {
        if (!responseType.IsValueType)
            throw new InvalidOperationException($"Handler '{method.Name}' return type must be a struct type.");

        object targetInstance;

        if (method.IsStatic)
        {
            // Static methods don't need an instance
            targetInstance = null;
        }
        else
        {
            // Check if DeclaringType is a MonoBehaviour
            var declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                return;
            }
            
            if (typeof(MonoBehaviour).IsAssignableFrom(declaringType))
            {
                targetInstance = TryGetSingletonInstance(declaringType) ?? CreateMonoBehaviourInstance(declaringType);
            }
            else
            {
                // Regular C# class - instantiate as usual
                targetInstance = Activator.CreateInstance(declaringType);
            }
        }

        var handlerDelegate = Delegate.CreateDelegate(
            typeof(Func<,>).MakeGenericType(requestType, responseType),
            targetInstance,
            method
        );

        _singleHandlers[(requestType, responseType)] = handlerDelegate;
    }

    /// <summary>
    /// Registers a multicast-handler method (for Publish).
    /// </summary>
    private void RegisterMulticastHandler(MethodInfo method, Type requestType)
    {
        object targetInstance;

        if (method.IsStatic)
        {
            targetInstance = null; // Static methods don't need an instance
        }
        else
        {
            var declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                return;
            }
            
            if (typeof(MonoBehaviour).IsAssignableFrom(declaringType))
            {
                targetInstance = TryGetSingletonInstance(declaringType) ?? CreateMonoBehaviourInstance(declaringType);
            }
            else
            {
                // Regular C# class - instantiate as usual
                targetInstance = Activator.CreateInstance(declaringType);
            }
        }

        var handlerDelegate = Delegate.CreateDelegate(
            typeof(Action<>).MakeGenericType(requestType),
            targetInstance,
            method
        );

        if (!_multiHandlers.TryGetValue(requestType, out var handlers))
        {
            handlers = new List<Delegate>();
            _multiHandlers[requestType] = handlers;
        }

        handlers.Add(handlerDelegate);
    }
    
    private object TryGetSingletonInstance(Type type)
    {
        // Check if the type inherits from Singleton<T>
        var singletonBase = GetSingletonBaseType(type);
        if (singletonBase == null)
            return null;

        // Use reflection to call the 'Instance' property on Singleton<T>
        var instanceProperty = singletonBase.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        return instanceProperty?.GetValue(null);
    }

    private Type GetSingletonBaseType(Type type)
    {
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Singleton<>))
                return type;

            type = type.BaseType;
        }

        return null;
    }

    private object CreateMonoBehaviourInstance(Type declaringType)
    {
        // Fallback: Attach MonoBehaviour to a new GameObject
        var go = new GameObject(declaringType.Name);
        return go.AddComponent(declaringType);
    }
}