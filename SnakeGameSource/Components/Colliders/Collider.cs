using Microsoft.Xna.Framework;
using SnakeGameSource.Model;
using System;

namespace SnakeGameSource.Components.Colliders
{
    internal abstract class Collider : Component
    {
        public event Action<GameObject>? CollisionEnter;

        public void InvokeCollision(GameObject gameObject)
        {
            CollisionEnter?.Invoke(gameObject);
        }

        public abstract float GetDistanceToEdge(Vector2 position);
    }
}