using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.GameEngine;

public class CollisionHandler(IScene scene) : ICollisionHandler
{
    private const string CollisionMethodName = "OnCollisionEnter";

    private static readonly Type[] InputType = [typeof(GameObject)];

    private readonly List<Collider> _sceneColliders = [];

    public void Update()
    {
        UpdateCollidersList();
        CheckCollisions();
    }

    public bool IsCollidingWithAnyCollider(Type colliderType, Vector2 position, Vector2 scale)
    {
        if (!colliderType.IsAssignableTo(typeof(Collider)))
        {
            throw new ArgumentException($"{nameof(colliderType)} must be an instance of 'Collider' class");
        }

        GameObject gameObject = new() { Transform = { Position = position } };

        var collider1 = (Collider)gameObject.AddComponent(colliderType);
        collider1.Scale = scale;

        return _sceneColliders.Any(collider2 => IsCollisionBetween(collider1, collider2));
    }

    public bool IsCollidingWithAnyCollider<T>(Vector2 position, Vector2 scale) where T : Collider, new()
    {
        return IsCollidingWithAnyCollider(typeof(T), position, scale);
    }

    private void UpdateCollidersList()
    {
        _sceneColliders.Clear();

        foreach (GameObject gameObject in scene.GetGameObjects())
        {
            if (gameObject.TryGetComponent(out Collider? collider))
            {
                _sceneColliders.Add(collider);
            }
        }
    }

    private static bool IsCollisionBetween(Collider collider1, Collider collider2)
    {
        Transform transform1 = collider1.Parent!.Transform;
        Transform transform2 = collider2.Parent!.Transform;

        Vector2 position1 = transform1.Position;
        Vector2 position2 = transform2.Position;

        float distanceToEdge1 = collider1.GetDistanceToEdge(position2);
        float distanceToEdge2 = collider2.GetDistanceToEdge(position1);
        float distanceBetween = Vector2.Distance(position1, position2);

        return distanceToEdge1 + distanceToEdge2 >= distanceBetween;
    }
    
    private void CheckCollisions()
    {
        Parallel.For(0, _sceneColliders.Count - 1, i =>
        {
            var temp = new object?[1];

            for (int j = i + 1; j < _sceneColliders.Count; j++)
            {
                Collider collider1 = _sceneColliders[i];
                Collider collider2 = _sceneColliders[j];

                if (!IsCollisionBetween(collider1, collider2))
                {
                    continue;
                }
                
                TryInvokeCollision(collider1.Parent!, collider2.Parent!, temp);
                TryInvokeCollision(collider2.Parent!, collider1.Parent!, temp);
            }
        });
    }

    private static void TryInvokeCollision(GameObject gameObject1, GameObject gameObject2, object?[] temp)
    {
        temp[0] = gameObject2;
        gameObject1.SendMessage(CollisionMethodName, InputType, temp);
    }
}