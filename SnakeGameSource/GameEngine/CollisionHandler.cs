using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;
using System.Reflection;

namespace SnakeGameSource.GameEngine
{
    internal class CollisionHandler(Scene scene)
    {
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
            for (int i = 0; i < _gameObjects.Count - 1; i++)
            {
                Collider collider1 = _gameObjects[i].GetComponent<Collider>();
                Transform transform1 = _gameObjects[i].GetComponent<Transform>();

                for (int j = i + 1; j < _gameObjects.Count; j++)
                {
                    Collider collider2 = _gameObjects[j].GetComponent<Collider>();
                    Transform transform2 = _gameObjects[j].GetComponent<Transform>();

                    float distanceToEdge1 = collider1.GetDistanceToEdge(transform2.Position);
                    float distanceToEdge2 = collider2.GetDistanceToEdge(transform1.Position);
                    float distanceBeetween = Vector2.Distance(transform1.Position, transform2.Position);

                    if (distanceToEdge1 + distanceToEdge2 >= distanceBeetween)
                    {
                        TryInvokeCollision(i, j);
                        TryInvokeCollision(j, i);
                    }
                }
            }
        }

        private void TryInvokeCollision(int i, int j)
        {
            foreach (Component component in _gameObjects[i].GetComponents())
            {
                Type type = component.GetType();
                foreach (MethodInfo method in type.GetRuntimeMethods())
                {
                    if (method.Name is "OnCollisionEnter")
                    {
                        method.Invoke(component, [_gameObjects[j]]);
                    }
                }
            }
        }
    }
}