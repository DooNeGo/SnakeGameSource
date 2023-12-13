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
}