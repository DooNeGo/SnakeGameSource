using SnakeGameSource.GameEngine;

namespace SnakeGameSource.Model.Abstractions;

internal interface IFoodCreator
{
    public GameObject Food { get; }
}