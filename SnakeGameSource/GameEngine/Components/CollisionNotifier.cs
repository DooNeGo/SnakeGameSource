namespace SnakeGameSource.GameEngine.Components;

internal class CollisionNotifier : Component
{
    public event Action<GameObject>? CollisionEnter;

    private void OnCollisionEnter(GameObject gameObject)
    {
        CollisionEnter?.Invoke(gameObject);
    }
}