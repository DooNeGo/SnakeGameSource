using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace SnakeGameSource.GameEngine.Abstractions;

public interface IInput
{
    public float Sensitivity { get; set; }

    public event Action<GestureSample>? Gesture;

    public event Action<Keys>? KeyDown;

    public Vector2? GetMoveDirection();

    public bool TryGetMoveDirection([NotNullWhen(true)] out Vector2? moveDirection);

    public void Update();
}