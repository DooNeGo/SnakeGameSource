using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.Components.Colliders;
using System;
using System.Linq;

namespace SnakeGameSource.Model
{
    internal class Grid
    {
        //private const float Scale = 1.0f;

        private readonly bool[,] _cells;
        //private readonly GameObject[,] _gameObjects;

        public Grid(GameWindow window)
        {
            CellSize = new Point(window.ClientBounds.Size.X / 15);
            Size = window.ClientBounds.Size.Divide(CellSize).Add(2);
            _cells = new bool[Size.Y, Size.X];
            Center = new Vector2((Size.X / 2f) - 1, (Size.Y / 2f) - 1);
            //_gameObjects = new GameObject[Size.Height, Size.Width];
            //InitializeGameObjects();
        }

        public Point Size { get; }

        public Point CellSize { get; }

        public Vector2 Center { get; }

        public Scene? ActiveScene { get; set; }

        public bool IsPositionOccupied(Vector2 position, float scale)
        {
            if (position.X >= _cells.GetLength(1)
                || position.Y >= _cells.GetLength(0)
                || position.X < 0
                || position.Y < 0)
                return false;

            Update();

            int checkSize = (int)MathF.Ceiling(scale);
            Vector2 startPosition = new(position.X - (checkSize / 2),
                                        position.Y - (checkSize / 2));

            for (int y = (int)startPosition.Y; y < checkSize; y++)
            {
                for (int x = (int)startPosition.X; x < checkSize; x++)
                {
                    if (_cells[y, x] is true)
                        return true;
                }
            }

            return false;
        }

        private void Update()
        {
            Clear();

            if (ActiveScene is null)
                throw new NullReferenceException(nameof(ActiveScene));

            foreach (GameObject gameObject in ActiveScene)
            {
                TryAddToGrid(gameObject);
            }
        }

        public Vector2 Project(Vector2 position)
        {
            Vector2 projection = new(position.X % Size.X, position.Y % Size.Y);

            if (projection.X < 0)
                projection.X += Size.X - 1;

            if (projection.Y < 0)
                projection.Y += Size.Y - 1;

            return projection;
        }

        public Vector2 GetTheClosestProjectionOnTheEdge(Vector2 position)
        {
            Vector2 projectionToUpperEdge = position with { Y = 0 };
            Vector2 projectionToLeftEdge = position with { X = 0 };
            Vector2 projectionToRightEdge = position with { X = Size.X - 1 };
            Vector2 projectionToBottomEdge = position with { Y = Size.Y - 1 };

            float distanceToUpperEdge = Vector2.Distance(position, projectionToUpperEdge);
            float distanceToLeftEdge = Vector2.Distance(position, projectionToLeftEdge);
            float distanceToRightEdge = Size.X - distanceToLeftEdge;
            float distanceToBottomEdge = Size.Y - distanceToUpperEdge;

            ValueTuple<float, Vector2>[] distancesWithProjections =
            {
                new(distanceToUpperEdge, projectionToUpperEdge),
                new(distanceToLeftEdge, projectionToLeftEdge),
                new(distanceToRightEdge, projectionToRightEdge),
                new(distanceToBottomEdge, projectionToBottomEdge),
            };

            return distancesWithProjections.Min().Item2;
        }

        public Vector2 GetAbsolutePosition(Vector2 relativePosition, float scale)
        {
            Vector2 offset = new Vector2(CellSize.X * (scale - 1),
                                 CellSize.Y * (scale - 1)) / 2;

            return new(((relativePosition.X - 1) * CellSize.X) - offset.X,
                       ((relativePosition.Y - 1) * CellSize.Y) - offset.Y);
        }

        private void TryAddToGrid(GameObject gameObject)
        {
            Transform? transform = gameObject.TryGetComponent<Transform>();
            Collider? collider = gameObject.TryGetComponent<Collider>();

            if (transform is null
                || collider is null
                || transform.Position.X >= _cells.GetLength(1)
                || transform.Position.Y >= _cells.GetLength(0)
                || transform.Position.X < 0
                || transform.Position.Y < 0)
                return;

            _cells[(int)transform.Position.Y, (int)transform.Position.X] = true;
        }

        private void Clear()
        {
            for (int y = 0; y < _cells.GetLength(0); y++)
            {
                for (int x = 0; x < _cells.GetLength(1); x++)
                {
                    if (_cells[y, x] is true)
                        _cells[y, x] = false;
                }
            }
        }

        //private void InitializeGameObjects()
        //{
        //    for (var y = 0; y < Size.Height; y++)
        //    {
        //        for (var x = 0; x < Size.Width; x++)
        //        {
        //            _gameObjects[y, x] = new GameObject();

        //            Transform transform = _gameObjects[y, x].AddComponent<Transform>();
        //            transform.Position = new Vector2(x, y);
        //            transform.Scale = Scale;

        //            TextureConfig textureConfig = _gameObjects[y, x].AddComponent<TextureConfig>();
        //            textureConfig.Name = TextureName.Grid;
        //            textureConfig.Color = ConsoleColor.White;
        //        }
        //    }
        //}

        //public IEnumerator<GameObject> GetEnumerator()
        //{
        //    foreach (GameObject gameObject in _gameObjects)
        //    {
        //        yield return gameObject;
        //    }
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return _gameObjects.GetEnumerator();
        //}
    }
}