namespace SnakeGameSource.GameEngine.Components;

public abstract class Component
{
    public GameObject? Parent { get; set; }

    private GameObject GetBaseGameObject()
    {
        return this as GameObject ?? Parent!;
    }

    public T GetComponent<T>() where T : Component
    {
        return GetBaseGameObject().GetComponent<T>();
    }

    public T AddComponent<T>() where T : Component, new()
    {
        return GetBaseGameObject().AddComponent<T>();
    }

    public virtual bool TryCopyTo<T>(T component) where T : Component
    {
        return false;
    }
}