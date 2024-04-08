using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Exceptions;

namespace SnakeGameSource.GameEngine;

public sealed class GameObject
{
    private const string ParentPropertyName  = "Parent";

    private static readonly MethodInvoker Invoker   = new();
    private static readonly PropertyInfo ParentProperty = typeof(Component).GetProperty(ParentPropertyName)!;

    private readonly List<Component> _componentsList = [];

    public GameObject(string? name = null)
    {
        Name      = name;
        Transform = new Transform { Parent = this };
    }

    public string? Name { get; }

    public Transform Transform { get; }

    public T AddComponent<T>() where T : Component, new()
    {
        Guard.IsNull(GetComponent<T>());

        var component = new T { Parent = this };
        _componentsList.Add(component);
        MethodInvoker.Awake(component);

        return component;
    }

    public Component AddComponent(Type type)
    {
        CheckComponentType(type);

        ConstructorInfo constructor =
            type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, [])
         ?? throw new NullReferenceException($"The component {type.Name} must have a parameterless constructor");

        var component = (Component)constructor.Invoke(null);

        ParentProperty.SetValue(component, this);
        _componentsList.Add(component);

        MethodInvoker.Awake(component);

        return component;
    }

    private void CheckComponentType(Type type)
    {
        if (!type.IsSubclassOf(typeof(Component)))
        {
            throw new Exception($"The component {type.Name} isn't subclass of class 'Component'");
        }

        Guard.IsNull(GetComponent(type));

        if (type.IsAbstract || type.IsInterface)
        {
            throw new Exception("You can't add an abstract component or interface");
        }
    }

    public Component? GetComponent(Type type)
    {
        foreach (Component component in GetComponents())
        {
            if (type.IsInstanceOfType(component))
            {
                return component;
            }
        }

        return null;
    }

    public T? GetComponent<T>() where T : Component
    {
        foreach (Component component in GetComponents())
        {
            if (component is T tComponent)
            {
                return tComponent;
            }
        }

        return null;
    }

    public bool TryGetComponent(Type type, [NotNullWhen(true)] out Component? component)
    {
        component = GetComponent(type);

        return component is not null;
    }

    public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : Component
    {
        component = GetComponent<T>();

        return component is not null;
    }

    public Component GetRequiredComponent(Type type)
    {
        if (TryGetComponent(type, out Component? component))
        {
            return component;
        }

        throw new ComponentNotFoundException(type.Name);
    }

    public T GetRequiredComponent<T>() where T : Component
    {
        if (TryGetComponent(out T? component))
        {
            return component;
        }

        throw new ComponentNotFoundException(typeof(T).Name);
    }

    public void SendMessage(string methodName, Type[] parametersTypes, object?[]? parameters)
    {
        foreach (Component component in GetComponents())
        {
            Invoker.TryInvokeMethod(component, methodName, parametersTypes, parameters);
        }
    }

    public void SendMessage(string methodName)
    {
        SendMessage(methodName, [], null);
    }

    public GameObject Clone()
    {
        var gameObject = new GameObject(Name);
        Transform.TryCopyTo(gameObject.Transform);

        foreach (Component component in GetComponents())
        {
            component.TryCopyTo(gameObject.AddComponent(component.GetType()));
        }

        return gameObject;
    }

    public ReadOnlySpan<Component> GetComponents()
    {
        return _componentsList.AsSpan();
    }
}