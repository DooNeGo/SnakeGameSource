namespace SnakeGameSource.GameEngine.Components
{
    public abstract class Component
    {
        public GameObject? Parent { get; init; }

        public T GetComponent<T>() where T : Component
        {
            return this is GameObject gameObject ? gameObject.GetComponent<T>() : Parent!.GetComponent<T>();
        }

        public T AddComponent<T>() where T : Component, new()
        {
            return this is GameObject gameObject ? gameObject.AddComponent<T>() : Parent!.AddComponent<T>();
        }
    }
}