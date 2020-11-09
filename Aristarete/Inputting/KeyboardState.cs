using System.Collections.Generic;
using System.Windows.Input;

namespace Aristarete.Inputting
{
    public struct KeyboardState
    {
        private const byte CapsLockModifier = 1;
        private const byte NumLockModifier = 2;

        // Used for the common situation where GetPressedKeys will return an empty array
        private static readonly Keys[] Empty = new Keys[0];

        #region Key Data

        // Array of 256 bits:
        private uint _keys0, _keys1, _keys2, _keys3, _keys4, _keys5, _keys6, _keys7;
        private readonly byte _modifiers;

        private bool GetKey(Keys key)
        {
            var mask = (uint)1 << ((int)key & 0x1f);

            uint element = ((int) key >> 5) switch
            {
                0 => _keys0,
                1 => _keys1,
                2 => _keys2,
                3 => _keys3,
                4 => _keys4,
                5 => _keys5,
                6 => _keys6,
                7 => _keys7,
                _ => 0
            };

            return (element & mask) != 0;
        }

        private void SetKey(Keys key)
        {
            var mask = (uint)1 << ((int)key & 0x1f);
            switch ((int)key >> 5)
            {
                case 0: _keys0 |= mask; break;
                case 1: _keys1 |= mask; break;
                case 2: _keys2 |= mask; break;
                case 3: _keys3 |= mask; break;
                case 4: _keys4 |= mask; break;
                case 5: _keys5 |= mask; break;
                case 6: _keys6 |= mask; break;
                case 7: _keys7 |= mask; break;
            }
        }

        private void ClearKey(Keys key)
        {
            var mask = (uint)1 << ((int)key & 0x1f);
            switch ((int)key >> 5)
            {
                case 0: _keys0 &= ~mask; break;
                case 1: _keys1 &= ~mask; break;
                case 2: _keys2 &= ~mask; break;
                case 3: _keys3 &= ~mask; break;
                case 4: _keys4 &= ~mask; break;
                case 5: _keys5 &= ~mask; break;
                case 6: _keys6 &= ~mask; break;
                case 7: _keys7 &= ~mask; break;
            }
        }

        private void ClearAllKeys()
        {
            _keys0 = 0;
            _keys1 = 0;
            _keys2 = 0;
            _keys3 = 0;
            _keys4 = 0;
            _keys5 = 0;
            _keys6 = 0;
            _keys7 = 0;
        }

        #endregion


        #region XNA Interface

        internal KeyboardState(List<Keys>? keys, bool capsLock = false, bool numLock = false) : this()
        {
            _keys0 = 0;
            _keys1 = 0;
            _keys2 = 0;
            _keys3 = 0;
            _keys4 = 0;
            _keys5 = 0;
            _keys6 = 0;
            _keys7 = 0;
            _modifiers = (byte)(0 | (capsLock ? CapsLockModifier : 0) | (numLock ? NumLockModifier : 0));

            if (keys != null)
                foreach (var k in keys)
                    SetKey(k);
        }
        
        /// <summary>
        /// Gets the current state of the Caps Lock key.
        /// </summary>
        public bool CapsLock => (_modifiers & CapsLockModifier) > 0;

        /// <summary>
        /// Gets the current state of the Num Lock key.
        /// </summary>
        public bool NumLock => (_modifiers & NumLockModifier) > 0;

        /// <summary>
        /// Returns the state of a specified key.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>The state of the key.</returns>
        public KeyStates this[Keys key] => GetKey(key) ? KeyStates.Down : KeyStates.None;

        /// <summary>
        /// Gets whether given key is currently being pressed.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>true if the key is pressed; false otherwise.</returns>
        public bool IsKeyDown(Keys key)
        {
            return GetKey(key);
        }

        /// <summary>
        /// Gets whether given key is currently being not pressed.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>true if the key is not pressed; false otherwise.</returns>
        public bool IsKeyUp(Keys key)
        {
            return !GetKey(key);
        }

        #endregion


        #region GetPressedKeys()

