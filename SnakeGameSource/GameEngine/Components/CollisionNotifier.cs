namespace SnakeGameSource.GameEngine.Components;

internal sealed class CollisionNotifier : Component
{
    public event Action<GameObject>? CollisionEnter;

    public override bool TryCopyTo(Component component)
    {
        if (component is not CollisionNotifier notifier)
        {
            return false;
        }

        CollisionEnter = (Action<GameObject>?)notifier.CollisionEnter?.Clone();

        return true;
    }

    protected override void OnCollisionEnter(GameObject gameObject)
    {
        CollisionEnter?.Invoke(gameObject);
    }
}