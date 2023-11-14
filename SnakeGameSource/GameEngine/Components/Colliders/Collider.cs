using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders
{
    internal abstract class Collider : Component
    {
        public abstract float GetDistanceToEdge(Vector2 position);
    }
}