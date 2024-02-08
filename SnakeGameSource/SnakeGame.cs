using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.GameEngine;
using SnakeGameSource.Model;
using SnakeGameSource.Model.Abstractions;

namespace SnakeGameSource;

public class SnakeGame : Game2D
{
    private PhysicsMovement _physicsMovement;
    private float           _value;
    
    public SnakeGame()
    {
        Configuring  += OnConfiguring;
        Initializing += OnInitializing;
        Updating     += OnUpdating;

        Window.AllowUserResizing = true;
    }

    private Color[] BackgroundColors { get; set; } = [new Color(224, 172, 213), new Color(57, 147, 221)];

    private void OnConfiguring(DiContainer container)
    {
        container.AddSingleton<ISnake, Snake>()
                 .AddTransient<SnakeConfig>()
                 .AddSingleton<IMovable, Snake>()
                 .AddSingleton<IFoodCreator, FoodCreator>()
                 .AddSingleton<PhysicsMovement>();
    }
    
    private void OnInitializing()
    {
        Input.KeyDown += OnKeyDown;
        Input.Gesture += OnGesture;

        _physicsMovement = Container.GetInstance<PhysicsMovement>();
        var snake       = Container.GetInstance<ISnake>();
        var foodCreator = Container.GetInstance<IFoodCreator>();
        Scene.Add(snake, [foodCreator.Food]);
        snake.Die += OnSnakeDie;

        TimeRatio = 0;
    }

    private void OnUpdating(GameTime gameTime)
    {
        _physicsMovement.Update(gameTime.ElapsedGameTime);
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
        else if (key is Keys.Up or Keys.Down or Keys.Left or Keys.Right)
        {
            TimeRatio = 1;
        }
        else if (key is Keys.OemPlus)
        {
            TimeRatio++;
        }
        else if (key is Keys.OemMinus)
        {
            TimeRatio--;
        }
    }

    private void OnGesture(GestureSample gesture)
    {
        if (gesture.GestureType is GestureType.DoubleTap)
        {
            TimeRatio = TimeRatio is 1 ? 0 : 1;
        }
        else
        {
            TimeRatio = 1;
        }
    }

    private void OnSnakeDie()
    {
        IsStop = true;
    }
}