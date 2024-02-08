using System.Reflection;
using System.Collections.Frozen;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

public class MethodInvoker
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
            
            foreach (MethodInfo method in heir.GetMethods(Flags))
            {
                //Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                methodsCache.Add(method.Name, method);
            }
            
            MethodsCache cache = new(methodsCache.ToFrozenDictionary());
            methodsCaches.Add(heir, cache);
        }

        _methodsCaches = methodsCaches.ToFrozenDictionary();
    }
    
    public void TryInvokeMethod(Component obj, string methodName, Type[] paramsTypes, object?[]? parameters)
    {
        Type type = obj.GetType();

        if (!_methodsCaches.TryGetValue(type, out MethodsCache cache))
        {
            throw new Exception();
        }

        if (!cache.Methods.TryGetValue(methodName, out MethodInfo? method))
        {
            return;
        }
        
        method.Invoke(obj, parameters);
    }
    
    private readonly struct MethodsCache(FrozenDictionary<string, MethodInfo> methods)
    {
        public FrozenDictionary<string, MethodInfo> Methods { get; } = methods;
    }
}