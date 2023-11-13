using SnakeGameSource.Components;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SnakeGameSource.Model
{
    internal class GameObject : Component
    {
        private readonly List<Component> _components = new();

        public GameObject(string? name = null)
        {
            Name = name;
        }

        public string? Name { get; }

        public new T AddComponent<T>() where T : Component, new()
        {
            T component = new() { Parent = this };
            _components.Add(component);
            return component;
        }

        public Component AddComponent(Type type)
        {
            if (type != typeof(Component)
                && type.BaseType != typeof(Component))
                throw new ArgumentException(null, nameof(type));

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
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T value)
                    return value;
            }

            throw new Exception($"There is no {nameof(T)} component");
        }

        public T? TryGetComponent<T>() where T : Component
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T value)
                    return value;
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
                type.GetMethod("CopyTo")?.Invoke(component, new object?[] { cloneComponent });
            }

            return gameObject;
        }

        public IEnumerator<T> GetComponents<T>() where T : Component
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T component)
                    yield return component;
            }
        }
    }
}