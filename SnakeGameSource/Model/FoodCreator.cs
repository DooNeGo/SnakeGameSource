using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;
using SnakeGameSource.Model.Abstractions;

namespace SnakeGameSource.Model;

internal class FoodCreator : IFoodCreator
{
    private const int FoodLifeTime = 7;

    private readonly Vector2 _foodScale = new(0.5f);

    public FoodCreator(IGrid grid, ICollisionHandler collisionHandler)
    {
        Food = new GameObject();

        Food.Transform.Scale = _foodScale;

        var textureConfig = Food.AddComponent<TextureConfig>();
        textureConfig.Name  = TextureName.Food;
        textureConfig.Color = Color.Red;

        var random = Food.AddComponent<FoodParametersRandom>();
        random.Grid             = grid;
        random.CollisionHandler = collisionHandler;
        random.FoodLifetime     = TimeSpan.FromSeconds(FoodLifeTime);

        Food.AddComponent<SquareCollider>();
        Food.AddComponent<Effect>();
    }

    public GameObject Food { get; }
}