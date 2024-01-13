using Microsoft.Xna.Framework;

namespace SnakeGameSource;

public static class Vector2Extension
{
    public static Vector2 Add(this Vector2 vector, float value)
    {
        vector.X += value;
        vector.Y += value;

        return vector;
    }

    public static Vector2 Abs(this Vector2 vector)
    {
        vector.X = float.Abs(vector.X);
        vector.Y = float.Abs(vector.Y);

        return vector;
    }

    public static Vector2 Rotate(this Vector2 vector, float angle)
    {
        Matrix matrix = Matrix.CreateRotationZ(angle * MathF.PI / 180f);
        vector = Vector2.Transform(vector, matrix);

        return vector;
    }
}