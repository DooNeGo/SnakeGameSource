using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace SnakeGameSource.GameEngine;

public class Input
{
    private GestureSample _gesture;
    private Keys          _pressedKey;

    public void Update()
    {
        UpdateGesture();
        UpdateKey();
    }

    public event Action<GestureSample>? Gesture;
    public event Action<Keys>?          KeyDown;

    private void UpdateKey()
    {
        if (Keyboard.GetState().GetPressedKeyCount() > 0)
        {
            Keys[] keys = Keyboard.GetState().GetPressedKeys();

            for (var i = 0; i < keys.Length; i++)
            {
                KeyDown?.Invoke(keys[i]);

                if (keys[i] is Keys.Up 
                               or Keys.Down
                               or Keys.Left
                               or Keys.Right)
                {
                    _pressedKey = keys[i];
                }
            }
        }
        else
        {
            _pressedKey = default;
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
            _gesture = default;
        }
    }

    public Vector2 GetMoveDirection()
    {
        Vector2 moveDirection = Vector2.Zero;

        if (_pressedKey is not default(Keys))
        {
            moveDirection = _pressedKey switch
            {
                Keys.Up    => -Vector2.UnitY,
                Keys.Down  => Vector2.UnitY,
                Keys.Left  => -Vector2.UnitX,
                Keys.Right => Vector2.UnitX,
                _          => Vector2.Zero
            };
        }
        else
        {
            moveDirection = _gesture.GestureType switch
            {
                GestureType.VerticalDrag   => _gesture.Delta.Y > 0 ? Vector2.UnitY : -Vector2.UnitY,
                GestureType.HorizontalDrag => _gesture.Delta.X > 0 ? Vector2.UnitX : -Vector2.UnitX,
                _                          => moveDirection
            };
        }

        return moveDirection;
    }
}