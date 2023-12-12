using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource;

internal class SnakeConfig
{
    public Vector2 StartPosition { get; } = new(3, 4);

    public Color HeadColor { get; } = new(247, 146, 86);

    public Color BodyColor { get; } = new(251, 209, 162);

    public Type ColliderType { get; } = typeof(CircleCollider);

    public float MoveSpeed { get; } = 4f;

    public float SlewingSpeed { get; } = 240;

    public Vector2 StartDirection { get; } = Vector2.UnitX;

    public int InitialLength { get; } = 2;
}