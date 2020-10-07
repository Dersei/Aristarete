using System;
using System.IO;
using SkiaSharp;

namespace Aristarete
{
    public class Image
    {
        public static unsafe void Save(string filename, Buffer buffer)
        {
            SKBitmap bitmap = new SKBitmap(buffer.Width, buffer.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
            
            fixed (uint* ptr = buffer.Pixels)
            {
                bitmap.SetPixels((IntPtr) ptr);
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(filename);
            data.SaveTo(stream);
        }
    }
}