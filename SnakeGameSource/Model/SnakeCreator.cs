using SnakeGameSource.GameEngine;

namespace SnakeGameSource.Model;

public class SnakeCreator
{
    internal SnakeCreator(SnakeConfig config, Grid grid)
    {
        GameObject snake = new();
    }

    public GameObject Snake { get; }
}