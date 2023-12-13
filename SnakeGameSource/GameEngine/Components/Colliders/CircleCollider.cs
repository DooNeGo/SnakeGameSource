using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

internal class CircleCollider : Collider
{
    public override float GetDistanceToEdge(Vector2 position)
    {
        return GetComponent<Transform>().Scale.X / 2;
    }
}