using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.GameEngine;

internal class CollisionHandler(Scene scene) : ICollisionHandler
{
    private const string CollisionMethodName = "OnCollisionEnter";

    private readonly List<Collider> _colliders = [];

    public void Update()
    {
        UpdateCollidersList();
        CheckCollisions();
    }

    private void UpdateCollidersList()
    {
        _colliders.Clear();

        foreach (GameObject gameObject in scene)
        {
            if (gameObject.TryGetComponent(out Collider? collider))
            {
                _colliders.Add(collider);
            }
        }
    }

    private static void CheckCollision(Collider collider1, Collider collider2)
    {
        Transform transform1 = collider1.Parent!.Transform;
        Transform transform2 = collider2.Parent!.Transform;

        float distanceToEdge1 = collider1.GetDistanceToEdge(transform2.Position);
        float distanceToEdge2 = collider2.GetDistanceToEdge(transform1.Position);
        float distanceBetween = Vector2.Distance(transform1.Position, transform2.Position);

        if (!(distanceToEdge1 + distanceToEdge2 >= distanceBetween))
        {
            return;
        }

        TryInvokeCollision(collider1.Parent!, collider2.Parent!);
    }

    private void CheckCollisions()
    {
        var tasks = new Task[_colliders.Count - 1];

        for (var i = 0; i < _colliders.Count - 1; i++)
        {
            int index = i;

            tasks[i] = Task.Run(() =>
            {
                for (int j = index + 1; j < _colliders.Count; j++)
                {
                    CheckCollision(_colliders[index], _colliders[j]);
                }
            });
        }

        Task.WaitAll(tasks);
    }

    private static void TryInvokeCollision(GameObject gameObject1, GameObject gameObject2)
    {
        foreach (Component component in gameObject1.GetComponents())
        {
            MethodInvoker.TryInvokeMethod(component, CollisionMethodName, [typeof(GameObject)], [gameObject2]);
        }

        foreach (Component component in gameObject2.GetComponents())
        {
            MethodInvoker.TryInvokeMethod(component, CollisionMethodName, [typeof(GameObject)], [gameObject1]);
        }
    }
}