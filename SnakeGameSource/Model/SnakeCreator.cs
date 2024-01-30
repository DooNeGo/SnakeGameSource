using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Abstractions;

namespace SnakeGameSource.Model;

public class SnakeCreator
{
    internal SnakeCreator(SnakeConfig config, IGrid grid)
    {
        GameObject snake = new();
    }

    public GameObject Snake { get; }
}