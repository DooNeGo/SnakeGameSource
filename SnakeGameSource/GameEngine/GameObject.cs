﻿using System.Reflection;using SnakeGameSource.GameEngine.Components;namespace SnakeGameSource.GameEngine;public class GameObject(string? name = null) : Component{    private const string ComponentCopyMethodName = "TryCopyTo";    private const string ParentPropertyName      = "Parent";    private const string AwakeMethodName         = "Awake";    private readonly List<Component> _components = [];    public string? Name { get; } = name;    public new T AddComponent<T>() where T : Component, new()    {        if (TryGetComponent(typeof(T)) is not null)        {            throw new Exception($"{typeof(T).FullName} has already added in components");        }        T component = new() { Parent = this };        _components.Add(component);        TryInvokeAwakeMethod(component);        return component;    }    public Component AddComponent(Type type)    {        IsValidComponentType(type);        ConstructorInfo? constructor = type.GetConstructor(BindingFlags.Instance |                                                           BindingFlags.Public   |                                                           BindingFlags.NonPublic,                                                           []);        if (constructor is null)        {            throw new NullReferenceException("Component must have parameterless constructor");        }        var component = (Component)constructor.Invoke(null);        type.GetProperty(ParentPropertyName)!.SetValue(component, this);        _components.Add(component);        TryInvokeAwakeMethod(component);        return component;    }    private bool IsValidComponentType(Type type)    {        return !type.IsSubclassOf(typeof(Component))            ? throw new Exception($"{type.FullName} is not subclass of Component")            : TryGetComponent(type) is not null                ? throw new Exception($"{type.FullName} has already added in components")                : type.IsAbstract                    ? throw new Exception("You can't add an abstract component")                    : true;    }    private void TryInvokeAwakeMethod(Component component)    {        Type type = component.GetType();        MethodInfo? method = type.GetMethod(AwakeMethodName,                                            BindingFlags.NonPublic |                                            BindingFlags.Public    |                                            BindingFlags.Instance,                                            []);        method?.Invoke(component, []);    }    public void RemoveComponent<T>() where T : Component    {        for (var i = 0; i < _components.Count; i++)        {            if (_components[i] is T)            {                _components.RemoveAt(i);            }        }    }    public new T GetComponent<T>() where T : Component    {        T? component = TryGetComponent<T>() ?? throw new Exception($"There is no {typeof(T).FullName} component");        return component;    }    public T? TryGetComponent<T>() where T : Component    {        for (var i = 0; i < _components.Count; i++)        {            if (_components[i] is T component)            {                return component;            }        }        return null;    }    public object? TryGetComponent(Type type)    {        for (var i = 0; i < _components.Count; i++)        {            if (_components[i].GetType() == type)            {                return _components[i];            }        }        return null;    }    public GameObject Clone()    {        GameObject gameObject = new(Name);        foreach (Component component in _components)        {            Type      type           = component.GetType();            Component cloneComponent = gameObject.AddComponent(type);            type.GetMethod(ComponentCopyMethodName)?.MakeGenericMethod(type).Invoke(component, [cloneComponent]);        }        return gameObject;    }    public IEnumerable<Component> GetComponents()    {        return _components.AsEnumerable();    }}