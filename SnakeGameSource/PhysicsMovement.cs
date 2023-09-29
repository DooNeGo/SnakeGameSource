using Microsoft.Xna.Framework;
using System;

namespace SnakeGameSource
{
    internal interface IMovable
    {
        public float MoveSpeed { get; }

        public Vector2 Position { get; }

        public float Scale { get; }

        public float SlewingTime { get; }

        public Vector2 Direction { get; }

        public void MoveTo(Vector2 position);
    }

    internal class PhysicsMovement
    {
        private readonly IMovable _snake;

        private Vector2 _smoothDirection;
        private Vector2 _lastDirection;

        public PhysicsMovement(IMovable snake)
        {
            _snake = snake;
            _smoothDirection = _snake.Direction;
            _lastDirection = _snake.Direction;
        }

        public void Move(Vector2 direction, TimeSpan delta)
        {
            if (direction != Vector2.Zero
                && _lastDirection + direction != Vector2.Zero
                && MathF.Abs(_lastDirection.X - _smoothDirection.X) < 1e-1
                && MathF.Abs(_lastDirection.Y - _smoothDirection.Y) < 1e-1)
                _lastDirection = direction;

            float lastDirectionImpact = (float)delta.TotalSeconds / _snake.SlewingTime * _snake.MoveSpeed / _snake.Scale;
            _smoothDirection = Vector2.Normalize(_smoothDirection + _lastDirection * lastDirectionImpact);
            Vector2 offset = (float)delta.TotalSeconds * _snake.MoveSpeed * _smoothDirection;
            _snake.MoveTo(_snake.Position + offset);
        }
    }
}