using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.GameEngine.Abstractions;

public interface ICollisionHandler
{
    public bool IsCollidingWithAnyCollider<T>(Vector2 position, Vector2 scale) where T : Collider, new();
    public bool IsCollidingWithAnyCollider(Type colliderType, Vector2 position, Vector2 scale);
    public void Update();
}