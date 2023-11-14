using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SnakeGameSource.GameEngine
{
    internal class Input
    {
#if ANDROID
        public event Action<GestureSample>? Gesture;

        private GestureSample _gesture = default;
#else
        public event Action<Keys>? KeyDown;

        private Keys _pressedKey = default;
#endif

        public void Update()
        {
#if ANDROID
            _gesture = default;
            if (!TouchPanel.IsGestureAvailable)
                return;

            _gesture = TouchPanel.ReadGesture();
            Gesture?.Invoke(_gesture);
#else
            _pressedKey = default;
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
#endif
        }

        public Vector2 GetMoveDirection()
        {
#if ANDROID
    
            if (_gesture.GestureType == GestureType.VerticalDrag)
            {
                return _gesture.Delta.Y > 0 ? Vector2.UnitY : -Vector2.UnitY;
            }
            else if (_gesture.GestureType == GestureType.HorizontalDrag)
            {
                return _gesture.Delta.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
            }

            return Vector2.Zero;
#else
            return _pressedKey switch
            {
                Keys.Up => -Vector2.UnitY,
                Keys.Down => Vector2.UnitY,
                Keys.Left => -Vector2.UnitX,
                Keys.Right => Vector2.UnitX,
                _ => Vector2.Zero
            };
#endif
        }
    }
}
