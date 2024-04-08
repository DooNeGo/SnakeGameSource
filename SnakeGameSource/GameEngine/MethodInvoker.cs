using CommunityToolkit.Diagnostics;
using SnakeGameSource.GameEngine.Components;
using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SnakeGameSource.GameEngine;

public sealed class MethodInvoker
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    //private readonly Dictionary<string, MethodsCache>      _methodsCaches = new();
    private readonly FrozenDictionary<Type, MethodsCache> _methodsCaches;

    public MethodInvoker()
    {
        Dictionary<Type, MethodsCache> methodsCaches = [];
        IEnumerable<Type> heirs = AppDomain.CurrentDomain.GetAssemblies()
                                           .SelectMany(a => a.GetTypes())
                                           .Where(t => t.IsSubclassOf(typeof(Component)));

        foreach (Type heir in heirs)
        {
            Dictionary<string, MethodInfo> methodsCache = [];

            foreach (MethodInfo method in heir.GetMethods(Flags).AsSpan())
            {
                //Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                methodsCache.Add(method.Name, method);
            }

            MethodsCache cache = new(methodsCache.ToFrozenDictionary());
            methodsCaches.Add(heir, cache);
        }

        _methodsCaches = methodsCaches.ToFrozenDictionary();
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Awake")]
    internal static extern void Awake(Component component);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Update")]
    internal static extern void Update(Component component, TimeSpan time);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "OnCollisionEnter")]
    internal static extern void OnCollisionEnter(Component component, GameObject gameObject);

    public void TryInvokeMethod(Component component, string methodName, Type[] paramsTypes, object?[]? parameters)
    {
        Type type = component.GetType();

        if (!_methodsCaches.TryGetValue(type, out MethodsCache cache))
        {
            ThrowHelper.ThrowMissingMethodException(methodName);
        }

        if (cache.Methods.TryGetValue(methodName, out MethodInfo? method))
        {
            method.Invoke(component, parameters);
        }
    }

    private readonly struct MethodsCache(FrozenDictionary<string, MethodInfo> methods)
    {
        public FrozenDictionary<string, MethodInfo> Methods { get; } = methods;
    }
}