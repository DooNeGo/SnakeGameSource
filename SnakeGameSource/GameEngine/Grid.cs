using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

//TODO: Убрать сетку как основной элемент позиции всех объектов в движке

public class Grid
{
    private readonly bool[,] _cells;

    public Grid(GameWindow window, Scene scene)
    {
        ActiveScene = scene;
        CellSize    = new Point(window.ClientBounds.Size.X / 15);
        Size        = window.ClientBounds.Size.Divide(CellSize);
        _cells      = new bool[Size.Y, Size.X];
        Center      = new Vector2(Size.X / 2f - 1, Size.Y / 2f - 1);

        window.ClientSizeChanged += OnClientSizeChanged;
    }

    public Point Size { get; }

    public Point CellSize { get; }

    public Vector2 Center { get; }

    public Scene ActiveScene { get; set; }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        //TODO: Implement
    }

    //TODO: Поменять этот метод на имплементацию в CollisionHandler
    public bool IsPositionOccupied(Vector2 position, float scale)
    {
        if (position.X >= _cells.GetLength(1) ||
            position.Y >= _cells.GetLength(0) ||
            position.X < 0                    ||
            position.Y < 0)
        {
            return false;
        }

        Update();

        var checkSize = (int)MathF.Ceiling(scale);
        Vector2 startPosition = new(position.X - checkSize / 2,
                                    position.Y - checkSize / 2);

        for (var y = (int)startPosition.Y; y < checkSize; y++)
        {
            for (var x = (int)startPosition.X; x < checkSize; x++)
            {
                if (_cells[y, x])
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Update()
    {
        Clear();

        foreach (GameObject gameObject in ActiveScene)
        {
            TryAddToGrid(gameObject);
        }
    }

    private void TryAddToGrid(GameObject gameObject)
    {
        var transform = gameObject.TryGetComponent<Transform>();

        if (transform is null                           ||
            transform.Position.X >= _cells.GetLength(1) ||
            transform.Position.Y >= _cells.GetLength(0) ||
            transform.Position.X < 0                    ||
            transform.Position.Y < 0)
        {
            return;
        }

        var checkSize = (int)MathF.Ceiling(transform.Scale);
        Vector2 startPosition = new(transform.Position.X - checkSize / 2.0f,
                                    transform.Position.Y - checkSize / 2.0f);

        for (var y = (int)startPosition.Y; y < checkSize; y++)
        {
            for (var x = (int)startPosition.X; x < checkSize; x++)
            {
                _cells[y, x] = true;
            }
        }
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

    public Vector2 GetAbsolutePosition(Vector2 relativePosition, float scale)
    {
        Vector2 offset = CellSize.ToVector2() * scale / 2;

        return new Vector2(relativePosition.X * CellSize.X - offset.X,
                           relativePosition.Y * CellSize.Y - offset.Y);
    }

    private void Clear()
    {
        for (var y = 0; y < _cells.GetLength(0); y++)
        {
            for (var x = 0; x < _cells.GetLength(1); x++)
            {
                if (_cells[y, x])
                {
                    _cells[y, x] = false;
                }
            }
        }
    }
}