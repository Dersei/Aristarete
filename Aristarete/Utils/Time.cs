using System;

namespace Aristarete.Utils
{
    public static class Time
    {
        public static float Scale { get; set; } = 1.0f;
        private static TimeContainer _deltaTime;
        public static TimeContainer DeltaTime => _deltaTime;
        private static TimeContainer _totalTime;
        public static TimeContainer TotalTime => _totalTime;
        public static TimeSpan RealGameTime { get; private set; }
        public static TimeSpan LastUpdate { get; private set; }

        public struct TimeContainer
        {
            public float Milliseconds;
            public float Seconds;
            public float Minutes;

            public static implicit operator TimeSpan(TimeContainer container)
            {
                return TimeSpan.FromMilliseconds(container.Milliseconds);
            }
        }

        public static void Update(TimeSpan time)
        {
            RealGameTime = time;
            var difference = time - LastUpdate;
            _deltaTime.Seconds = (float) difference.TotalSeconds * Scale;
            _deltaTime.Milliseconds = (float) difference.TotalMilliseconds * Scale;
            _deltaTime.Minutes = (float) difference.TotalMinutes * Scale;

            _totalTime.Milliseconds += _deltaTime.Milliseconds;
            _totalTime.Seconds += _deltaTime.Seconds;
            _totalTime.Minutes += _deltaTime.Minutes;
            LastUpdate = time;
        }
    }
}