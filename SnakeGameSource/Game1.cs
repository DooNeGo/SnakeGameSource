using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.Controllers;
using SnakeGameSource.GameEngine;
using SnakeGameSource.Model;

namespace SnakeGameSource
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private FoodController _foodController;
        private Scene _mainScene;
        private CollisionHandler _collisionHandler;
        private PhysicsMovement _snakeMovement;
        private Input _input;
        private SpriteDrawer _drawer;

        private DIContainer _diContainer;

        private float _timeRatio = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //_graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            _diContainer = new DIContainer()
                        .AddSingleton<Grid>()
                        .AddSingleton<Snake>()
                        .AddSingleton<FoodController>()
                        .AddSingleton<CollisionHandler>()
                        .AddSingleton<SpriteBatch>()
                        .AddTransient<SnakeConfig>()
                        .AddSingleton<PhysicsMovement>()
                        .AddSingleton<SpriteDrawer>()
                        .AddTransient<Scene>()
                        .AddTransient<Input>()
                        .AddSingleton<IMovable, Snake>()
                        .AddSingleton(Content)
                        .AddSingleton(Window)
                        .AddSingleton(GraphicsDevice)
                        .Build();

            Snake snake = _diContainer.GetInstance<Snake>();
            _foodController = _diContainer.GetInstance<FoodController>();
            _mainScene = _diContainer.GetInstance<Scene>();
            _collisionHandler = _diContainer.GetInstance<CollisionHandler>();
            _snakeMovement = _diContainer.GetInstance<PhysicsMovement>();
            _input = _diContainer.GetInstance<Input>();
            _drawer = _diContainer.GetInstance<SpriteDrawer>();
            _spriteBatch = _diContainer.GetInstance<SpriteBatch>();

            _mainScene.Add(snake, _foodController);
            SetActiveScene(_mainScene);

            snake.Die += Exit;
            _input.KeyDown += OnKeyDown;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _drawer.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            _input.Update();
            _snakeMovement.Move(_input.GetMoveDirection(), gameTime.ElapsedGameTime * _timeRatio);
            _foodController.Update(gameTime.ElapsedGameTime * _timeRatio);
            _mainScene.Update();
            _collisionHandler.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumPurple);
            _drawer.Draw(_spriteBatch);
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _drawer.UnloadContent();
        }

        private void SetActiveScene(Scene scene)
        {
            _diContainer.GetInstance<Grid>().ActiveScene = scene;
            _drawer.ActiveScene = scene;
            _collisionHandler.ActiveScene = scene;
        }

        private void OnKeyDown(Keys key)
        {
            if (key == Keys.Escape)
                Exit();
            else if (_timeRatio == 1 && key == Keys.Space)
                _timeRatio = 0;
            else if (_timeRatio == 0)
                _timeRatio = 1;
        }

        private void OnGesture(GestureSample gesture)
        {
            if (_timeRatio == 1 && gesture.GestureType == GestureType.DoubleTap)
                _timeRatio = 0;
            else if (_timeRatio == 0)
                _timeRatio = 1;
        }
    }
}