using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.Controllers;

internal class FoodController
{
    private const float FoodScale = 1f;

    public FoodController(Grid grid)
    {
        Food = new GameObject();

        var transform = Food.AddComponent<Transform>();
        transform.Position = grid.Center;
        transform.Scale    = FoodScale;

        var textureConfig = Food.AddComponent<TextureConfig>();
        textureConfig.Name  = TextureName.Food;
        textureConfig.Color = Color.Red;

        var random = Food.AddComponent<FoodParametersRandom>();
        random.FoodLifetime = 7;
        random.Grid         = grid;

        Food.AddComponent<CircleCollider>();
        Food.AddComponent<Effect>();
    }

    public GameObject Food { get; }
}