using SnakeGameSource.Model;

namespace SnakeGameSource.Components
{
    internal abstract class Component
    {
        public GameObject? Parent { get; init; }

        public T GetComponent<T>() where T : Component
        {
            if (this is GameObject gameObject)
            {
                return gameObject.GetComponent<T>();
            }
            else
            {
                return Parent!.GetComponent<T>();
            }
        }

        public T AddComponent<T>() where T : Component, new()
        {
            if (this is GameObject gameObject)
            {
                return gameObject.AddComponent<T>();
            }
            else
            {
                return Parent!.AddComponent<T>();
            }
        }
    }
}