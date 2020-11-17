using System;
using Aristarete.Basic;
using Aristarete.Rendering;

namespace Aristarete
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var buffer = new Buffer(768, 768);
            buffer.Clear(FloatColor.Black);
            var rasterizer = new Rasterizer(buffer);
            var displayWindow = new DisplayWindow(rasterizer, new ModelRendering());
            displayWindow.Display();
        }
    }
}