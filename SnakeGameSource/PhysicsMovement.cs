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

        public void MoveTo(Vector2 position);
    }

    internal class PhysicsMovement
    {
        private readonly IMovable _snake;

        private Vector2 _smoothDirection = Vector2.Zero;
        private Vector2 _lastDirection = Vector2.Zero;
        private Vector2 _penultimateDirection = Vector2.UnitX;

        public PhysicsMovement(IMovable snake)
        {
            _snake = snake;
        }

        public void Move(Vector2 direction, TimeSpan delta)
        {
            if (_lastDirection == Vector2.Zero
                && _penultimateDirection + direction != Vector2.Zero
                || _lastDirection != Vector2.Zero
                && MathF.Abs(_lastDirection.X - _smoothDirection.X) < 1e-1
                && MathF.Abs(_lastDirection.Y - _smoothDirection.Y) < 1e-1
                && _lastDirection + direction != Vector2.Zero)
            {
                _penultimateDirection = _lastDirection;
                _lastDirection = direction;
            }

            float lastDirectionImpact = (float)delta.TotalSeconds / _snake.SlewingTime * _snake.MoveSpeed / _snake.Scale;

            if (_lastDirection != Vector2.Zero)
                _smoothDirection = Vector2.Normalize(_smoothDirection + _lastDirection * lastDirectionImpact);
            else
                _smoothDirection = Vector2.Zero;

            Vector2 offset = (float)delta.TotalSeconds * _snake.MoveSpeed * _smoothDirection;
            _snake.MoveTo(_snake.Position + offset);
        }
    }
}