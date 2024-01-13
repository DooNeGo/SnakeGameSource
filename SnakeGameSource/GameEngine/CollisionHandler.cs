using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.GameEngine;

internal class CollisionHandler(Scene scene) : ICollisionHandler
{
    private const string CollisionMethodName = "OnCollisionEnter";

    private readonly List<GameObject> _gameObjects = [];

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
            MethodInvoker.TryInvokeMethod(component, CollisionMethodName, [typeof(GameObject)],
                                          [_gameObjects[gameObjectIndex]]);
        }
    }
}