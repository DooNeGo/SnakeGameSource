using System.Reflection;

namespace SnakeGameSource.GameEngine;

public static class MethodInvoker
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static readonly Dictionary<string, MethodsCache> MethodsCaches = [];

    private static readonly object Locker1 = new();

    public static void TryInvokeMethod(object obj, string methodName, Type[] paramsTypes, object?[]? parameters)
    {
        Type type = obj.GetType();

        //lock (Locker1)
        {
            MethodsCache cache = MethodsCaches.TryGetValue(methodName, out MethodsCache? value)
                ? value
                : MethodsCaches[methodName] = new MethodsCache();
            
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
    }

    private class MethodsCache
    {
        public Dictionary<Type, MethodInfo> Methods { get; } = [];

        public HashSet<Type> WithoutMethod { get; } = [];
    }
}