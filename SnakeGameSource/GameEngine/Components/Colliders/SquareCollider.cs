using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public class SquareCollider : Collider
{
    private Transform _transform;

    public Vector2 Scale { get; set; } = Vector2.One;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public override float GetDistanceToEdge(Vector2 position)
    {
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
        return false;
    }
}