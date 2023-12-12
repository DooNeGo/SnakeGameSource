using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.Model;

internal class FoodCreator
{
    private const float FoodScale    = 0.5f;
    private const int   FoodLifeTime = 7;

    public FoodCreator(Grid grid)
    {
        Food = new GameObject();

        var transform = Food.AddComponent<Transform>();
        transform.Position = grid.Center;
        transform.Scale    = FoodScale;

        var textureConfig = Food.AddComponent<TextureConfig>();
        textureConfig.Name  = TextureName.Food;
        textureConfig.Color = Color.Red;

        var random = Food.AddComponent<FoodParametersRandom>();
        random.FoodLifetime = FoodLifeTime;
        random.Grid         = grid;

        Food.AddComponent<CircleCollider>();
        Food.AddComponent<Effect>();
    }

    public GameObject Food { get; }
}