using Microsoft.Xna.Framework;

namespace SnakeGameSource;

public static class Vector2Extension
{
    public static Vector2 Add(this Vector2 vector, float value) => new(vector.X + value, vector.Y + value);

    public static Vector2 Abs(this Vector2 vector) => new(float.Abs(vector.X), float.Abs(vector.Y));

    public static Vector2 Rotate(this Vector2 vector, float angle)
    {
        float radians = angle * MathF.PI / 180f;
        
        return new Vector2(vector.X * MathF.Cos(radians) - vector.Y * MathF.Sin(radians),
                           vector.X * MathF.Sin(radians) + vector.Y * MathF.Cos(radians));
    }
}