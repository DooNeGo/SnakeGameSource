namespace SnakeGameSource.GameEngine.Abstractions;

public interface IScene
{
    public void Add(params IEnumerable<GameObject>[] compositeObjects);

    public void Remove(params IEnumerable<GameObject>[] compositeObjects);

    public void Update(TimeSpan delta);

    public IEnumerable<GameObject> GetGameObjects();
}