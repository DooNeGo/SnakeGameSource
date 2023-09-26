using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnakeGameSource.Components;
using SnakeGameSource.Model;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGameSource
{
    internal class Drawer
    {
        private readonly Dictionary<TextureName, Texture2D> _textures = new();
        private readonly ContentManager _contentManager;
        private readonly Grid _grid;

        private Scene _scene;

        public Drawer(Scene initialScene, ContentManager contentManager, Grid grid)
        {
            _scene = initialScene;
            _contentManager = contentManager;
            _grid = grid;
        }

        public Scene ActiveScene
        {
            get { return _scene; }
            set
            {
                _scene = value;
                LoadContent();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (GameObject gameObject in _scene)
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
            UnloadContent();

            var textureNames = from gameObject in _scene
                               where gameObject.TryGetComponent<TextureConfig>() is not null
                               select gameObject.GetComponent<TextureConfig>().Name;

            foreach (TextureName textureName in textureNames)
            {
                _textures.TryAdd(textureName, _contentManager.Load<Texture2D>(textureName.ToString()));
            }
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
