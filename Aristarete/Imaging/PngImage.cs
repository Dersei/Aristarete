using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace Aristarete.Imaging
{
    public static class PngImage
    {
        public static unsafe void Save(string filename, Buffer buffer)
        {
            SKBitmap bitmap = new SKBitmap(buffer.Width, buffer.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
            
            fixed (uint* ptr = buffer.Pixels)
            {
                bitmap.SetPixels((IntPtr) ptr);
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(filename);
            data.SaveTo(stream);
        }

        public static void SaveWithWpf(string filename, Buffer buffer)
        {
            WriteableBitmap wb = new WriteableBitmap(buffer.Width, buffer.Height, 96, 96, PixelFormats.Bgra32, null);
            wb.WritePixels(new Int32Rect(0, 0, buffer.Width, buffer.Height), buffer.Pixels, wb.BackBufferStride, 0);
            using FileStream stream = new FileStream(filename, FileMode.Create);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(wb));
            encoder.Save(stream);
        }

        public static void SaveAndDisplay(string filename, Buffer buffer)
        {
            WriteableBitmap wb = new WriteableBitmap(buffer.Width, buffer.Height, 96, 96, PixelFormats.Bgra32, null);
            wb.WritePixels(new Int32Rect(0, 0, buffer.Width, buffer.Height), buffer.Pixels, wb.BackBufferStride, 0);
            using FileStream stream = new FileStream(filename, FileMode.Create);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(wb));
            encoder.Save(stream);
            var window = new Window();
            var image = new Image();
            image.Source = wb;
            window.Content = image;
            window.ShowDialog();
        }
    }
}