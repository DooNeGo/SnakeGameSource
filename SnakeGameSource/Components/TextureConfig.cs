using Microsoft.Xna.Framework;
using System;

namespace SnakeGameSource.Components
{
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
            component.Name = Name;
            component.Color = Color;
        }

        public static bool operator ==(TextureConfig left, TextureConfig right)
        {
            if (left.Color == right.Color
                && left.Name == right.Name
                && left.Scale == right.Scale)
                return true;

            return false;
        }

        public static bool operator !=(TextureConfig left, TextureConfig right)
        {
            if (left == right)
                return false;

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is TextureConfig textureConfig
                   && textureConfig == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Color, Scale);
        }
    }
}
