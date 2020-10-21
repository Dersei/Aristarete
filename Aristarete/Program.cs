using System;
using Aristarete.Basic;

namespace Aristarete
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var buffer = new Buffer(512, 512);
            buffer.Clear(FloatColor.Black);

            var rasterizer = new Rasterizer(buffer);

            rasterizer.Triangle(new[]
                {
                    new Vector3(-0.5f, 0.5f, 0.0f),
                    new Vector3(0.5f, 0.5f, 0.0f),
                    new Vector3(0.5f, -0.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Red, 
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    new Vector3(0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f, 0.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Green, 
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    new Vector3(1f, 0.5f, 2.0f),
                    new Vector3(0f, -0.5f, 2.0f),
                    new Vector3(0.8f, -1.5f, -1.0f)
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue, 
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    new Vector3(0.5f, 0.5f, 0.0f),
                    new Vector3(0.5f, -0.5f, 0.0f),
                    new Vector3(1.5f, -1.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow, 
                    FloatColor.UnityYellow
                });

            PngImage.SaveAndDisplay($"output-{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.png", buffer);
        }
    }
}