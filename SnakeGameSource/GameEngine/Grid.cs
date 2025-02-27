using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;
using Point = System.Drawing.Point;
using Vector2 = System.Numerics.Vector2;

namespace SnakeGameSource.GameEngine;

public sealed class Grid : IGrid
{
    public Grid(GameWindow window)
    {
        InitializeGrid(window);

        window.ClientSizeChanged += OnClientSizeChanged;
    }

    public Point Size { get; private set; }

    public Point CellSize { get; private set; }

    public Vector2 Center { get; private set; }

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

    public Vector2 GetAbsolutePosition(Vector2 relativePosition) =>
        new(relativePosition.X * CellSize.X, relativePosition.Y * CellSize.Y);

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        if (sender is GameWindow window) InitializeGrid(window);
    }

    private void InitializeGrid(GameWindow window)
    {
        // TODO: Убрать магическое число
        int xCellSize = window.ClientBounds.Size.X / 15;
        CellSize = new Point(xCellSize, xCellSize);
        Size = window.ClientBounds.Size.ToNumerics().Divide(CellSize);
        Center = new Vector2(Size.X / 2f - 1, Size.Y / 2f - 1);
    }
}

public static class PointExtensions
{
    public static Point ToNumerics(this Microsoft.Xna.Framework.Point point) => new(point.X, point.Y);
    
    public static Vector2 ToVector2(this Point point) => new(point.X, point.Y);
    
    public static Point Divide(this Point point, Point divider) => new(point.X / divider.X, point.Y / divider.Y);
}