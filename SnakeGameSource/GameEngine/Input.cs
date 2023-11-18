2using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace SnakeGameSource.GameEngine
{
    public class Input
    {
        public event Action<GestureSample>? Gesture;
        public event Action<Keys>? KeyDown;

        private GestureSample _gesture = default;
        private Keys _pressedKey = default;

        public void Update()
        {
            UpdateGesture();
            UpdateKey();
        }

        private void UpdateKey()
        {
            if (Keyboard.GetState().GetPressedKeyCount() > 0)
            {
                Keys[] keys = Keyboard.GetState().GetPressedKeys();

                for (int i = 0; i < keys.Length; i++)
                {
                    KeyDown?.Invoke(keys[i]);

                    if (keys[i] is Keys.Up
                        or Keys.Down
                        or Keys.Left
                        or Keys.Right)
                        _pressedKey = keys[i];
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
                    Keys.Up => -Vector2.UnitY,
                    Keys.Down => Vector2.UnitY,
                    Keys.Left => -Vector2.UnitX,
                    Keys.Right => Vector2.UnitX,
                    _ => Vector2.Zero
                };
            }
            else
            {
                if (_gesture.GestureType is GestureType.VerticalDrag)
                {
                    moveDirection = _gesture.Delta.Y > 0
                        ? Vector2.UnitY
                        : -Vector2.UnitY;
                }
                else if (_gesture.GestureType is GestureType.HorizontalDrag)
                {
                    moveDirection = _gesture.Delta.X > 0
                        ? Vector2.UnitX
                        : -Vector2.UnitX;
                }
            }

            return moveDirection;
        }
    }
}
