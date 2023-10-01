using Microsoft.Xna.Framework;

namespace SnakeGameSource
{
    internal static class PointExtension
    {
        public static Point Divide(this Point point, int value)
        {
            return new Point(point.X / value, point.Y / value);
        }

        public static Point Divide(this Point point, Point value)
        {
            return new Point(point.X / value.X, point.Y / value.Y);
        }

        public static Point Add(this Point point, int value)
        {
            return new Point(point.X + value, point.Y + value);
        }
    }
}
