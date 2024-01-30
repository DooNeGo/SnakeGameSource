using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Abstractions;

namespace SnakeGameSource;

internal interface IMovable
{
    public float MoveSpeed { get; }

    public Vector2 Position { get; }

    public Vector2 Scale { get; }

    public float SlewingSpeed { get; }

    public Vector2 Direction { get; }

    public void MoveTo(Vector2 position);
}

internal class PhysicsMovement
{
    private readonly IInput   _input;
    private readonly IMovable _snake;

    private Vector2 _lastDirection;
    private Vector2 _smoothDirection;

    public PhysicsMovement(IMovable snake, IInput input)
    {
        _input           = input;
        _snake           = snake;
        _smoothDirection = _snake.Direction;
        _lastDirection   = _snake.Direction;
    }

    public void Update(TimeSpan delta)
    {
        if (_input.TryGetMoveDirection(out Vector2? direction)
         && direction != Vector2.Zero)
        {
            _lastDirection = direction.Value;
            float slewingAngle = (float)delta.TotalSeconds
                               * _snake.SlewingSpeed
                               / ((_snake.Scale.X + _snake.Scale.Y) / 2);
            RotateDirection(slewingAngle);
        }

        Move(_smoothDirection, delta);
    }

    private void Move(Vector2 direction, TimeSpan delta)
    {
        Vector2 offset = (float)delta.TotalSeconds * _snake.MoveSpeed * direction;
        _snake.MoveTo(_snake.Position + offset);
    }

    private void RotateDirection(float angle)
    {
        float rotateAngle = GetRotateAngle();
        angle = rotateAngle > 0 ? angle : -angle;

        _smoothDirection = _smoothDirection.Rotate(float.MinMagnitude(angle, rotateAngle));
        _smoothDirection.Normalize();
    }

    private float GetRotateAngle()
    {
        float cos  = Vector2.Dot(_smoothDirection,           _lastDirection);
        float cos1 = Vector2.Dot(_smoothDirection.Rotate(1), _lastDirection);

        cos  = float.Min(1, cos);
        cos  = float.Max(-1, cos);
        cos1 = float.Min(1, cos1);
        cos1 = float.Max(-1, cos1);

        float angelBetween = MathF.Acos(cos);
        float angelPlusOne = MathF.Acos(cos1);

        angelBetween *= 180f / MathF.PI;
        angelPlusOne *= 180f / MathF.PI;

        return angelBetween > angelPlusOne ? angelBetween : -angelBetween;
    }
}