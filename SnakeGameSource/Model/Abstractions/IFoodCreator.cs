using SnakeGameSource.GameEngine.Common;

namespace SnakeGameSource.Model.Abstractions;

internal interface IFoodCreator
{
    public GameObject Food { get; }
}