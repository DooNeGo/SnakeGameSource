using SnakeGameSource.GameEngine.Components;
using System.Reflection;

namespace SnakeGameSource.GameEngine
{
    public class GameObject(string? name = null) : Component
    {
        private readonly List<Component> _components = [];

        public string? Name { get; } = name;

        public new T AddComponent<T>() where T : Component, new()
        {
            if (TryGetComponent<T>() is not null)
                throw new Exception($"{nameof(T)} has already added in components");

            T component = new() { Parent = this };
            _components.Add(component);

            return component;
        }

        public Component AddComponent(Type type)
        {
            if (TryGetComponent(type) is not null)
                throw new Exception($"{type.FullName} has already added in components");

            if (!type.IsSubclassOf(typeof(Component)))
                throw new Exception($"{type.FullName} is not subclass of Component");

            ConstructorInfo[] constructors = type.GetConstructors();
            Component component = (Component)constructors[0].Invoke(null);

            type.GetProperty("Parent")!.SetValue(component, this);
            _components.Add(component);

            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                    _components.RemoveAt(i);
            }
        }

        public new T GetComponent<T>() where T : Component
        {
            T? component = TryGetComponent<T>() 
                ?? throw new Exception($"There is no {nameof(T)} component");
            return component;
        }

        public T? TryGetComponent<T>() where T : Component
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T component)
                    return component;
            }

            return null;
        }

        public object? TryGetComponent(Type type)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].GetType() == type)
                    return _components[i];
            }

            return null;
        }

        public GameObject Clone()
        {
            GameObject gameObject = new(Name);

            foreach (Component component in _components)
            {
                Type type = component.GetType();
                Component cloneComponent = gameObject.AddComponent(type);
                type.GetMethod("CopyTo")?.Invoke(component, [cloneComponent]);
            }

            return gameObject;
        }

        public IEnumerable<Component> GetComponents()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                yield return _components[i];
            }
        }
    }
}