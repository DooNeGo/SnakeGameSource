using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.GameEngine;
using SnakeGameSource.Model;

namespace SnakeGameSource;

public class SnakeGame : Game2D
{
    private PhysicsMovement _physicsMovement;
    private float           _timeRatio;

    public SnakeGame()
    {
        Initializing += OnInitializing;
        Updating     += OnUpdating;
    }

    private void OnInitializing()
    {
        Input.KeyDown += OnKeyDown;
        Input.Gesture += OnGesture;

        Container.AddSingleton<Snake>()
                 .AddTransient<SnakeConfig>()
                 .AddSingleton<IMovable, Snake>()
                 .AddSingleton<FoodCreator>()
                 .AddSingleton<PhysicsMovement>()
                 .Build();

        _physicsMovement = Container.GetInstance<PhysicsMovement>();
        var snake       = Container.GetInstance<Snake>();
        var foodCreator = Container.GetInstance<FoodCreator>();
        Container.GetInstance<Scene>().Add(snake, [foodCreator.Food]);
        snake.Die += OnSnakeDie;
    }

    private void OnUpdating(GameTime gameTime)
    {
        _physicsMovement.Move(Input.GetMoveDirection(), gameTime.ElapsedGameTime * _timeRatio);
    }

    private void OnKeyDown(Keys key)
    {
        if (key is Keys.Escape)
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
        IsStop = true;
    }
}