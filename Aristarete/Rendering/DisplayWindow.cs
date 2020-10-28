using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Aristarete.Rendering
{
    public class DisplayWindow
    {
        private readonly Window _window;
        private readonly Rasterizer _rasterizer;
        private readonly WriteableBitmap _wb;
        private readonly List<IRendering> _renderings = new List<IRendering>();

        public DisplayWindow(Rasterizer rasterizer, params IRendering[] renderings)
        {
            _rasterizer = rasterizer;
            _renderings.AddRange(renderings);
            _wb = new WriteableBitmap(_rasterizer.Buffer.Width, _rasterizer.Buffer.Height, 96, 96, PixelFormats.Bgra32,
                null);
            var image = new Image {Source = _wb, Width = _rasterizer.Buffer.Width, Height = _rasterizer.Buffer.Height};
            _window = new Window {Content = image, SizeToContent = SizeToContent.WidthAndHeight};
            CompositionTarget.Rendering += CompositionTargetOnRendering;
            _window.MouseRightButtonDown += WindowOnMouseRightButtonDown;
        }

        private void CompositionTargetOnRendering(object? sender, EventArgs e)
        {
            _rasterizer.Clear();
            foreach (var rendering in _renderings)
            {
                rendering.Run(_rasterizer);
            }

            _wb.WritePixels(new Int32Rect(0, 0, _rasterizer.Buffer.Width, _rasterizer.Buffer.Height),
                _rasterizer.Buffer.Pixels, _wb.BackBufferStride,
                0);
        }

        private void WindowOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            using FileStream stream =
                new FileStream($"output-{DateTime.Now:yyyy-dd-M--HH-mm-ss.fff}.png", FileMode.Create);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_wb));
            encoder.Save(stream);
        }

        public void Display()
        {
            _window.ShowDialog();
        }
    }
}