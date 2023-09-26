using SnakeGameSource.Components;
using System;
using System.Collections.Generic;

namespace SnakeGameSource.Model
{
    internal class GameObject : Component
    {
        private readonly List<Component> _components = new();

        public GameObject() { }

        public GameObject(string name)
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

        public void RemoveComponent<T>()
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

            throw new Exception();
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