using System;
using Aristarete.TgaTools;
using static Aristarete.ColorUtils;

namespace Aristarete
{
    internal static class Program
    {
        private static void Main()
        {
            var buffer = new Buffer(512, 512);
            buffer.Clear(PackColor(0, 255, 0, 128));
            for (int i = 0; i < buffer.Width; i++)
            {
                for (int j = 0; j < buffer.Height; j++)
                {
                    buffer[i, j] = PackColor((byte) (i / 2), (byte) (j / 2), 0);
                }
            }
            Image.Save($"output{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.png", buffer);

            var tga = TgaImage.FromBuffer(buffer);
            tga.WriteToFile($"output{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.tga");
            //tga.WriteToFile($"output{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.tga", false);
        }
    }
}