using Microsoft.Xna.Framework;
using System;

namespace SnakeGameSource.Components.Colliders
{
    internal class SquareCollider : Collider
    {
        public override float GetDistanceToEdge(Vector2 position)
        {
            Vector2 vectorToCollider = Vector2.Normalize(GetComponent<Transform>().Position - position);
            vectorToCollider = new Vector2(MathF.Abs(vectorToCollider.X), MathF.Abs(vectorToCollider.Y));
            Vector2 unitVector = vectorToCollider.X > vectorToCollider.Y ? Vector2.UnitX : Vector2.UnitY;
            float cosBeetweenVectors = Vector2.Dot(unitVector, vectorToCollider);
            return GetComponent<Transform>().Scale / 2 / cosBeetweenVectors;
        }
    }
}