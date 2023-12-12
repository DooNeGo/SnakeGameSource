using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components;

public class Transform : Component
{
    public Vector2 Position { get; set; }

    public Quaternion Rotation { get; set; }

    public float Scale { get; set; }

    public void CopyTo(Transform transform)
    {
        transform.Position = Position;
        transform.Rotation = Rotation;
        transform.Scale    = Scale;
    }
}