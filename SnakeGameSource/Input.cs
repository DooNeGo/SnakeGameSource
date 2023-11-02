using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace SnakeGameSource
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
            if (System.MathF.Abs(_gesture.Delta.X) > System.MathF.Abs(_gesture.Delta.Y))
            {
                return _gesture.Delta.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
            }
            else
            {
                return _gesture.Delta.Y > 0 ? Vector2.UnitY : -Vector2.UnitY;
            }
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
