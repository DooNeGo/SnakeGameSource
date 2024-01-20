using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;
using SnakeGameSource.Model.Abstractions;

namespace SnakeGameSource.Model;

internal class FoodCreator : IFoodCreator
{
    private const int FoodLifeTime = 7;

    private readonly Vector2 _foodScale = new(0.5f);

    public FoodCreator(Grid grid)
    {
        Food = new GameObject();

        Transform transform = Food.Transform;
        transform.Scale = _foodScale;

        var textureConfig = Food.AddComponent<TextureConfig>();
        textureConfig.Name  = TextureName.Food;
        textureConfig.Color = Color.Red;

        var random = Food.AddComponent<FoodParametersRandom>();
        random.Grid         = grid;
        random.FoodLifetime = TimeSpan.FromSeconds(FoodLifeTime);

        Food.AddComponent<SquareCollider>();
        Food.AddComponent<Effect>();
    }

    public GameObject Food { get; }
}