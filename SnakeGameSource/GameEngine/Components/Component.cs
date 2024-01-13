namespace SnakeGameSource.GameEngine.Components;

public abstract class Component
{
    public GameObject? Parent { get; init; }

    public T? GetComponent<T>() where T : Component
    {
        return Parent!.GetComponent<T>();
    }

    public T AddComponent<T>() where T : Component, new()
    {
        return Parent!.AddComponent<T>();
    }

    public abstract bool TryCopyTo(Component component);
}