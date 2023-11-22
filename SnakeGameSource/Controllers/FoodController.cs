using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.Controllers
{
    internal class FoodController
    {
        private const float FoodScale = 1f;

        private readonly Grid _grid;

        public FoodController(Grid grid)
        {
            _grid = grid;
            Food = new GameObject();

            Transform transform = Food.AddComponent<Transform>();
            transform.Position = _grid.Center;
            transform.Scale = FoodScale;

            TextureConfig textureConfig = Food.AddComponent<TextureConfig>();
            textureConfig.Name = TextureName.Food;
            textureConfig.Color = Color.Red;

            FoodParametersRandom random = Food.AddComponent<FoodParametersRandom>();
            random.FoodLifetime = 7;
            random.Grid = _grid;

            Food.AddComponent<CircleCollider>();
            Food.AddComponent<Effect>();
        }

        public GameObject Food { get; }
    }
}