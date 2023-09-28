using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.Components.Colliders;
using SnakeGameSource.Model;
using System;
using System.Collections.Generic;

namespace SnakeGameSource.Controllers
{
    internal class CollisionHandler
    {
        private readonly List<GameObject> _gameObjects = new();

        public Scene? ActiveScene { get; set; }

        public void Update()
        {
            UpdateGameObjectsList();
            CheckCollisions();
        }

        private void UpdateGameObjectsList()
        {
            if (ActiveScene is null)
                throw new NullReferenceException(nameof(ActiveScene));

            _gameObjects.Clear();

            foreach (GameObject gameObject in ActiveScene)
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
                Collider collider1 = _gameObjects[i].GetComponent<Collider>();
                Transform transform1 = _gameObjects[i].GetComponent<Transform>();

                for (var j = i + 1; j < _gameObjects.Count; j++)
                {
                    Collider collider2 = _gameObjects[j].GetComponent<Collider>();
                    Transform transform2 = _gameObjects[j].GetComponent<Transform>();

                    float distanceToEdge1 = collider1.GetDistanceToEdge(transform2.Position);
                    float distanceToEdge2 = collider2.GetDistanceToEdge(transform1.Position);
                    float distanceBeetween = Vector2.Distance(transform1.Position, transform2.Position);

                    if (distanceBeetween <= distanceToEdge1 + distanceToEdge2)
                    {
                        collider1.InvokeCollision(_gameObjects[j]);
                        collider2.InvokeCollision(_gameObjects[i]);
                    }
                }
            }
        }
    }
}
