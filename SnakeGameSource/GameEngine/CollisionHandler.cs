using System.Drawing;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.GameEngine;

public sealed class CollisionHandler(IScene scene) : ICollisionHandler
{
    private readonly List<Collider> _colliders = [];

    public void Update()
    {
        UpdateCollidersList();
        CheckCollisions();
    }

    public bool IsCollidingWithAnyCollider(Type colliderType, Vector2 position, Vector2 scale)
    {
        if (!colliderType.IsSubclassOf(typeof(Collider)))
        {
            throw new ArgumentException($"{nameof(colliderType)} must be an instance of 'Collider' class");
        }

        GameObject gameObject = new() { Transform = { Position = position } };

        var collider1 = (Collider)gameObject.AddComponent(colliderType);
        collider1.Scale = scale;

        foreach (Collider collider in _colliders.AsSpan())
        {
            if (IsCollisionBetween(collider1, collider))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCollidingWithAnyCollider<T>(Vector2 position, Vector2 scale) where T : Collider, new()
    {
        return IsCollidingWithAnyCollider(typeof(T), position, scale);
    }

    private void UpdateCollidersList()
    {
        _colliders.Clear();

        foreach (GameObject gameObject in scene.GetGameObjects())
        {
            if (gameObject.TryGetComponent(out Collider? collider))
            {
                _colliders.Add(collider);
            }
        }
    }

    private static bool IsCollisionBetween(Collider collider1, Collider collider2)
    {
        RectangleF bounds1 = collider1.GetBounds();
        RectangleF bounds2 = collider2.GetBounds();

        if (!bounds1.IntersectsWith(bounds2))
        {
            return false;
        }

        Vector2 position1 = collider1.Parent!.Transform.Position;
        Vector2 position2 = collider2.Parent!.Transform.Position;

        float distanceToEdge1 = collider1.GetDistanceToEdge(position2);
        if (distanceToEdge1 is float.NaN)
        {
            return true;
        }

        float distanceToEdge2 = collider2.GetDistanceToEdge(position1);
        if (distanceToEdge2 is float.NaN)
        {
            return true;
        }

        float distanceBetween = Vector2.Distance(position1, position2);

        return distanceToEdge1 + distanceToEdge2 >= distanceBetween;
    }

    private void CheckCollisions()
    {
        if (_colliders.Count <= 1)
        {
            return;
        }

        var collisionChecker = new CollisionChecker(_colliders);

        if (_colliders.Count <= 100)
        {
            for (var i = 0; i < _colliders.Count - 1; i++)
            {
                collisionChecker.Invoke(i);
            }
        }
        else
        {
            Parallel.For(0, _colliders.Count - 1, i => collisionChecker.Invoke(i));
        }
    }

    private static void TryInvokeCollision(GameObject gameObject1, GameObject gameObject2)
    {
        foreach (Component component in gameObject1.GetComponents())
        {
            MethodInvoker.OnCollisionEnter(component, gameObject2);
        }
    }

    private readonly struct CollisionChecker(List<Collider> colliders) : IAction
    {
        public void Invoke(int i)
        {
            ReadOnlySpan<Collider> span = colliders.AsSpan();

            for (int j = i + 1; j < span.Length; j++)
            {
                Collider collider1 = span[i];
                Collider collider2 = span[j];

                if (!IsCollisionBetween(collider1, collider2))
                {
                    continue;
                }

                TryInvokeCollision(collider1.Parent!, collider2.Parent!);
                TryInvokeCollision(collider2.Parent!, collider1.Parent!);
            }
        }
    }
}