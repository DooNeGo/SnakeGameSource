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

    private void CheckCollisions()
    {
        for (var i = 0; i < _colliders.Count - 1; i++)
        {
            Collider  collider1  = _colliders[i];
            Transform transform1 = _colliders[i].Parent!.Transform;

            for (int j = i + 1; j < _colliders.Count; j++)
            {
                Collider  collider2  = _colliders[j];
                Transform transform2 = _colliders[j].Parent!.Transform;

                float distanceToEdge1 = collider1.GetDistanceToEdge(transform2.Position);
                float distanceToEdge2 = collider2.GetDistanceToEdge(transform1.Position);
                float distanceBetween = Vector2.Distance(transform1.Position, transform2.Position);

                if (!(distanceToEdge1 + distanceToEdge2 >= distanceBetween))
                {
                    continue;
                }

                TryInvokeCollision(collider1.Parent!, collider2.Parent!);
                TryInvokeCollision(collider2.Parent!, collider1.Parent!);
            }
        }
    }

    private static void TryInvokeCollision(GameObject target, GameObject gameObject)
    {
        foreach (Component component in target.GetComponents())
        {
            MethodInvoker.TryInvokeMethod(component, CollisionMethodName, [typeof(GameObject)],
                                          [gameObject]);
        }
    }
}