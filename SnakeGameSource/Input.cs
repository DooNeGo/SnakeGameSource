using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace SnakeGameSource
{
    internal class Input
    {
#if ANDROID
        GestureSample _gesture = default;
#else
        private Keys _pressedKey = new();
        private Vector2 _lastDirection = Vector2.Zero;
#endif

        public void Update()
        {

#if ANDROID
            if (!TouchPanel.IsGestureAvailable)
                return;

            _gesture = TouchPanel.ReadGesture();
#else

            Keys[] keys = Keyboard.GetState().GetPressedKeys();

            for (var i = 0; i < keys.Length; i++)
            {
                if (keys[i] == Keys.Up
                    || keys[i] == Keys.Down
                    || keys[i] == Keys.Left
                    || keys[i] == Keys.Right)
                    _pressedKey = keys[i];
            }
#endif
        }

        public Vector2 ReadMovement()
        {
#if ANDROID
            if (System.MathF.Abs(_gesture.Delta.X) > System.MathF.Abs(_gesture.Delta.Y))
            {
                return _gesture.Delta.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
            }
            else
            {
                return _gesture.Delta.Y > 0 ? Vector2.UnitY : -Vector2.UnitY;
            }

#else
            _lastDirection = _pressedKey switch
            {
                Keys.Up => -Vector2.UnitY,
                Keys.Down => Vector2.UnitY,
                Keys.Left => -Vector2.UnitX,
                Keys.Right => Vector2.UnitX,
                _ => _lastDirection
            };

            return _lastDirection;
#endif
        }
    }
}
