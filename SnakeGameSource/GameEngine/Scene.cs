using SnakeGameSource.GameEngine.Abstractions;

namespace SnakeGameSource.GameEngine;

public class Scene : IScene
{
    private const string UpdateMethodName = "Update";

    private static readonly Type[]    InputType  = [typeof(TimeSpan)];
    private static readonly object?[] InputDelta = new object?[1];

    private readonly List<IEnumerable<GameObject>> _compositeObjects = [];
    private readonly List<GameObject>              _gameObjects      = [];

    public void Add(params IEnumerable<GameObject>[] compositeObjects)
    {
        _compositeObjects.AddRange(compositeObjects);
    }

    public void Remove(params IEnumerable<GameObject>[] compositeObjects)
    {
        foreach (IEnumerable<GameObject> compositeObject in compositeObjects)
        {
            _compositeObjects.Remove(compositeObject);
        }
    }

    public void Update(TimeSpan delta)
    {
        UpdateGameObjectsList();
        InvokeUpdateMethods(delta);
    }

    public IEnumerable<GameObject> GetGameObjects()
    {
        return _gameObjects;
    }

    private void InvokeUpdateMethods(TimeSpan delta)
    {
        InputDelta[0] = delta;

        Parallel.For(0, _gameObjects.Count, i =>
        {
            _gameObjects[i].SendMessage(UpdateMethodName, InputType, InputDelta);
        });
    }

    private void UpdateGameObjectsList()
    {
        _gameObjects.Clear();

        foreach (IEnumerable<GameObject> compositeObject in _compositeObjects)
        {
            _gameObjects.AddRange(compositeObject);
        }
    }
}