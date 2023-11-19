namespace SnakeGameSource.GameEngine.Components
{
    public abstract class Component
    {
        public GameObject? Parent { get; init; }

        private GameObject GetBaseGameObject()
        {
            return this is GameObject gameObject
                ? gameObject
                : Parent!;
        }

        public T GetComponent<T>() where T : Component
        {
            return GetBaseGameObject().GetComponent<T>();
        }

        public T AddComponent<T>() where T : Component, new()
        {
            return GetBaseGameObject().AddComponent<T>();
        }
    }
}