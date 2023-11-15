using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using SnakeGameSource.Controllers;
using SnakeGameSource.GameEngine;
using SnakeGameSource.Model;

namespace SnakeGameSource
{
    public class SnakeGame : Game2D
    {
        private float _timeRatio = 0;
        private bool _isStop = false;

        private PhysicsMovement _physicsMovement;
        private FoodController _foodController;

        public SnakeGame()
        {
            Initializing += OnInitializing;
            LoadingContent += OnLoadingContent;
            Updating += OnUpdating;
            Drawing += OnDrawing;
            UnloadingContent += OnUnloadingContent;
        }

        private void OnInitializing()
        {
            Input.KeyDown += OnKeyDown;
            Input.Gesture += OnGesture;

            Container.AddSingleton<Snake>()
                     .AddTransient<SnakeConfig>()
                     .AddSingleton<IMovable, Snake>()
                     .AddSingleton<FoodController>()
                     .AddSingleton<PhysicsMovement>()
                     .Build();

            _foodController = Container.GetInstance<FoodController>();
            _physicsMovement = Container.GetInstance<PhysicsMovement>();
            Snake snake = Container.GetInstance<Snake>();
            FoodController foodController = Container.GetInstance<FoodController>();
            Container.GetInstance<Scene>().Add(snake, foodController);
            snake.Die += OnSnakeDie;
        }

        private void OnLoadingContent()
        {

        }

        private void OnUpdating(GameTime gameTime)
        {
            if (_isStop)
                return;

            _foodController.Update(gameTime.ElapsedGameTime * _timeRatio);
            _physicsMovement.Move(Input.GetMoveDirection(), gameTime.ElapsedGameTime * _timeRatio);
        }

        private void OnDrawing(GameTime gameTime)
        {

        }

        private void OnUnloadingContent()
        {

        }

        private void OnKeyDown(Keys key)
        {
            if (key == Keys.Escape)
                Exit();
            else if (_timeRatio is 1 && key is Keys.Space)
                _timeRatio = 0;
            else if (_timeRatio is 0)
                _timeRatio = 1;
        }

        private void OnGesture(GestureSample gesture)
        {
            if (_timeRatio is 1 && gesture.GestureType is GestureType.DoubleTap)
                _timeRatio = 0;
            else if (_timeRatio is 0)
                _timeRatio = 1;
        }

        private void OnSnakeDie()
        {
            _isStop = true;
        }
    }
}
