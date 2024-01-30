using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public class SquareCollider : Collider
{
    private Transform? _transform;

    private void Awake()
    {
        _transform = Parent!.Transform;
    }

    public override float GetDistanceToEdge(Vector2 position)
    {
        if (_transform is null)
        {
            throw new NullReferenceException(nameof(_transform) + "must be not null");
        }

        Vector2 vectorToCollider = Vector2.Normalize(_transform.Position - position).Abs();
        Vector2 unitVector;
        float   sideLength;

        if (vectorToCollider.X > vectorToCollider.Y)
        {
            unitVector = Vector2.UnitX;
            sideLength = _transform.Scale.X * Scale.X;
        }
        else
        {
            unitVector = Vector2.UnitY;
            sideLength = _transform.Scale.Y * Scale.Y;
        }

        float cosBetweenVectors = Vector2.Dot(unitVector, vectorToCollider);

        return sideLength / 2 / cosBetweenVectors;
    }

    public override bool TryCopyTo(Component component)
    {
        if (component is not SquareCollider collider)
        {
            return false;
        }

        collider.Scale = Scale;

        return true;
    }
}