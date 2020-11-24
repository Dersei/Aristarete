using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Aristarete.Imaging
{
    public static class PngImage
    {

        public static void SaveWithWpf(string filename, Buffer buffer)
        {
            WriteableBitmap wb = new WriteableBitmap(buffer.Width, buffer.Height, 96, 96, PixelFormats.Bgra32, null);
            wb.WritePixels(new Int32Rect(0, 0, buffer.Width, buffer.Height), buffer.Pixels, wb.BackBufferStride, 0);
            using FileStream stream = new FileStream(filename, FileMode.Create);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(wb));
            encoder.Save(stream);
        }
    }
}