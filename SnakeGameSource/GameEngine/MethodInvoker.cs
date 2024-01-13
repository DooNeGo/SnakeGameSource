using System.Reflection;

namespace SnakeGameSource.GameEngine;

public static class MethodInvoker
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static readonly Dictionary<string, MethodsCache> MethodsCaches = [];

    public static void TryInvokeMethod(object obj, string methodName, Type[] paramsTypes, object?[]? parameters)
    {
        MethodsCache cache = MethodsCaches.TryGetValue(methodName, out MethodsCache? value)
            ? value
            : MethodsCaches[methodName] = new MethodsCache();

        Type type = obj.GetType();

        if (cache.WithoutMethod.Contains(type))
        {
            return;
        }

        MethodInfo? method = cache.Methods.TryGetValue(type, out MethodInfo? info)
            ? info
            : type.GetMethod(methodName, Flags, paramsTypes);

        if (method is null)
        {
            cache.WithoutMethod.Add(type);

            return;
        }

        cache.Methods[type] = method;
        method.Invoke(obj, parameters);
    }

    private class MethodsCache
    {
        public Dictionary<Type, MethodInfo> Methods { get; } = [];

        public HashSet<Type> WithoutMethod { get; } = [];
    }
}