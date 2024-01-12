using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.GameEngine;
using SnakeGameSource.Model;

namespace SnakeGameSource;

public class SnakeGame : Game2D
{
    private PhysicsMovement _physicsMovement;
    private float           _value;

    public SnakeGame()
    {
        Initializing += OnInitializing;
        Updating     += OnUpdating;

        Window.AllowUserResizing = true;
    }

    protected Color[] BackgroundColors { get; set; } = [new Color(224, 172, 213), new Color(57, 147, 221)];

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

        TimeRatio = 0;
    }

    private void OnUpdating(GameTime gameTime)
    {
        _physicsMovement.Move(Input.GetMoveDirection(), gameTime.ElapsedGameTime);
        BackgroundColor =  Color.Lerp(BackgroundColors[0], BackgroundColors[1], MathF.Cos(_value));
        _value          += 0.005f * TimeRatio;
    }

    private void OnKeyDown(Keys key)
    {
        if (key is Keys.Escape)
        {
            Exit();
        }
        else if (key is Keys.Space)
        {
            TimeRatio = TimeRatio is 1 ? 0 : 1;
        }
    }

    private void OnGesture(GestureSample gesture)
    {
        if (gesture.GestureType is GestureType.DoubleTap)
        {
            TimeRatio = TimeRatio is 1 ? 0 : 1;
        }
    }

    private void OnSnakeDie()
    {
        IsStop = true;
    }
}