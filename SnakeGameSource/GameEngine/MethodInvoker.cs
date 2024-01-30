using System.Reflection;

namespace SnakeGameSource.GameEngine;

public class MethodInvoker
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private readonly object _locker1 = new();

    private readonly Dictionary<string, MethodsCache> _methodsCaches = [];

    public void TryInvokeMethod(object obj, string methodName, Type[] paramsTypes, object?[]? parameters)
    {
        Type          type = obj.GetType();
        MethodsCache? cache;

        //lock (Locker1)
        {
            if (!_methodsCaches.TryGetValue(methodName, out cache))
            {
                cache = new MethodsCache();
                _methodsCaches.Add(methodName, cache);
            }
        }

        if (cache.WithoutMethod.Contains(type))
        {
            return;
        }

        if (!cache.Methods.TryGetValue(type, out MethodInfo? method))
        {
            method = type.GetMethod(methodName, Flags, paramsTypes);

            if (method is null)
            {
                cache.WithoutMethod.Add(type);

                return;
            }

            cache.Methods[type] = method;
        }

        method.Invoke(obj, parameters);
    }

    private class MethodsCache
    {
        public Dictionary<Type, MethodInfo> Methods { get; } = [];

        public HashSet<Type> WithoutMethod { get; } = [];
    }
}