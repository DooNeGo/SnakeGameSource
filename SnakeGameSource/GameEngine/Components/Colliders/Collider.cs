using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public abstract class Collider : Component
{
    public Vector2 Scale { get; set; } = Vector2.One;

    public abstract float GetDistanceToEdge(Vector2 position);
}