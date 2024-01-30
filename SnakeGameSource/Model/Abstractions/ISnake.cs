using SnakeGameSource.GameEngine;

namespace SnakeGameSource.Model.Abstractions
{
    internal interface ISnake : IMovable, IEnumerable<GameObject>
    {
        public event Action? Die;

        public int Score { get; }
    }
}