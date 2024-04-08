using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public sealed class CircleCollider : Collider
{
    private Transform? _transform;

    protected override void Awake()
    {
        _transform = Parent!.Transform;
    }

    public override float GetDistanceToEdge(Vector2 position)
    {
        Guard.IsNotNull(_transform, nameof(_transform));

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