using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Abstractions;
public interface IGrid
{
    public Point CellSize { get; }

    public Vector2 Center { get; }

    public Point Size { get; }

    public Vector2 GetAbsolutePosition(Vector2 relativePosition);
    public Vector2 Project(Vector2 position);
}