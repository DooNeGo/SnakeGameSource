using Microsoft.Xna.Framework;

namespace SnakeGameSource
{
    internal class Config
    {
        public const int ScreenHeight = GridCellHeight * 9;
        public const int ScreenWidth = GridCellWidth * 16;

        public const int GridCellHeight = (int)(ConsoleCharWidth * Scale);
        public const int GridCellWidth = (int)(ConsoleCharHeight * Scale);

        public const int ConsoleCharHeight = 6;
        public const int ConsoleCharWidth = 4;

        public const float Scale = 2f;

        public static Color SnakeHeadColor = Color.Yellow;
        public static Color SnakeBodyColor = Color.LightYellow;
        public static Color BackgroundColor = Color.Gold;

        public const float SnakeSpeed = 4f;
        public const float SnakeSlewingTime = 0.5f;

        public const int FramePerSecond = 71;
        public const double FrameDelay = 1000 / FramePerSecond;
    }
}