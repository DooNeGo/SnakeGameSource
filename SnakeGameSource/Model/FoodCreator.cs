using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.Model;

internal class FoodCreator
{
    private const int FoodLifeTime = 7;

    private readonly Vector2 _foodScale = new(0.5f);

    public FoodCreator(Grid grid)
    {
        Food = new GameObject();

        var transform = Food.AddComponent<Transform>();
        transform.Position = grid.Center;
        transform.Scale    = _foodScale;

        var textureConfig = Food.AddComponent<TextureConfig>();
        textureConfig.Name  = TextureName.Food;
        textureConfig.Color = Color.Red;

        Food.AddComponent<FoodParametersRandom>().Grid = grid;
        Food.AddComponent<CircleCollider>();
        Food.AddComponent<Effect>();
    }

    public GameObject Food { get; }
}