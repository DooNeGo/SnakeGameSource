using System.Drawing;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Common;
using SnakeGameSource.GameEngine.Common.Components;
using SnakeGameSource.GameEngine.Common.Components.Colliders;

namespace SnakeGameSource.GameEngine;

public sealed class CollisionHandler : ICollisionHandler
{
    private readonly List<Collider> _colliders = [];
    private readonly IScene _scene;
    private readonly Action _synchronouslyCheckCollisions;
    private readonly Action _parallelCheckCollisions;

    public CollisionHandler(IScene scene)
    {
        _scene = scene;
        _synchronouslyCheckCollisions = () => 
        {
            for (var i = 0; i < _colliders.Count - 1; i++)
            {
                CheckCollision(i);
            } 
        };
        _parallelCheckCollisions = () => Parallel.For(0, _colliders.Count - 1, CheckCollision);
    }

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

    public bool IsCollidingWithAnyCollider<T>(Vector2 position, Vector2 scale) where T : Collider, new() =>
        IsCollidingWithAnyCollider(typeof(T), position, scale);

    private void UpdateCollidersList()
    {
        _colliders.Clear();

        foreach (GameObject gameObject in _scene.GameObjects)
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

        if (!bounds1.IntersectsWith(bounds2)) return false;

        Vector2 position1 = collider1.Parent!.Transform.Position;
        Vector2 position2 = collider2.Parent!.Transform.Position;

        float distanceToEdge1 = collider1.GetDistanceToEdge(position2);
        if (distanceToEdge1 is float.NaN) return true;

        float distanceToEdge2 = collider2.GetDistanceToEdge(position1);
        if (distanceToEdge2 is float.NaN) return true;

        float distanceBetween = Vector2.Distance(position1, position2);

        return distanceToEdge1 + distanceToEdge2 >= distanceBetween;
    }
    
    private void CheckCollision(int i)
    {
        ReadOnlySpan<Collider> span = _colliders.AsSpan();

        for (int j = i + 1; j < span.Length; j++)
        {
            (Collider collider1, Collider collider2) = (span[i], span[j]);
            if (!IsCollisionBetween(collider1, collider2)) continue;

            TryInvokeCollision(collider1.Parent!, collider2.Parent!);
            TryInvokeCollision(collider2.Parent!, collider1.Parent!);
        }
    }

    private void CheckCollisions() => GetCollisionChecker().Invoke();

    private Action GetCollisionChecker() => _colliders.Count switch
    {
        <= 1 => () => { },
        <= 100 => _synchronouslyCheckCollisions,
        _ => _parallelCheckCollisions
    };

    private static void TryInvokeCollision(GameObject gameObject1, GameObject gameObject2)
    {
        foreach (Component component in gameObject1.Components)
        {
            MethodInvoker.OnCollisionEnter(component, gameObject2);
        }
    }
}