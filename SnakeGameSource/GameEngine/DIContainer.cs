using System.Reflection;

namespace SnakeGameSource.GameEngine;

public class DiContainer
{
    private readonly Dictionary<Type, Type>    _associationSingletonTypes = [];
    private readonly Dictionary<Type, Type>    _associationTransientTypes = [];
    private readonly Dictionary<Type, object?> _singletonTypes            = [];
    private readonly HashSet<Type>             _transientTypes            = [];

    public DiContainer AddTransient<T>() where T : class
    {
        Type type = typeof(T);

        if (type.IsInterface)
        {
            throw new ArgumentException("You can't add an interface as an implementation");
        }

        if (_singletonTypes.ContainsKey(type))
        {
            throw new ArgumentException($"You can't add type {type.Name} as singleton and as transient");
        }

        _transientTypes.Add(type);

        return this;
    }

    public DiContainer AddSingleton<T>() where T : class
    {
        return AddSingletonInternal<T>(null);
    }

    public DiContainer AddSingleton<T>(T instance) where T : class
    {
        return AddSingletonInternal(instance);
    }

    public DiContainer AddTransient<TAssociation, TImplementation>() where TImplementation : class
    {
        if (!_associationTransientTypes.TryAdd(typeof(TAssociation), typeof(TImplementation)))
        {
            throw new Exception($"Association {typeof(TAssociation).Name} has already added");
        }

        return AddTransient<TImplementation>();
    }

    public DiContainer AddSingleton<TAssociation, TImplementation>() where TImplementation : class
    {
        return AddSingletonInternal<TAssociation, TImplementation>(null);
    }

    public DiContainer AddSingleton<TAssociation, TImplementation>(TImplementation instance)
        where TImplementation : class
    {
        return AddSingletonInternal<TAssociation, TImplementation>(instance);
    }

    public DiContainer Build()
    {
        foreach (Type type in _singletonTypes.Keys)
        {
            GetInstance(type);
        }

        foreach (Type type in _transientTypes)
        {
            GetInstance(type);
        }

        return this;
    }

    private DiContainer AddSingletonInternal<T>(T? instance) where T : class
    {
        Type type = typeof(T);

        if (type.IsInterface)
        {
            throw new ArgumentException("You can't add an interface as an implementation");
        }

        if (_transientTypes.Contains(type))
        {
            throw new ArgumentException($"You can't add type {type.Name} as singleton and as transient");
        }

        _singletonTypes.TryAdd(type, instance);

        return this;
    }

    private DiContainer AddSingletonInternal<TAssociation, TImplementation>(TImplementation? instance)
        where TImplementation : class
    {
        if (!_associationSingletonTypes.TryAdd(typeof(TAssociation), typeof(TImplementation)))
        {
            throw new Exception($"Association {typeof(TAssociation).Name} has already added");
        }

        return AddSingletonInternal(instance);
    }

    public object GetInstance(Type type)
    {
        if (_associationTransientTypes.TryGetValue(type, out Type? transientType))
        {
            type = transientType;
        }

        if (_transientTypes.Contains(type))
        {
            return CreateInstance(type);
        }

        if (_associationSingletonTypes.TryGetValue(type, out Type? singletonType))
        {
            type = singletonType;
        }

        if (!_singletonTypes.TryGetValue(type, out object? instance))
        {
            throw new ArgumentException($"You forgot to add {type.Name} in the Container.");
        }

        if (instance is null)
        {
            instance              = CreateInstance(type);
            _singletonTypes[type] = instance;
        }

        return instance;
    }

    public T GetInstance<T>()
    {
        return (T)GetInstance(typeof(T));
    }

    private object CreateInstance(Type type)
    {
        ConstructorInfo[] constructorInfo = type.GetConstructors();
        ParameterInfo[]   parameters      = constructorInfo[0].GetParameters();

        if (parameters.Length <= 0)
        {
            return constructorInfo[0].Invoke(null);
        }

        var objects = new object[parameters.Length];

        for (var i = 0; i < objects.Length; i++)
        {
            Type parameterType = parameters[i].ParameterType;
            objects[i] = GetInstance(parameterType);
        }

        return constructorInfo[0].Invoke(objects);
    }
}