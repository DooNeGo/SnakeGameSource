using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components.Colliders;

public class CircleCollider : Collider
{
    private Transform? _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public override float GetDistanceToEdge(Vector2 position)
    {
        if (_transform is null)
        {
            throw new NullReferenceException("There is no Transform component in GameObject");
        }

        return _transform.Scale.X / 2;
    }

    public override bool TryCopyTo(Component component)
    {
        return false;
    }
}