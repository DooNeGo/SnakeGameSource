using Microsoft.Xna.Framework;

namespace SnakeGameSource.Components
{
    internal class Transform : Component
    {
        public Vector2 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public float Scale { get; set; }

        public void CopyTo(Transform component)
        {
            component.Position = Position;
            component.Rotation = Rotation;
            component.Scale = Scale;
        }
    }
}