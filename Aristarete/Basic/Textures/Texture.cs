using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Aristarete.Basic.Textures
{
    public class Texture : ITexture
    {
        public readonly int Width;
        public readonly int Height;
        public readonly FloatColor[,] ColorMap;

        private Texture(int width, int height, FloatColor[,] colorMap)
        {
            Width = width;
            Height = height;
            ColorMap = colorMap;
        }

        private static int ChangeValue(int value, int limit)
        {
            var counter = value / limit;
            for (var i = 0; i < counter; i++)
            {
                value -= limit;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FixWrongValues(ref int x, ref int y)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;
        }

        public FloatColor GetColor(Float2 uv, Float2 scale, Float2 offset)
        {
            var x = (int) ((uv.X + offset.X) * Width * scale.X);
            var y = (int) ((uv.Y + offset.Y) * Height * scale.Y);
            if (x >= Width)
            {
                x = ChangeValue(x, Width);
            }

            if (y >= Height)
            {
                y = ChangeValue(y, Height);
            }

            FixWrongValues(ref x, ref y);
            return ColorMap[x, y];
        }

        public FloatColor GetColor(Float2 uv)
        {
            var x = (int) (uv.X * Width);
            var y = (int) (uv.Y * Height);
            if (x >= Width)
            {
                x = ChangeValue(x, Width);
            }

            if (y >= Height)
            {
                y = ChangeValue(y, Height);
            }

            FixWrongValues(ref x, ref y);
            return ColorMap[x, y];
        }

        private static readonly Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();

        public static unsafe Texture CreateFrom(BitmapImage bitmap, bool mirrorX = false, bool mirrorY = false)
        {
            var wbm = new WriteableBitmap(bitmap);

            var hash = bitmap.GetHashCode().ToString();
            if (Textures.TryGetValue(hash, out var texture))
            {
                return texture;
            }

            wbm.Lock();

            var pixels = new FloatColor[bitmap.PixelWidth, bitmap.PixelHeight];
            var backBuffer = wbm.BackBuffer;
            var stride = wbm.BackBufferStride;
            var bufferPointer = (byte*) backBuffer.ToPointer();

            FloatColor GetPixel(int xp, int yp)
            {
                if (yp > wbm.PixelHeight - 1 ||
                    xp > wbm.PixelWidth - 1)
                    return FloatColor.FromArgb(0, 0, 0, 0);
                var loc = yp * stride + xp * 4;
                return FloatColor.FromArgb(255, bufferPointer[loc + 2], bufferPointer[loc + 1],
                    bufferPointer[loc]);
            }


            for (var y = 0; y < bitmap.PixelHeight; y++)
            {
                for (var x = 0; x < bitmap.PixelWidth; x++)
                {
                    var indexX = x;
                    var indexY = y;
                    if (mirrorY) indexX = bitmap.PixelWidth - 1 - x;
                    if (mirrorX) indexY = bitmap.PixelHeight - 1 - y;
                    pixels[indexX, indexY] = GetPixel(indexX, indexY);
                }
            }

            wbm.Unlock();
            var result = new Texture(bitmap.PixelWidth, bitmap.PixelHeight, pixels);
            Textures.Add(hash, result);
            return result;
        }

        public static unsafe Texture LoadFrom(string filename, bool mirrorX = false, bool mirrorY = false)
        {
            if (Textures.TryGetValue(filename, out var texture))
            {
                return texture;
            }

            var bitmap = new BitmapImage(new Uri(filename, UriKind.Relative));
            var wbm = new WriteableBitmap(bitmap);
            wbm.Lock();

            var pixels = new FloatColor[bitmap.PixelWidth, bitmap.PixelHeight];
            var backBuffer = wbm.BackBuffer;
            var stride = wbm.BackBufferStride;
            var bufferPointer = (byte*) backBuffer.ToPointer();

            FloatColor GetPixel(int xp, int yp)
            {
                if (yp > wbm.PixelHeight - 1 ||
                    xp > wbm.PixelWidth - 1)
                    return FloatColor.FromArgb(0, 1, 0, 0);
                var loc = yp * stride + xp * (wbm.Format.BitsPerPixel/4);
                return FloatColor.FromArgb(255, bufferPointer[loc + 2], bufferPointer[loc + 1],
                    bufferPointer[loc]);
            }


            for (var y = 0; y < bitmap.PixelHeight; y++)
            {
                for (var x = 0; x < bitmap.PixelWidth; x++)
                {
                    var indexX = x;
                    var indexY = y;
                    if (mirrorY) indexX = bitmap.PixelWidth - 1 - x;
                    if (mirrorX) indexY = bitmap.PixelHeight - 1 - y;
                    pixels[indexX, indexY] = GetPixel(indexX, indexY);
                }
            }
            wbm.Unlock();
            var result = new Texture(bitmap.PixelWidth, bitmap.PixelHeight, pixels);
            Textures.Add(filename, result);
            return result;
        }

        public static Texture GenerateWith(Func<int, int, FloatColor> colorGenerator)
        {
            const int height = 512;
            const int width = 512;
            if (Textures.TryGetValue(colorGenerator.GetHashCode().ToString(), out var texture))
            {
                return texture;
            }

            var pixels = new FloatColor[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    pixels[x, y] = colorGenerator(x, y);
                }
            }

            var result = new Texture(width, height, pixels);
            Textures.Add(colorGenerator.GetHashCode().ToString(), result);
            return result;
        }
    }
}