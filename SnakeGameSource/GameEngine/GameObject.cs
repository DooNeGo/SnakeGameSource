using System.Reflection;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

public sealed class GameObject(string? name = null)
{
    private const string TryCopyToMethodName = "TryCopyTo";
    private const string ParentPropertyName  = "Parent";
    private const string AwakeMethodName     = "Awake";

    private static readonly PropertyInfo ParentProperty = typeof(Component).GetProperty(ParentPropertyName)
                                                       ?? throw new NullReferenceException(
                                                              "There is no Parent property in Component class");

    private readonly List<Component> _components = [];

    public string? Name { get; } = name;

    public T AddComponent<T>() where T : Component, new()
    {
        if (TryGetComponent(typeof(T)) is not null)
        {
            throw new Exception($"{typeof(T).FullName} has already added in components");
        }

        T component = new() { Parent = this };
        _components.Add(component);
        MethodInvoker.TryInvokeMethod(component, AwakeMethodName, [], null);

        return component;
    }

    public Component AddComponent(Type type)
    {
        CheckComponentType(type);

        ConstructorInfo? constructor =
            type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, []);

        if (constructor is null)
        {
            throw new NullReferenceException("Component must have parameterless constructor");
        }

        var component = (Component)constructor.Invoke(null);

        ParentProperty.SetValue(component, this);
        _components.Add(component);
        MethodInvoker.TryInvokeMethod(component, AwakeMethodName, [], null);

        return component;
    }

    private void CheckComponentType(Type type)
    {
        if (!type.IsSubclassOf(typeof(Component)))
        {
            throw new Exception($"{type.FullName} is not subclass of Component");
        }

        if (TryGetComponent(type) is not null)
        {
            throw new Exception($"{type.FullName} has already added in components");
        }

        if (type.IsAbstract
         || type.IsInterface)
        {
            throw new Exception("You can't add an abstract component or interface");
        }
    }

    public void RemoveComponent<T>() where T : Component
    {
        for (var i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                _components.RemoveAt(i);
            }
        }
    }

    public T GetComponent<T>() where T : Component
    {
        return TryGetComponent<T>() ?? throw new Exception($"There is no {typeof(T).Name} component");
    }

    public T? TryGetComponent<T>() where T : Component
    {
        return (T?)TryGetComponent(typeof(T));
    }

    public object? TryGetComponent(Type type)
    {
        return _components.FirstOrDefault(type.IsInstanceOfType);
    }

    public GameObject Clone()
    {
        GameObject gameObject = new(Name);

        foreach (Component component in _components)
        {
            Type      type           = component.GetType();
            Component cloneComponent = gameObject.AddComponent(type);
            MethodInvoker.TryInvokeMethod(component, TryCopyToMethodName, [typeof(Component)], [cloneComponent]);
        }

        return gameObject;
    }

    public IEnumerable<Component> GetComponents()
    {
        return _components;
    }
}