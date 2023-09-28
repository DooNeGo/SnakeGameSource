using System;
using System.Collections.Generic;
using System.Reflection;

namespace SnakeGameSource
{
    internal class DIContainer
    {
        private readonly Dictionary<Type, Type> _singletoneTypes = new();
        private readonly Dictionary<Type, Type> _transientTypes = new();
        private readonly Dictionary<Type, object> _singletoneObjects = new();

        public T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public DIContainer AddTransient<T>() where T : class
        {
            return AddTransient<T, T>();
        }

        public DIContainer AddTransient<D, S>() where S : class
        {
            _transientTypes.TryAdd(typeof(D), typeof(S));
            return this;
        }

        public DIContainer AddSingletone<T>() where T : class
        {
            return AddSingletone<T, T>();
        }

        public DIContainer AddSingletone<D, S>() where S : class
        {
            _singletoneTypes.TryAdd(typeof(D), typeof(S));
            return this;
        }

        public DIContainer AddSingletone<D, S>(S instance) where S : class
        {
            _singletoneTypes.TryAdd(typeof(D), typeof(S));
            _singletoneObjects.TryAdd(typeof(S), instance);
            return this;
        }

        public DIContainer AddSingletone<T>(T instance) where T : class
        {
            return AddSingletone<T, T>(instance);
        }

        public void Build()
        {
            foreach (KeyValuePair<Type, Type> singletoneType in _singletoneTypes)
            {
                GetInstance(singletoneType.Value);
            }
        }

        public object GetInstance(Type type)
        {
            if (_singletoneTypes.TryGetValue(type, out Type? singletoneType))
            {
                if (_singletoneObjects.TryGetValue(singletoneType, out object? value))
                {
                    return value;
                }

                object instance = CreateInstance(singletoneType);
                _singletoneObjects.Add(type, instance);
                return instance;
            }

            if (_transientTypes.TryGetValue(type, out Type? transientType))
            {
                return CreateInstance(transientType);
            }

            throw new NotImplementedException($"{type}");
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
