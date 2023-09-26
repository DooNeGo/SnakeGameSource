using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.Components.Colliders;

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
            textureConfig.Name = TextureName.Food;

            AddComponent<BoxCollider>();
        }

        public int LifeTime { get; }
    }
}