using System.Diagnostics.CodeAnalysis;

namespace SnakeGameSource.GameEngine.Components;

public abstract class Component
{
    public GameObject? Parent { get; init; }

    public T? GetComponent<T>() where T : Component
    {
        return Parent!.GetComponent<T>();
    }

    public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : Component
    {
        return Parent!.TryGetComponent(out component);
    }

    public T AddComponent<T>() where T : Component, new()
    {
        return Parent!.AddComponent<T>();
    }

    public abstract bool TryCopyTo(Component component);
}