using System.Reflection;

namespace SnakeGameSource.GameEngine;

public class MethodInvoker
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private readonly Dictionary<string, MethodsCache> MethodsCaches = [];

    private readonly object Locker1 = new();

    public void TryInvokeMethod(object obj, string methodName, Type[] paramsTypes, object?[]? parameters)
    {
        Type type = obj.GetType();
        MethodsCache? cache;

        //lock (Locker1)
        {
            if (!MethodsCaches.TryGetValue(methodName, out cache))
            {
                cache = new MethodsCache();
                MethodsCaches.Add(methodName, cache);
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