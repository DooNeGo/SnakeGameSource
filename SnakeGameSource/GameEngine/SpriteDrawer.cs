using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnakeGameSource.GameEngine.Components;
using System.Collections.Frozen;

namespace SnakeGameSource.GameEngine
{
    internal class SpriteDrawer(ContentManager content, SpriteBatch spriteBatch, Grid grid, Scene scene)
    {
        private FrozenDictionary<TextureName, Texture2D> _textures;

        public void Draw()
        {
            spriteBatch.Begin();

            foreach (GameObject gameObject in scene)
            {
                TextureConfig? textureConfig = gameObject.TryGetComponent<TextureConfig>();
                Transform? transform = gameObject.TryGetComponent<Transform>();

                if (textureConfig is not null && transform is not null)
                {
                    Vector2 absolutePosition = grid.GetAbsolutePosition(transform.Position, transform.Scale);
                    float scale = grid.CellSize.X * textureConfig.Scale / _textures[textureConfig.Name].Bounds.Size.X;
                    Point spriteCenter = _textures[textureConfig.Name].Bounds.Center;
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
            }

            spriteBatch.End();
        }

        public void LoadContent()
        {
            Dictionary<TextureName, Texture2D> textures = [];
            TextureName[] values = Enum.GetValues<TextureName>();

            for (int i = 0; i < values.Length; i++)
            {
                Texture2D texture = content.Load<Texture2D>(values[i].ToString());
                textures.Add(values[i], texture);
            }

            _textures = textures.ToFrozenDictionary();
        }

        public void UnloadContent()
        {
            foreach (KeyValuePair<TextureName, Texture2D> keyValue in _textures)
            {
                keyValue.Value.Dispose();
            }
        }
    }
}
