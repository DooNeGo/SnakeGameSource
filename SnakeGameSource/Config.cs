using Microsoft.Xna.Framework;

namespace SnakeGameSource
{
    internal class Config
    {
        public const float Scale = 2f;

        public static Color SnakeHeadColor = new(131, 240, 60);
        public static Color SnakeBodyColor = new(255, 241, 64);
        public static Color BackgroundColor = Color.MediumPurple;

        public const float SnakeSpeed = 4f;
        public const float SnakeSlewingTime = 0.5f;

        public const int FramePerSecond = 60;
        public const double FrameDelay = 1000 / FramePerSecond;
    }
}