using System.Drawing;
using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public abstract class Collider : Component
{
    public Vector2 Scale { get; set; } = Vector2.One;

    public abstract float GetDistanceToEdge(Vector2 position);

    public virtual RectangleF GetBounds()
    {
        Guard.IsNotNull(Parent);

        Transform transform = Parent.Transform;

        var size  = new SizeF((transform.Scale * Scale).ToNumerics());
        var point = new PointF(transform.Position.ToNumerics() - (size / 2f).ToVector2());

        return new RectangleF(point, size);
    }
}