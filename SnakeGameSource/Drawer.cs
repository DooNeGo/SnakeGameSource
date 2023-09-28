using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnakeGameSource.Components;
using SnakeGameSource.Model;
using System;
using System.Collections.Generic;

namespace SnakeGameSource
{
    internal class Drawer
    {
        private readonly Dictionary<TextureName, Texture2D> _textures = new();
        private readonly ContentManager _contentManager;
        private readonly Grid _grid;

        public Drawer(ContentManager content, Grid grid)
        {
            _contentManager = content;
            _grid = grid;
        }

        public Scene? ActiveScene { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ActiveScene is null)
                throw new NullReferenceException(nameof(ActiveScene));

            spriteBatch.Begin();

            foreach (GameObject gameObject in ActiveScene)
            {
                TextureConfig? textureConfig = gameObject.TryGetComponent<TextureConfig>();
                Transform? transform = gameObject.TryGetComponent<Transform>();

                if (textureConfig is not null && transform is not null)
                {
                    Vector2 absolutePosition = _grid.GetAbsolutePosition(transform.Position, transform.Scale);
                    float scale = _grid.CellSize.X * textureConfig.Scale / _textures[textureConfig.Name].Bounds.Size.X;
                    spriteBatch.Draw(_textures[textureConfig.Name], absolutePosition, null, textureConfig.Color, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
                }
            }

            spriteBatch.End();
        }

        public void LoadContent()
        {
            _textures.Add(TextureName.SnakeHead, _contentManager.Load<Texture2D>(TextureName.SnakeHead.ToString()));
            _textures.Add(TextureName.SnakeBody, _contentManager.Load<Texture2D>(TextureName.SnakeBody.ToString()));
            _textures.Add(TextureName.Food, _contentManager.Load<Texture2D>(TextureName.Food.ToString()));
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
