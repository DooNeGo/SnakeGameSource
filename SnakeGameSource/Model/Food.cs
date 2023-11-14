using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.Model
{
    internal class Food : GameObject
    {
        public Food(Vector2 position, float scale, Color color, int lifeTime)
        {
            LifeTime = lifeTime;

            Transform transform = AddComponent<Transform>();
            transform.Position = position;
            transform.Scale = scale;

            TextureConfig textureConfig = AddComponent<TextureConfig>();
            textureConfig.Color = color;
            textureConfig.Name = TextureName.SnakeBody;

            AddComponent<CircleCollider>();
            AddComponent<CollisionNotifier>();
        }

        public int LifeTime { get; }
    }
}