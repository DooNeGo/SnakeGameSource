﻿using System.Collections;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

public class Scene : IEnumerable<GameObject>
{
    private const string UpdateMethodName = "Update";

    private static readonly Type[]    InputType  = [typeof(TimeSpan)];
    private static readonly object?[] InputDelta = new object?[1];

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

    private static void InvokeUpdateMethod(GameObject gameObject)
    {
        IReadOnlyList<Component> components = gameObject.GetComponents();

        for (var j = 0; j < components.Count; j++)
        {
            MethodInvoker.TryInvokeMethod(components[j], UpdateMethodName, InputType, InputDelta);
        }
    }

    private void InvokeUpdateMethods(TimeSpan delta)
    {
        var tasks = new Task[_gameObjects.Count];
        InputDelta[0] = delta;
        
        for (var i = 0; i < _gameObjects.Count; i++)
        {
            int index = i;

            tasks[i] = Task.Run(() =>
            {
                InvokeUpdateMethod(_gameObjects[index]);
            });
        }
        
        Task.WaitAll(tasks);
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