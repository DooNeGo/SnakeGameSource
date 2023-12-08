using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.Controllers;
using SnakeGameSource.GameEngine;
using SnakeGameSource.Model;

namespace SnakeGameSource;

public class SnakeGame : Game2D
{
    private bool            _isStop;
    private PhysicsMovement _physicsMovement;
    private float           _timeRatio;

    public SnakeGame()
    {
        Initializing     += OnInitializing;
        LoadingContent   += OnLoadingContent;
        Updating         += OnUpdating;
        Drawing          += OnDrawing;
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

        _physicsMovement = Container.GetInstance<PhysicsMovement>();
        var snake          = Container.GetInstance<Snake>();
        var foodController = Container.GetInstance<FoodController>();
        Container.GetInstance<Scene>().Add(snake, [foodController.Food]);
        snake.Die += OnSnakeDie;
    }

    private void OnLoadingContent()
    {
    }

    private void OnUpdating(GameTime gameTime)
    {
        if (_isStop)
        {
            return;
        }

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
        {
            Exit();
        }
        else
        {
            _timeRatio = _timeRatio switch
            {
                1 when key is Keys.Space => 0,
                0                        => 1,
                _                        => _timeRatio
            };
        }
    }

    private void OnGesture(GestureSample gesture)
    {
        _timeRatio = _timeRatio switch
        {
            1 when gesture.GestureType is GestureType.DoubleTap => 0,
            0                                                   => 1,
            _                                                   => _timeRatio
        };
    }

    private void OnSnakeDie()
    {
        _isStop = true;
    }
}