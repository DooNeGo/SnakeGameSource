using System.Reflection;
using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.GameEngine;

internal class CollisionHandler(Scene scene) : ICollisionHandler
{
    private const string CollisionMethodName = "OnCollisionEnter";

    private readonly Dictionary<Type, MethodInfo> _collisionMethods       = [];
    private readonly List<GameObject>             _gameObjects            = [];
    private readonly HashSet<Type>                _withoutCollisionMethod = [];

    public void Update()
    {
        UpdateGameObjectsList();
        CheckCollisions();
    }

    private void UpdateGameObjectsList()
    {
        _gameObjects.Clear();

        foreach (GameObject gameObject in scene)
        {
            if (gameObject.TryGetComponent<Collider>() is not null)
            {
                _gameObjects.Add(gameObject);
            }
        }
    }

    private void CheckCollisions()
    {
        for (var i = 0; i < _gameObjects.Count - 1; i++)
        {
            var collider1  = _gameObjects[i].GetComponent<Collider>();
            var transform1 = _gameObjects[i].GetComponent<Transform>();

            for (int j = i + 1; j < _gameObjects.Count; j++)
            {
                var collider2  = _gameObjects[j].GetComponent<Collider>();
                var transform2 = _gameObjects[j].GetComponent<Transform>();

                float distanceToEdge1 = collider1.GetDistanceToEdge(transform2.Position);
                float distanceToEdge2 = collider2.GetDistanceToEdge(transform1.Position);
                float distanceBetween = Vector2.Distance(transform1.Position, transform2.Position);

                if (!(distanceToEdge1 + distanceToEdge2 >= distanceBetween))
                {
                    continue;
                }

                TryInvokeCollision(i, j);
                TryInvokeCollision(j, i);
            }
        }
    }

    private void TryInvokeCollision(int targetIndex, int gameObjectIndex)
    {
        foreach (Component component in _gameObjects[targetIndex].GetComponents())
        {
            Type type = component.GetType();

            if (_withoutCollisionMethod.Contains(type))
            {
                continue;
            }

            if (!_collisionMethods.TryGetValue(type, out MethodInfo? method))
            {
                method = type.GetMethod(CollisionMethodName,
                                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public,
                                        [typeof(GameObject)]);
                if (method is null)
                {
                    _withoutCollisionMethod.Add(type);

                    continue;
                }

                _collisionMethods.Add(type, method);
            }

            method.Invoke(component, [_gameObjects[gameObjectIndex]]);
        }
    }
}