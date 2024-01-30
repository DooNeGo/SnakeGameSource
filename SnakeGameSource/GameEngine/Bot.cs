using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.Model.Abstractions;

namespace SnakeGameSource.GameEngine;

internal class Bot : IInput
{
    private readonly IFoodCreator _foodCreator;
    private readonly Input        _input;
    private readonly IMovable     _movable;

    public Bot(IMovable movable, IFoodCreator foodCreator, IGrid grid)
    {
        _movable     = movable;
        _foodCreator = foodCreator;
        _input       = new Input(grid);

        _input.KeyDown += p => { KeyDown?.Invoke(p); };
    }

    public float Sensitivity { get; set; } = 1f;

    public event Action<GestureSample>? Gesture;

    public event Action<Keys>? KeyDown;

    public Vector2? GetMoveDirection()
    {
        return Vector2.Normalize(_foodCreator.Food.Transform.Position - _movable.Position);
    }

    public bool TryGetMoveDirection([NotNullWhen(true)] out Vector2? moveDirection)
    {
        moveDirection = GetMoveDirection();

        return moveDirection is not null;
    }

    public void Update()
    {
        _input.Update();
    }
}