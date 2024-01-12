using Microsoft.Xna.Framework;

namespace SnakeGameSource.GameEngine.Components;

internal enum TextureName
{
    SnakeHead,
    SnakeBody,
    Food
}

internal class TextureConfig : Component
{
    private Vector2    _scale = Vector2.One;
    private Transform? _transform;
    
    public TextureName Name { get; set; }

    public Color Color { get; set; }

    public Vector2 Scale
    {
        get => _transform!.Scale * _scale;
        set => _scale = value;
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public override bool TryCopyTo(Component component)
    {
        if (component is not TextureConfig textureConfig)
        {
            return false;
        }

        textureConfig.Name  = Name;
        textureConfig.Color = Color;

        return true;
    }

    public static bool operator ==(TextureConfig left, TextureConfig right)
    {
        return left.Color == right.Color && left.Name == right.Name && left.Scale == right.Scale;
    }

    public static bool operator !=(TextureConfig left, TextureConfig right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return obj is TextureConfig textureConfig && textureConfig == this;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Color, Scale);
    }
}