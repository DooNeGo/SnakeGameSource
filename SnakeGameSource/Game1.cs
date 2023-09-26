using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SnakeGameSource.Controllers;
using SnakeGameSource.Model;
using static SnakeGameSource.Config;

namespace SnakeGameSource
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Grid _grid;
        private Snake _snake;
        private FoodController _foodController;
        private Scene _mainScene;
        private CollisionHandler _collisionHandler;
        private PhysicsMovement _snakeMovement;
        private KeyboardInput _input;
        private Drawer _drawer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //_graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            _grid = new Grid(Window.ClientBounds.Size, new Point(50, 50));
            _snake = new Snake(new Vector2(3, 4), SnakeHeadColor, SnakeBodyColor, SnakeSpeed, _grid);
            _foodController = new FoodController(_grid);
            _mainScene = new Scene(_snake, _foodController);
            _collisionHandler = new CollisionHandler(_mainScene);
            _snakeMovement = new PhysicsMovement(_snake, SnakeSlewingTime);
            _input = new KeyboardInput();
            _drawer = new Drawer(_mainScene, Content, _grid);

            _grid.ActiveScene = _mainScene;
            _snake.Die += Exit;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _drawer.LoadContent();
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
            _drawer.Draw(_spriteBatch);
            base.Draw(gameTime);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        protected override void UnloadContent()
        {
            _drawer.UnloadContent();

            base.UnloadContent();
        }
    }
}