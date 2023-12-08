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
    public TextureName Name { get; set; }

    public Color Color { get; set; }

    public float Scale => GetComponent<Transform>().Scale;

    public void CopyTo(TextureConfig component)
    {
        component.Name  = Name;
        component.Color = Color;
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