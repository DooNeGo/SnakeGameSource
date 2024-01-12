using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public abstract class Collider : Component
{
    public abstract float GetDistanceToEdge(Vector2 position);
}