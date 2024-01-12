using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components;

public class Transform : Component
{
    public Vector2 Position { get; set; }

    public Quaternion Rotation { get; set; }

    public Vector2 Scale { get; set; }

    public override bool TryCopyTo(Component component)
    {
        if (component is not Transform transform)
        {
            return false;
        }

        transform.Position = Position;
        transform.Rotation = Rotation;
        transform.Scale    = Scale;

        return true;
    }
}