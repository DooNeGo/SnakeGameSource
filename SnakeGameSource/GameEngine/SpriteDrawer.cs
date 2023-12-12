using System.Collections.Frozen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine;

internal class SpriteDrawer(ContentManager content, SpriteBatch spriteBatch, Grid grid, Scene scene)
{
    private FrozenDictionary<TextureName, Texture2D>? _textures;

    public void Draw()
    {
        if (_textures is null)
        {
            throw new NullReferenceException("No sprites");
        }

        spriteBatch.Begin();

        foreach (GameObject gameObject in scene)
        {
            var textureConfig = gameObject.TryGetComponent<TextureConfig>();
            var transform     = gameObject.TryGetComponent<Transform>();

            if (textureConfig is null || transform is null)
            {
                continue;
            }

            Vector2 absolutePosition = grid.GetAbsolutePosition(transform.Position, transform.Scale);
            float   scale = grid.CellSize.X * textureConfig.Scale / _textures[textureConfig.Name].Bounds.Size.X;
            Point   spriteCenter = _textures[textureConfig.Name].Bounds.Center;
            spriteBatch.Draw(_textures[textureConfig.Name],
                             absolutePosition,
                             null,
                             textureConfig.Color,
                             transform.Rotation.Z,
                             spriteCenter.ToVector2(),
                             scale,
                             SpriteEffects.None,
                             1);
        }

        spriteBatch.End();
    }

    public void LoadContent()
    {
        var           textures = new Dictionary<TextureName, Texture2D>();
        TextureName[] values   = Enum.GetValues<TextureName>();

        for (var i = 0; i < values.Length; i++)
        {
            var texture = content.Load<Texture2D>(values[i].ToString());
            textures.Add(values[i], texture);
        }

        _textures = textures.ToFrozenDictionary();
    }

    public void UnloadContent()
    {
        if (_textures is null)
        {
            throw new NullReferenceException("No sprites");
        }

        foreach (KeyValuePair<TextureName, Texture2D> keyValue in _textures)
        {
            keyValue.Value.Dispose();
        }
    }
}