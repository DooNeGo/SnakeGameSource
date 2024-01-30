using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

public sealed class GameObject
{
    private const string TryCopyToMethodName = "TryCopyTo";
    private const string ParentPropertyName  = "Parent";
    private const string AwakeMethodName     = "Awake";

    private static readonly MethodInvoker Invoker        = new();
    private static readonly Type[]        InputType      = [typeof(Component)];
    private static readonly PropertyInfo  ParentProperty = typeof(Component).GetProperty(ParentPropertyName)
                                                       ?? throw new NullReferenceException(
                                                              "The 'Component' class doesn't contain a parent property");

    private readonly Dictionary<Type, Component> _componentsDictionary = [];
    private readonly List<Component>             _componentsList       = [];

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
            throw new Exception($"The component {type.Name} have already added in game object");
        }

        T component = new() { Parent = this };
        _componentsDictionary[type] = component;
        _componentsList.Add(component);
        Invoker.TryInvokeMethod(component, AwakeMethodName, [], null);

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
        _componentsDictionary[type] = component;
        _componentsList.Add(component);
        Invoker.TryInvokeMethod(component, AwakeMethodName, [], null);

        return component;
    }

    private void CheckComponentType(Type type)
    {
        if (!type.IsSubclassOf(typeof(Component)))
        {
            throw new Exception($"The component {type.Name} isn't subclass of class 'Component'");
        }

        if (GetComponent(type) is not null)
        {
            throw new Exception($"The component {type.Name} have already added in game object");
        }

        if (type.IsAbstract
         || type.IsInterface)
        {
            throw new Exception("You can't add an abstract component or interface");
        }
    }

    public Component? GetComponent(Type type)
    {
        return _componentsDictionary.TryGetValue(type, out Component? component)
            ? component
            : _componentsDictionary.Values.FirstOrDefault(type.IsInstanceOfType);
    }

    public T? GetComponent<T>() where T : Component
    {
        return (T?)GetComponent(typeof(T));
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

    public GameObject Clone()
    {
        GameObject gameObject = new(Name);
        var        temp       = new object[1];

        foreach (Component component in _componentsDictionary.Values)
        {
            Component componentClone = component.GetType() != typeof(Transform) 
                ? gameObject.AddComponent(component.GetType())
                : gameObject.Transform;
            temp[0] = componentClone;
            Invoker.TryInvokeMethod(component, TryCopyToMethodName, InputType, temp);
        }

        return gameObject;
    }

    public IReadOnlyList<Component> GetComponents()
    {
        return _componentsList;
    }
}