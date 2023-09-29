using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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
        private Input _input;
        private SpriteDrawer _drawer;

        private DIContainer _diContainer;

        private float timeRatio = 0;

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
                        .AddSingleton<SnakeConfig>()
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (timeRatio == 0 && (TouchPanel.IsGestureAvailable
                || Keyboard.GetState().GetPressedKeyCount() > 0))
                timeRatio = 1;

            _input.Update();
            _snakeMovement.Move(_input.ReadMovement(), gameTime.ElapsedGameTime * timeRatio);
            _foodController.Update(gameTime.ElapsedGameTime * timeRatio);
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