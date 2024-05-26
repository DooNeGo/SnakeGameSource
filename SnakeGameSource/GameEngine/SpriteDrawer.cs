using System.Collections.Frozen;
using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Common;
using SnakeGameSource.GameEngine.Common.Components;

namespace SnakeGameSource.GameEngine;

internal sealed class SpriteDrawer(ContentManager content, SpriteBatch spriteBatch, IGrid grid, IScene scene)
    : ISpriteDrawer
{
    private FrozenDictionary<TextureName, Texture2D>? _textures;

    public void Draw()
    {
        Guard.IsNotNull(_textures);

        spriteBatch.Begin();

        foreach (GameObject gameObject in scene.GameObjects)
        {
            if (!gameObject.TryGetComponent(out TextureConfig? textureConfig)) continue;

            Transform transform = gameObject.Transform;

            Vector2 absolutePosition = grid.GetAbsolutePosition(transform.Position);
            Vector2 scale = grid.CellSize.ToVector2()
                          * textureConfig.Scale
                          * transform.Scale
                          / _textures[textureConfig.Name].Bounds.Size.ToVector2();
            var spriteCenter = _textures[textureConfig.Name].Bounds.Center.ToVector2();

            spriteBatch.Draw(_textures[textureConfig.Name], absolutePosition, null, textureConfig.Color,
                             transform.Rotation.Z, spriteCenter, scale, SpriteEffects.None, 1);
        }

        spriteBatch.End();
    }

    public void LoadContent()
    {
        var           textures = new Dictionary<TextureName, Texture2D>();
        TextureName[] names    = Enum.GetValues<TextureName>();

        foreach (TextureName name in names.AsSpan())
        {
            var texture = content.Load<Texture2D>(name.ToString());
            textures.Add(name, texture);
        }

        _textures = textures.ToFrozenDictionary();
    }

    public void UnloadContent()
    {
        Guard.IsNotNull(_textures);

        foreach (Texture2D texture in _textures.Values.AsSpan())
        {
            texture.Dispose();
        }
    }
}