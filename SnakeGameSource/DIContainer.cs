using System;
using System.Collections.Generic;
using System.Reflection;

namespace SnakeGameSource
{
    internal class DIContainer
    {
        private readonly Dictionary<Type, Type> _associationTypes = new();
        private readonly Dictionary<Type, object?> _singletonTypes = new();
        private readonly HashSet<Type> _transientTypes = new();

        public T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public DIContainer AddTransient<T>() where T : class
        {
            if (typeof(T).IsInterface)
                throw new ArgumentException("You can't add an interface as an implementation");

            _transientTypes.Add(typeof(T));
            return this;
        }

        public DIContainer AddTransient<D, S>() where S : class
        {
            _associationTypes.Add(typeof(D), typeof(S));
            return AddTransient<S>();
        }

        public DIContainer AddSingleton<T>() where T : class
        {
            if (typeof(T).IsInterface)
                throw new ArgumentException("You can't add an interface as an implementation");

            _singletonTypes.TryAdd(typeof(T), null);
            return this;
        }

        public DIContainer AddSingleton<D, S>() where S : class
        {
            _associationTypes.Add(typeof(D), typeof(S));
            return AddSingleton<S>();
        }

        public DIContainer AddSingleton<T>(T instance) where T : class
        {
            if (typeof(T).IsInterface)
                throw new ArgumentException("You can't add an interface as an implementation");

            _singletonTypes.Add(typeof(T), instance);
            return this;
        }

        public DIContainer AddSingleton<D, S>(S instance) where S : class
        {
            _associationTypes.Add(typeof(D), typeof(S));
            return AddSingleton(instance);
        }

        public void Build()
        {
            foreach (KeyValuePair<Type, object?> singletonType in _singletonTypes)
            {
                GetInstance(singletonType.Key);
            }
        }

        private object GetInstance(Type type)
        {
            if (_associationTypes.TryGetValue(type, out Type? type1))
            {
                type = type1;
            }

            if (_singletonTypes.TryGetValue(type, out object? instance))
            {
                if (instance is not null)
                {
                    return instance;
                }

                instance = CreateInstance(type);
                _singletonTypes[type] = instance;
                return instance;
            }

            if (_transientTypes.Contains(type))
            {
                return CreateInstance(type);
            }

            throw new ArgumentException($"You forgot to add {type.Name} in the Container.");
        }

        private object CreateInstance(Type type)
        {
            ConstructorInfo[] constructorInfo = type.GetConstructors();
            ParameterInfo[] parameters = constructorInfo[0].GetParameters();

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
}
