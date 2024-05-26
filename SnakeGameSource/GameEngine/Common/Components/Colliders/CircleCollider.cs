using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Common.Components.Colliders;

public sealed class CircleCollider : Collider
{
    private Transform? _transform;

    protected override void Awake() => _transform = Parent!.Transform;

    public override float GetDistanceToEdge(Vector2 position)
    {
        Guard.IsNotNull(_transform, nameof(_transform));

        Vector2 vectorToCollider = Vector2.Normalize(_transform.Position - position).Abs();
        float radius = vectorToCollider.X > vectorToCollider.Y
            ? _transform.Scale.X * Scale.X
            : _transform.Scale.Y * Scale.Y;

        return radius / 2;
    }
}