using SnakeGameSource.GameEngine.Common;

namespace SnakeGameSource.GameEngine.Abstractions;

public interface IScene
{
    public void Add(params ReadOnlySpan<IEnumerable<GameObject>> compositeObjects);

    public void Remove(params ReadOnlySpan<IEnumerable<GameObject>> compositeObjects);

    public void Update(TimeSpan delta);

    public ReadOnlySpan<GameObject> GameObjects { get; }
}