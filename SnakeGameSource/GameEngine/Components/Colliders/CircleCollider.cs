using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public class CircleCollider : Collider
{
    private Transform _transform;

    public Vector2 Scale { get; set; } = Vector2.One;

    private void Awake()
    {
        _transform = Parent!.Transform;
    }

    public override float GetDistanceToEdge(Vector2 position)
    {
        Vector2 vectorToCollider = Vector2.Normalize(_transform.Position - position).Abs();
        float radius = vectorToCollider.X > vectorToCollider.Y
            ? _transform.Scale.X * Scale.X
            : _transform.Scale.Y * Scale.Y;

        return radius / 2;
    }

    public override bool TryCopyTo(Component component)
    {
        if (component is not CircleCollider collider)
        {
            return false;
        }

        collider.Scale = Scale;

        return false;
    }
}