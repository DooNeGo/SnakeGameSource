using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Common;
using SnakeGameSource.GameEngine.Common.Components;
using SnakeGameSource.GameEngine.Common.Components.Colliders;

namespace SnakeGameSource.GameEngine.Extensions;

public static class GameObjectExtensions
{
    public static GameObject AddComponentWithSetup(this GameObject gameObject, Type type, Action<Component> setup)
    {
        setup(gameObject.AddComponent(type));
        return gameObject;
    }
    
    public static GameObject AddComponentWithSetup<T>(this GameObject gameObject, Action<T> setup) where T : Component, new()
    {
        setup(gameObject.AddComponent<T>());
        return gameObject;
    }

    public static GameObject WithComponent<T>(this GameObject gameObject) where T : Component, new()
    {
        gameObject.AddComponent<T>();
        return gameObject;
    }
    
    public static GameObject WithTransform(this GameObject gameObject, Vector2 position,
        in Vector2 scale, in Quaternion rotation = default)
    {
        gameObject.Transform.Position = position;
        gameObject.Transform.Rotation = rotation;
        gameObject.Transform.Scale = scale;

        return gameObject;
    }

    public static GameObject WithCollider(this GameObject gameObject, Type colliderType, Vector2 scale) =>
        gameObject.AddComponentWithSetup(colliderType, component => ((Collider)component).Scale = scale);

    public static GameObject WithCollider(this GameObject gameObject, Type colliderType) =>
        gameObject.WithCollider(colliderType, Vector2.One);

    public static GameObject WithCollider<TCollider>(this GameObject gameObject, Vector2 scale) 
        where TCollider : Collider, new() =>
        gameObject.AddComponentWithSetup<TCollider>(collider => collider.Scale = scale);

    public static GameObject WithCollider<TCollider>(this GameObject gameObject)
        where TCollider : Collider, new() =>
        gameObject.WithCollider<TCollider>(Vector2.One);

    public static GameObject WithTextureConfig(this GameObject gameObject, TextureName name, Color color, Vector2 scale) =>
        gameObject.AddComponentWithSetup<TextureConfig>(texture =>
        {
            texture.Name = name;
            texture.Color = color;
            texture.Scale = scale;
        });

    public static GameObject WithTextureConfig(this GameObject gameObject, TextureName name, Color color) =>
        gameObject.WithTextureConfig(name, color, Vector2.One);
}