using SnakeGameSource.GameEngine;

namespace SnakeGameSource.Model.Abstractions;

internal interface ISnake : IMovable, IEnumerable<GameObject>
{
    public int Score { get; }

    public event Action? Die;
}