using Microsoft.Xna.Framework;

namespace SnakeGameSource
{
    internal class SnakeConfig
    {
        public Vector2 StartPosition { get; } = new(3, 4);

        public Color HeadColor { get; } = new(131, 240, 60);

        public Color BodyColor { get; } = new(255, 241, 64);

        public float MoveSpeed { get; } = 4f;

        public float SlewingTime { get; } = 0.5f;
    }
}