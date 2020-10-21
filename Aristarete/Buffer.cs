using System.Runtime.CompilerServices;
using Aristarete.Basic;

namespace Aristarete
{
    public class Buffer
    {
        public uint[,] Pixels { get; private set; }
        public int Width { get; }
        public int Height { get; }
        public int Count => Pixels.Length;

        public Buffer(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new uint[height, width];
        }

        public uint this[int i, int j]
        {
            get => Pixels[j, i];
            set => Pixels[j, i] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckIfSafe(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, uint color)
        {
            if (!CheckIfSafe(x, y)) return;
            Pixels[y, x] = color;
        }

        public void SetPixels(uint[,] pixels)
        {
            Pixels = pixels;
        }

        public void Clear(FloatColor color)
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    Pixels[i, j] = color;
                }
            }
        }

        public void Clear(uint color)
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    Pixels[i, j] = color;
                }
            }
        }
    }
}