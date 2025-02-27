using System.Numerics;

namespace SnakeGameSource.GameEngine.Common;

public static class Vector2Extensions
{
    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        float radians = degrees * MathF.PI / 180f;
        var rotationMatrix = Matrix3x2.CreateRotation(radians);
        return Vector2.Transform(vector, rotationMatrix);
    }
    
    public static Vector2 Abs(this Vector2 vector) => Vector2.Abs(vector);
    
    public static Vector2 Normalize(this Vector2 vector) => Vector2.Normalize(vector);
}