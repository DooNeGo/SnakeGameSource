using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.GameEngine;
using SnakeGameSource.Model;
using SnakeGameSource.Model.Abstractions;

namespace SnakeGameSource;

public sealed class SnakeGame : Game2D
{
    private PhysicsMovement? _physicsMovement;
    private float            _value;

    public SnakeGame()
    {
        Configuring  += OnConfiguring;
        Initializing += OnInitializing;
        Updating     += OnUpdating;

        Window.AllowUserResizing = true;
    }

    private Color[] BackgroundColors { get; } = [new Color(224, 172, 213), new Color(57, 147, 221)];

    private static void OnConfiguring(DiContainer container) =>
        container
            .AddSingleton<ISnake, Snake>()
            .AddTransient<SnakeConfig>()
            .AddSingleton<IMovable, Snake>()
            .AddSingleton<IFoodCreator, FoodCreator>()
            .AddSingleton<PhysicsMovement>();

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
        _physicsMovement?.Update(gameTime.ElapsedGameTime);
        BackgroundColor =  Color.Lerp(BackgroundColors[0], BackgroundColors[1], MathF.Cos(_value));
        _value          += 0.005f * TimeRatio;
    }

    private void OnKeyDown(Keys key) => GetActionFromKey(key).Invoke();

    private void OnGesture(GestureSample gesture) => GetActionFromGesture(gesture).Invoke();

    private Action GetActionFromKey(Keys key) => key switch
    {
        Keys.Escape => Exit,
        Keys.Space => () => IsStop = !IsStop,
        Keys.OemPlus => IncreaseTimeRatio,
        Keys.OemMinus => DecreaseTimeRatio,
        Keys.Up or Keys.Down or Keys.Left or Keys.Right => ResumeGame,
        _ => () => { }
    };

    private Action GetActionFromGesture(GestureSample gesture) => gesture.GestureType switch
    {
        GestureType.DoubleTap => () => IsStop = !IsStop,
        _ => ResumeGame
    };

    private void ResumeGame()
    {
        if (!IsStop) IsStop = true;
    }

    private void IncreaseTimeRatio() => TimeRatio++;
    
    private void DecreaseTimeRatio() => TimeRatio--;

    private void OnSnakeDie()
    {
        IsStop = true;
    }
}