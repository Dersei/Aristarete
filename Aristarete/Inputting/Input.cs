using System;

namespace Aristarete.Inputting
{
    public static class Input
    {
        private static KeyboardState _lastKeyboardState;
        private static KeyboardState _currentKeyboardState;
        private static bool _refreshData;

        /// <summary>
        /// Fetches the latest input states.
        /// </summary>
        public static void Update(object? sender, EventArgs e)
        {
            if (!_refreshData)
                _refreshData = true;

            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// The previous keyboard state.
        /// </summary>
        public static KeyboardState LastKeyboardState => _lastKeyboardState;

        /// <summary>
        /// The current state of the keyboard.
        /// </summary>
        public static KeyboardState CurrentKeyboardState => _currentKeyboardState;
        
        /// <summary>
        /// Used for debug purposes.
        /// Indicates if the user wants to exit immediately.
        /// </summary>
        public static bool IsExitPressed => IsCurrentPress(Keys.Escape);

        /// <summary>
        /// Checks if the requested key is currently pressed and wasn't pressed in the last state.
        /// </summary>
        public static bool IsNewPress(Keys key) => _lastKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Checks if the requested key is pressed currently and in the last state.
        /// </summary>
        public static bool IsCurrentPress(Keys key) => _lastKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Checks if the requested key was pressed in the last state and not currently.
        /// </summary>
        public static bool IsReleased(Keys key) => _lastKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);
    }
}