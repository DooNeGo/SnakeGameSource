using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Common;
using SnakeGameSource.GameEngine.Common.Components;
using SnakeGameSource.GameEngine.Common.Components.Colliders;
using SnakeGameSource.GameEngine.Extensions;
using SnakeGameSource.Model.Abstractions;
using Vector2 = System.Numerics.Vector2;

namespace SnakeGameSource.Model;

internal sealed class FoodCreator : IFoodCreator
{
    private const int FoodLifeTimeInSeconds = 7;

    private readonly Vector2 _foodScale = new(0.5f);

    public FoodCreator(IGrid grid, ICollisionHandler collisionHandler) =>
        Food = new GameObject()
            .WithTransform(Vector2.One, _foodScale)
            .WithTextureConfig(TextureName.Food, Color.Red)
            .WithCollider<SquareCollider>()
            .WithComponent<FoodEffect>()
            .AddComponentWithSetup<FoodParametersRandom>(parametersRandom =>
            {
                parametersRandom.Grid = grid;
                parametersRandom.CollisionHandler = collisionHandler;
                parametersRandom.FoodLifetime = TimeSpan.FromSeconds(FoodLifeTimeInSeconds);
            });

    public GameObject Food { get; }
}