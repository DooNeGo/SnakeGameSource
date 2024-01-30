using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;

namespace SnakeGameSource.GameEngine;

public class Grid : IGrid
{
    public Grid(GameWindow window)
    {
        InitializeGrid(window);

        window.ClientSizeChanged += OnClientSizeChanged;
    }

    public Point Size { get; private set; }

    public Point CellSize { get; private set; }

    public Vector2 Center { get; private set; }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        if (sender is GameWindow window)
        {
            InitializeGrid(window);
        }
    }

    private void InitializeGrid(GameWindow window)
    {
        CellSize = new Point(window.ClientBounds.Size.X / 15);
        Size = window.ClientBounds.Size.Divide(CellSize);
        Center = new Vector2(Size.X / 2f - 1, Size.Y / 2f - 1);
    }

    //TODO: Рассмотреть вариант проекции через матрицу проекций
    public Vector2 Project(Vector2 position)
    {
        Vector2 projection = new(position.X % Size.X, position.Y % Size.Y);

        if (projection.X < 0)
        {
            projection.X += Size.X;
        }

        if (projection.Y < 0)
        {
            projection.Y += Size.Y;
        }

        return projection;
    }

    public Vector2 GetAbsolutePosition(Vector2 relativePosition)
    {
        return new Vector2(relativePosition.X * CellSize.X, relativePosition.Y * CellSize.Y);
    }
}