using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SnakeGameSource.GameEngine.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace SnakeGameSource.GameEngine;

public class Input(Grid grid) : IInput
{
    private GestureSample _gesture;
    private Keys _pressedKey;

    public float Sensitivity { get; set; } = 0.1f;

    public void Update()
    {
        UpdateGesture();
        UpdateKey();
    }

    public event Action<GestureSample>? Gesture;

    public event Action<Keys>? KeyDown;

    private void UpdateKey()
    {
        if (Keyboard.GetState().GetPressedKeyCount() > 0)
        {
            Keys[] keys = Keyboard.GetState().GetPressedKeys();

            foreach (Keys key in keys)
            {
                KeyDown?.Invoke(key);

                if (key is Keys.Up or Keys.Down or Keys.Left or Keys.Right)
                {
                    _pressedKey = key;
                }
            }
        }
        else
        {
            _pressedKey = default(Keys);
        }
    }

    private void UpdateGesture()
    {
        if (TouchPanel.IsGestureAvailable)
        {
            _gesture = TouchPanel.ReadGesture();
            Gesture?.Invoke(_gesture);
        }
        else
        {
            _gesture = new GestureSample();
        }
    }

    public Vector2? GetMoveDirection()
    {
        Vector2? moveDirection = null;

        if (_pressedKey is not default(Keys))
        {
            moveDirection = _pressedKey switch
            {
                Keys.Up => -Vector2.UnitY,
                Keys.Down => Vector2.UnitY,
                Keys.Left => -Vector2.UnitX,
                Keys.Right => Vector2.UnitX,
                _ => null
            };
        }
        else if (!_gesture.Equals(new GestureSample()))
        {
            Vector2 delta = _gesture.Delta / grid.CellSize.ToVector2() * Sensitivity;

            if (float.Abs(delta.X) <= 1
             && float.Abs(delta.Y) <= 1)
            {
                moveDirection = delta;
            }
            else
            {
                moveDirection = Vector2.Normalize(delta);
            }

        }

        return moveDirection;
    }

    public bool TryGetMoveDirection([NotNullWhen(true)] out Vector2? moveDirection)
    {
        moveDirection = GetMoveDirection();

        return moveDirection is not null;
    }
}