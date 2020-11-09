using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aristarete.Inputting
{
    public static class Keyboard
    {
        private static readonly byte[] DefinedKeyCodes;

        private static readonly byte[] KeyState = new byte[256];
        private static readonly List<Keys> Keys = new List<Keys>(10);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        private static readonly Predicate<Keys> IsKeyReleasedPredicate = key => IsKeyReleased((byte)key);

        static Keyboard()
        {
            var definedKeys = (int[])Enum.GetValues(typeof(Keys));
            var keyCodes = new List<byte>(Math.Min(definedKeys.Length, 255));
            foreach (var key in definedKeys)
            {
                var keyCode = key;
                if (keyCode >= 1 && keyCode <= 255)
                    keyCodes.Add((byte)keyCode);
            }
            DefinedKeyCodes = keyCodes.ToArray();
        }

        public static KeyboardState GetState()
        {
            if (GetKeyboardState(KeyState))
            {
                Keys.RemoveAll(IsKeyReleasedPredicate);

                foreach (var keyCode in DefinedKeyCodes)
                {
                    if (IsKeyReleased(keyCode))
                        continue;
                    var key = (Keys)keyCode;
                    if (!Keys.Contains(key))
                        Keys.Add(key);
                }
            }

            return new KeyboardState(Keys, Console.CapsLock, Console.NumberLock);
        }

        private static bool IsKeyReleased(byte keyCode)
        {
            return (KeyState[keyCode] & 0x80) == 0;
        }
    }
}