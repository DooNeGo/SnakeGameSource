using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SnakeGameSource.Controllers;
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
        private KeyboardInput _input;
        private Drawer _drawer;

        private DIContainer _diContainer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //_graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            _diContainer = new DIContainer();
            _diContainer.AddSingletone<Grid>()
                        .AddSingletone<Snake>()
                        .AddSingletone<FoodController>()
                        .AddSingletone<CollisionHandler>()
                        .AddSingletone<SpriteBatch>()
                        .AddSingletone<SnakeConfig>()
                        .AddSingletone<PhysicsMovement>()
                        .AddSingletone<Drawer>()
                        .AddTransient<Scene>()
                        .AddTransient<KeyboardInput>()
                        .AddSingletone<IMovable, Snake>()
                        .AddSingletone(Content)
                        .AddSingletone(Window)
                        .AddSingletone(GraphicsDevice);

            _diContainer.Build();

            Snake snake = _diContainer.GetInstance<Snake>();
            _foodController = _diContainer.GetInstance<FoodController>();
            _mainScene = _diContainer.GetInstance<Scene>();
            _collisionHandler = _diContainer.GetInstance<CollisionHandler>();
            _snakeMovement = _diContainer.GetInstance<PhysicsMovement>();
            _input = _diContainer.GetInstance<KeyboardInput>();
            _drawer = _diContainer.GetInstance<Drawer>();
            _spriteBatch = _diContainer.GetInstance<SpriteBatch>();

            _mainScene.Add(snake, _foodController);
            SetActiveScene(_mainScene);

            snake.Die += Exit;

            base.Initialize();
        }

        protected override void LoadContent()
        {
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
    }
}