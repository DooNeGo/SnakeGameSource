using Microsoft.Xna.Framework;

namespace SnakeGameSource.Components.Colliders
{
    internal class CircleCollider : Collider
    {
        public override float GetDistanceToEdge(Vector2 position)
        {
            return GetComponent<Transform>().Scale / 2;
        }
    }
}