        /// <summary>
        /// Returns the number of pressed keys in this <see cref="KeyboardState"/>.
        /// </summary>
        /// <returns>An integer representing the number of keys currently pressed in this <see cref="KeyboardState"/>.</returns>
        public int GetPressedKeyCount()
        {
            var count = CountBits(_keys0) + CountBits(_keys1) + CountBits(_keys2) + CountBits(_keys3)
                        + CountBits(_keys4) + CountBits(_keys5) + CountBits(_keys6) + CountBits(_keys7);
            return (int)count;
        }

        private static uint CountBits(uint v)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            v -= (v >> 1) & 0x55555555;                    // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);     // temp
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
        }

        private static int AddKeysToArray(uint keys, int offset, Keys[] pressedKeys, int index)
        {
            for (var i = 0; i < 32; i++)
            {
                if ((keys & (1 << i)) != 0)
                    pressedKeys[index++] = (Keys)(offset + i);
            }
            return index;
        }

        /// <summary>
        /// Returns an array of values holding keys that are currently being pressed.
        /// </summary>
        /// <returns>The keys that are currently being pressed.</returns>
        public Keys[] GetPressedKeys()
        {
            var count = CountBits(_keys0) + CountBits(_keys1) + CountBits(_keys2) + CountBits(_keys3)
                        + CountBits(_keys4) + CountBits(_keys5) + CountBits(_keys6) + CountBits(_keys7);
            if (count == 0)
                return Empty;
            Keys[] keys = new Keys[count];

            var index = 0;
            if (_keys0 != 0) index = AddKeysToArray(_keys0, 0 * 32, keys, index);
            if (_keys1 != 0) index = AddKeysToArray(_keys1, 1 * 32, keys, index);
            if (_keys2 != 0) index = AddKeysToArray(_keys2, 2 * 32, keys, index);
            if (_keys3 != 0) index = AddKeysToArray(_keys3, 3 * 32, keys, index);
            if (_keys4 != 0) index = AddKeysToArray(_keys4, 4 * 32, keys, index);
            if (_keys5 != 0) index = AddKeysToArray(_keys5, 5 * 32, keys, index);
            if (_keys6 != 0) index = AddKeysToArray(_keys6, 6 * 32, keys, index);
            if (_keys7 != 0) AddKeysToArray(_keys7, 7 * 32, keys, index);

            return keys;
        }
        
        #endregion


        #region Object and Equality

        /// <summary>
        /// Gets the hash code for <see cref="KeyboardState"/> instance.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return (int)(_keys0 ^ _keys1 ^ _keys2 ^ _keys3 ^ _keys4 ^ _keys5 ^ _keys6 ^ _keys7);
        }

        /// <summary>
        /// Compares whether two <see cref="KeyboardState"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="KeyboardState"/> instance to the left of the equality operator.</param>
        /// <param name="b"><see cref="KeyboardState"/> instance to the right of the equality operator.</param>
        /// <returns>true if the instances are equal; false otherwise.</returns>
        public static bool operator ==(KeyboardState a, KeyboardState b)
        {
            return a._keys0 == b._keys0
                && a._keys1 == b._keys1
                && a._keys2 == b._keys2
                && a._keys3 == b._keys3
                && a._keys4 == b._keys4
                && a._keys5 == b._keys5
                && a._keys6 == b._keys6
                && a._keys7 == b._keys7;
        }

        /// <summary>
        /// Compares whether two <see cref="KeyboardState"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="KeyboardState"/> instance to the left of the inequality operator.</param>
        /// <param name="b"><see cref="KeyboardState"/> instance to the right of the inequality operator.</param>
        /// <returns>true if the instances are different; false otherwise.</returns>
        public static bool operator !=(KeyboardState a, KeyboardState b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="KeyboardState"/> to compare.</param>
        /// <returns>true if the provided <see cref="KeyboardState"/> instance is same with current; false otherwise.</returns>
        public override bool Equals(object? obj)
        {
            return obj is KeyboardState state && this == state;
        }

        #endregion

    }
}