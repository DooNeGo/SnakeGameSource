using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Common.Components.Colliders;

public sealed class SquareCollider : Collider
{
    private Transform? _transform;

    protected override void Awake()
    {
        Guard.IsNotNull(Parent, nameof(Parent));
        _transform = Parent.Transform;
    }

    public override float GetDistanceToEdge(Vector2 position)
    {
        Guard.IsNotNull(_transform, nameof(_transform));

        //
        // Vector2 directionToCollider = Vector2.Normalize(_transform.Position - position).Abs();
        // (Vector2 unitVector, float sideLength) = directionToCollider.X > directionToCollider.Y
        //     ? (Vector2.UnitX, _transform.Scale.X * Scale.X)
        //     : (Vector2.UnitY, _transform.Scale.Y * Scale.Y);
        //
        // float cosBetweenVectors = Vector2.Dot(unitVector, directionToCollider);
        //
        // return cosBetweenVectors is 0 ? sideLength : sideLength / 2 / cosBetweenVectors;

        Vector2 directionToCollider = _transform.Position - position;

        // Используем метрику Махаланобиса для расчета расстояния.
        // Для этого необходимо определить матрицу ковариации.
        // В этом примере используется единичная матрица, что эквивалентно Евклидовому расстоянию.
        Matrix covarianceMatrix = Matrix.Identity;

        // Преобразуем направление в махаланобисово расстояние.
        float mahalanobisDistance = Vector2.Transform(directionToCollider, covarianceMatrix).Length();

        // Возвращаем расстояние, учитывая масштаб.
        return mahalanobisDistance * Scale.Length();
    }
}