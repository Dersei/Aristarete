using System;
using Aristarete.Scenes;
using Daeira;
using Buffer = Aristarete.Rendering.Buffer;

namespace Aristarete
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var buffer = new Buffer(1440, 720);
            buffer.Clear(FloatColor.Black);
            var displayWindow = new DisplayWindow(new Test(buffer));
            displayWindow.Display();
        }
    }
}