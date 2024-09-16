using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine.Common;
using SnakeGameSource.GameEngine.Common.Components;
using SnakeGameSource.GameEngine.Common.Components.Colliders;

namespace SnakeGameSource.GameEngine.Extensions;

public static class GameObjectExtensions
{
    public static GameObject WithComponentAndSetup(this GameObject gameObject, Type type, Action<Component> setup)
    {
        setup(gameObject.AddComponent(type));
        return gameObject;
    }

    public static GameObject WithComponentAndSetup<T>(this GameObject gameObject, Action<T> setup)
        where T : Component, new()
    {
        setup(gameObject.AddComponent<T>());
        return gameObject;
    }

    public static GameObject WithComponent(this GameObject gameObject, Type componentType)
    {
        gameObject.AddComponent(componentType);
        return gameObject;
    }
    
    public static GameObject WithComponent<T>(this GameObject gameObject) where T : Component, new()
    {
        gameObject.AddComponent<T>();
        return gameObject;
    }

    public static GameObject WithTransform(this GameObject gameObject, Vector2 position, Vector2 scale,
        Quaternion rotation = default)
    {
        gameObject.Transform.Position = position;
        gameObject.Transform.Rotation = rotation;
        gameObject.Transform.Scale = scale;

        return gameObject;
    }

    public static GameObject WithCollider(this GameObject gameObject, Type colliderType, Vector2 scale) =>
        gameObject.WithComponentAndSetup(colliderType, component => ((Collider)component).Scale = scale);

    public static GameObject WithCollider<T>(this GameObject gameObject, Vector2 scale) where T : Collider, new() =>
        gameObject.WithComponentAndSetup<T>(collider => collider.Scale = scale);

    public static GameObject WithTextureConfig(this GameObject gameObject, TextureName name, Color color, Vector2 scale) =>
        gameObject.WithComponentAndSetup<TextureConfig>(texture =>
        {
            texture.Name = name;
            texture.Color = color;
            texture.Scale = scale;
        });

    public static GameObject WithTextureConfig(this GameObject gameObject, TextureName name, Color color) =>
        gameObject.WithTextureConfig(name, color, Vector2.One);
}