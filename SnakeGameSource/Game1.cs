using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SnakeGameSource.Components;
using SnakeGameSource.Controllers;
using SnakeGameSource.Model;
using System.Collections.Generic;
using static SnakeGameSource.Config;

namespace SnakeGameSource
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly Dictionary<TextureName, Texture2D> _textures = new();

        private Grid _grid;
        private Snake _snake;
        private FoodController _foodController;
        private Scene _mainScene;
        private CollisionHandler _collisionHandler;
        private PhysicsMovement _snakeMovement;
        private KeyboardInput _input;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _grid = new(Window.ClientBounds.Size, new Point(50, 50));
            _snake = new(new Vector2(3, 4), SnakeHeadColor, SnakeBodyColor, SnakeSpeed, _grid);
            _foodController = new(_grid);
            _mainScene = new Scene(_snake, _foodController);
            _collisionHandler = new(_mainScene);
            _snakeMovement = new(_snake, SnakeSlewingTime);
            _input = new();

            _grid.ActiveScene = _mainScene;
            _snake.Die += Exit;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D headTexture = Content.Load<Texture2D>("SnakeHead");
            Texture2D bodyTexture = Content.Load<Texture2D>("SnakeBody");
            Texture2D foodTexture = Content.Load<Texture2D>("Food");

            _textures.Add(TextureName.Food, foodTexture);
            _textures.Add(TextureName.SnakeHead, headTexture);
            _textures.Add(TextureName.SnakeBody, bodyTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _input.Update();
            _snakeMovement.Move(_input.ReadMovement(), gameTime.ElapsedGameTime);
            _foodController.Update(gameTime.ElapsedGameTime);
            _mainScene.Update();
            _collisionHandler.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            _spriteBatch.Begin();

            foreach (GameObject gameObject in _mainScene)
            {
                TextureConfig? textureConfig = gameObject.TryGetComponent<TextureConfig>();
                Transform? transform = gameObject.TryGetComponent<Transform>();

                if (textureConfig is not null && transform is not null)
                {
                    Vector2 absolutePosition = _grid.GetAbsolutePosition(transform.Position, transform.Scale);
                    float scale = _grid.CellSize.X * textureConfig.Scale / _textures[textureConfig.Name].Bounds.Size.X;
                    _spriteBatch.Draw(_textures[textureConfig.Name], absolutePosition, null, textureConfig.Color, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}