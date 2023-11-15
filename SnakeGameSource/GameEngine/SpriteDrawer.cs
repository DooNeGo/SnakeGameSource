using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.GameEngine
{
    internal class SpriteDrawer
    {
        private readonly Dictionary<TextureName, Texture2D> _textures = new();
        private readonly ContentManager _contentManager;
        private readonly SpriteBatch _spriteBatch;
        private readonly Grid _grid;

        public SpriteDrawer(ContentManager content, SpriteBatch spriteBatch, Grid grid, Scene scene)
        {
            _contentManager = content;
            _spriteBatch = spriteBatch;
            _grid = grid;
            ActiveScene = scene;
        }

        public Scene ActiveScene { get; set; }

        public void Draw()
        {
            _spriteBatch.Begin();

            foreach (GameObject gameObject in ActiveScene)
            {
                TextureConfig? textureConfig = gameObject.TryGetComponent<TextureConfig>();
                Transform? transform = gameObject.TryGetComponent<Transform>();

                if (textureConfig is not null && transform is not null)
                {
                    Vector2 absolutePosition = _grid.GetAbsolutePosition(transform.Position, transform.Scale);
                    float scale = _grid.CellSize.X * textureConfig.Scale / _textures[textureConfig.Name].Bounds.Size.X;
                    Point spriteCenter = _textures[textureConfig.Name].Bounds.Center;
                    _spriteBatch.Draw(_textures[textureConfig.Name],
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

            _spriteBatch.End();
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
