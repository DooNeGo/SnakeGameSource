using Microsoft.Xna.Framework;

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
    private readonly IMovable _snake;

    private Vector2 _lastDirection;
    private Vector2 _smoothDirection;

    public PhysicsMovement(IMovable snake)
    {
        _snake           = snake;
        _smoothDirection = _snake.Direction;
        _lastDirection   = _snake.Direction;
    }

    public void Move(Vector2? direction, TimeSpan delta)
    {
        if (direction is not null
         && direction != Vector2.Zero)
        {
            _lastDirection = Vector2.Normalize(direction.Value);
            float slewingAngle = (float)delta.TotalSeconds
                               * _snake.SlewingSpeed
                               / ((_snake.Scale.X + _snake.Scale.Y) / 2);
            RotateDirection(slewingAngle);
        }

        Vector2 offset = (float)delta.TotalSeconds * _snake.MoveSpeed * _smoothDirection;
        _snake.MoveTo(_snake.Position + offset);
    }

    private void RotateDirection(float angle)
    {
        float rotateAngle = GetRotateAngle();
        angle = rotateAngle > 0 ? angle : -angle;

        _smoothDirection = GetRotatedVector(_smoothDirection, float.MinMagnitude(angle, rotateAngle));
        _smoothDirection.Normalize();
    }

    private Vector2 GetRotatedVector(Vector2 vector, float angle)
    {
        return Vector2.Transform(vector, Matrix.CreateRotationZ(angle * MathF.PI / 180f));
    }

    private float GetRotateAngle()
    {
        float angelBetween = MathF.Acos(Vector2.Dot(_smoothDirection, _lastDirection));
        float angelPlusOne = MathF.Acos(Vector2.Dot(GetRotatedVector(_smoothDirection, 1), _lastDirection));

        angelBetween *= 180f / MathF.PI;
        angelPlusOne *= 180f / MathF.PI;

        return angelBetween > angelPlusOne ? angelBetween : -angelBetween;
    }
}