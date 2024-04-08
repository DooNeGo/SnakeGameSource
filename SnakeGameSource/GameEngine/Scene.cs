using CommunityToolkit.HighPerformance;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

public sealed class Scene : IScene
{
    private readonly List<IEnumerable<GameObject>> _compositeObjects = [];
    private readonly List<GameObject>              _gameObjects      = [];

    public void Add(params IEnumerable<GameObject>[] compositeObjects)
    {
        foreach (IEnumerable<GameObject> compositeObject in compositeObjects.AsSpan())
        {
            _compositeObjects.Add(compositeObject);
        }
    }

    public void Remove(params IEnumerable<GameObject>[] compositeObjects)
    {
        foreach (IEnumerable<GameObject> compositeObject in compositeObjects.AsSpan())
        {
            _compositeObjects.Remove(compositeObject);
        }
    }

    public void Update(TimeSpan delta)
    {
        UpdateGameObjectsList();
        InvokeUpdateMethods(delta);
    }

    public ReadOnlySpan<GameObject> GetGameObjects()
    {
        return _gameObjects.AsSpan();
    }

    private void InvokeUpdateMethods(TimeSpan delta)
    {
        if (_gameObjects.Count <= 100)
        {
            foreach (GameObject gameObject in GetGameObjects())
            {
                foreach (Component component in gameObject.GetComponents())
                {
                    MethodInvoker.Update(component, delta);
                }
            }
        }
        else
        {
            Parallel.ForEach(_gameObjects, gameObject =>
            {
                foreach (Component component in gameObject.GetComponents())
                {
                    MethodInvoker.Update(component, delta);
                }
            });
        }
    }

    private void UpdateGameObjectsList()
    {
        _gameObjects.Clear();

        foreach (IEnumerable<GameObject> compositeObject in _compositeObjects.AsSpan())
        {
            _gameObjects.AddRange(compositeObject);
        }
    }
}