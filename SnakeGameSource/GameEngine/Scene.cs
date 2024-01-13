using System.Collections;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

public class Scene : IEnumerable<GameObject>
{
    private const string UpdateMethodName = "Update";

    private readonly List<IEnumerable<GameObject>> _compositeObjects = [];
    private readonly List<GameObject>              _gameObjects      = [];

    public IEnumerator<GameObject> GetEnumerator()
    {
        return _gameObjects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _gameObjects.GetEnumerator();
    }

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

    private void InvokeUpdateMethods(TimeSpan delta)
    {
        foreach (Component component in _gameObjects.SelectMany(gameObject => gameObject.GetComponents()))
        {
            MethodInvoker.TryInvokeMethod(component, UpdateMethodName, [typeof(TimeSpan)], [delta]);
        }
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