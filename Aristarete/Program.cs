using System;

namespace Aristarete
{
    internal static class Program
    {
        public static uint PackColor(byte r, byte g, byte b, byte a = 255)
        {
            return (uint)((a << 24) | (b << 16) | (g << 8) | r);
        }

        private static void Main()
        {
            var buffer = new Buffer(512, 512);
            buffer.Clear(PackColor(255, 0, 0, 128));
            Image.Save($"output{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.png", buffer);
        }
    }
}