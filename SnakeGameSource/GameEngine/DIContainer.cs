using System.Reflection;

namespace SnakeGameSource.GameEngine;

public class DiContainer
{
    private readonly Dictionary<Type, Type>    _associationTypes = [];
    private readonly Dictionary<Type, object?> _singletonTypes   = [];
    private readonly HashSet<Type>             _transientTypes   = [];

    public T GetInstance<T>()
    {
        return (T)GetInstance(typeof(T));
    }

    public DiContainer AddTransient<T>() where T : class
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("You can't add an interface as an implementation");
        }

        _transientTypes.Add(typeof(T));

        return this;
    }

    public DiContainer AddTransient<TAssociation, TImplementation>() where TImplementation : class
    {
        _associationTypes.Add(typeof(TAssociation), typeof(TImplementation));

        return AddTransient<TImplementation>();
    }

    public DiContainer AddSingleton<T>() where T : class
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("You can't add an interface as an implementation");
        }

        _singletonTypes.TryAdd(typeof(T), null);

        return this;
    }

    public DiContainer AddSingleton<TAssociation, TImplementation>() where TImplementation : class
    {
        _associationTypes.Add(typeof(TAssociation), typeof(TImplementation));

        return AddSingleton<TImplementation>();
    }

    public DiContainer AddSingleton<T>(T instance) where T : class
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("You can't add an interface as an implementation");
        }

        _singletonTypes.Add(typeof(T), instance);

        return this;
    }

    public DiContainer AddSingleton<TAssociation, TImplementation>(TImplementation instance)
        where TImplementation : class
    {
        _associationTypes.Add(typeof(TAssociation), typeof(TImplementation));

        return AddSingleton(instance);
    }

    public DiContainer Build()
    {
        foreach (KeyValuePair<Type, object?> singletonType in _singletonTypes)
        {
            GetInstance(singletonType.Key);
        }

        foreach (Type type in _transientTypes)
        {
            GetInstance(type);
        }

        return this;
    }

    private object GetInstance(Type type)
    {
        if (_associationTypes.TryGetValue(type, out Type? type1))
        {
            type = type1;
        }

        if (!_singletonTypes.TryGetValue(type, out object? instance))
        {
            return _transientTypes.Contains(type)
                ? CreateInstance(type)
                : throw new ArgumentException($"You forgot to add {type.Name} in the Container.");
        }

        if (instance is not null)
        {
            return instance;
        }

        instance              = CreateInstance(type);
        _singletonTypes[type] = instance;

        return instance;
    }

    private object CreateInstance(Type type)
    {
        ConstructorInfo[] constructorInfo = type.GetConstructors();
        ParameterInfo[]   parameters      = constructorInfo[0].GetParameters();

        object[]? objects = null;

        if (parameters.Length > 0)
        {
            objects = new object[parameters.Length];

            for (var i = 0; i < objects.Length; i++)
            {
                Type parameterType = parameters[i].ParameterType;
                objects[i] = GetInstance(parameterType);
            }
        }

        return constructorInfo[0].Invoke(objects);
    }
}