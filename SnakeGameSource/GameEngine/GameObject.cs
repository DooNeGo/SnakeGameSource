using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

public sealed class GameObject
{
    private const string TryCopyToMethodName = "TryCopyTo";
    private const string ParentPropertyName  = "Parent";
    private const string AwakeMethodName     = "Awake";

    private static readonly PropertyInfo ParentProperty = typeof(Component).GetProperty(ParentPropertyName)
                                                       ?? throw new NullReferenceException(
                                                              "There is no Parent property in Component class");

    private readonly Dictionary<Type, Component> _components = [];

    public GameObject(string? name = null)
    {
        Name      = name;
        Transform = AddComponent<Transform>();
    }

    public string? Name { get; }

    public Transform Transform { get; }

    public T AddComponent<T>() where T : Component, new()
    {
        Type type = typeof(T);

        if (GetComponent(type) is not null)
        {
            throw new Exception($"{type.FullName} has already added in components");
        }

        T component = new() { Parent = this };
        _components[type] = component;
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
        _components[type] = component;
        MethodInvoker.TryInvokeMethod(component, AwakeMethodName, [], null);

        return component;
    }

    private void CheckComponentType(Type type)
    {
        if (!type.IsSubclassOf(typeof(Component)))
        {
            throw new Exception($"{type.FullName} is not subclass of Component");
        }

        if (GetComponent(type) is not null)
        {
            throw new Exception($"{type.FullName} has already added in components");
        }

        if (type.IsAbstract
         || type.IsInterface)
        {
            throw new Exception("You can't add an abstract component or interface");
        }
    }

    // TODO: Исправить баг с Transform
    public void RemoveComponent<T>() where T : Component
    {
        var component = GetComponent<T>();

        if (component is null)
        {
            return;
        }

        _components.Remove(component.GetType());
    }

    public T? GetComponent<T>() where T : Component
    {
        return (T?)GetComponent(typeof(T));
    }

    public Component? GetComponent(Type type)
    {
        if (_components.TryGetValue(type, out Component? component))
        {
            return component;
        }

        return _components.Values.FirstOrDefault(type.IsInstanceOfType);
    }

    public bool TryGetComponent(Type type, [NotNullWhen(true)] out Component? component)
    {
        component = GetComponent(type);

        return component is not null;
    }

    public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : Component
    {
        bool result = TryGetComponent(typeof(T), out Component? value);
        component = (T?)value;

        return result;
    }

    public GameObject Clone()
    {
        GameObject gameObject = new(Name);
        Dictionary<Type, Component>.ValueCollection values = _components.Values;

        foreach (Component component in _components.Values)
        {
            Component cloneComponent = component.GetType() != typeof(Transform) 
                ? gameObject.AddComponent(component.GetType())
                : gameObject.Transform;
            MethodInvoker.TryInvokeMethod(component, TryCopyToMethodName, [typeof(Component)], [cloneComponent]);
        }

        return gameObject;
    }

    public IEnumerable<Component> GetComponents()
    {
        return _components.Values;
    }
}