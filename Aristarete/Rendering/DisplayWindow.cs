using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aristarete.Inputting;

namespace Aristarete.Rendering
{
    public class DisplayWindow
    {
        private readonly Window _window;
        private readonly Rasterizer _rasterizer;
        private readonly WriteableBitmap _wb;
        private readonly Label _fpsLabel;
        private readonly List<IRendering> _renderings = new List<IRendering>();

        public DisplayWindow(Rasterizer rasterizer, params IRendering[] renderings)
        {
            _rasterizer = rasterizer;
            _renderings.AddRange(renderings);
            _wb = new WriteableBitmap(_rasterizer.Buffer.Width, _rasterizer.Buffer.Height, 96, 96, PixelFormats.Bgra32,
                null);
            var image = new Image {Source = _wb, Width = _rasterizer.Buffer.Width, Height = _rasterizer.Buffer.Height};
            var canvasPanel = new Canvas {Width = image.Width, Height = image.Height};
            _fpsLabel = new Label {Content = "FPS", Foreground = Brushes.White};
            canvasPanel.Children.Add(image);
            canvasPanel.Children.Add(_fpsLabel);
            _window = new Window {Content = canvasPanel, SizeToContent = SizeToContent.WidthAndHeight};
            CompositionTarget.Rendering += CompositionTargetOnRendering;
            CompositionTarget.Rendering += FpsCounting;
            CompositionTarget.Rendering += Input.Update;
            _window.MouseRightButtonDown += WindowOnMouseRightButtonDown;
            _stopwatch = new Stopwatch();
        }

        private void CompositionTargetOnRendering(object? sender, EventArgs e)
        {
            _rasterizer.Clear();
            foreach (var rendering in _renderings)
            {
                rendering.Run(_rasterizer);
            }

            _wb.WritePixels(new Int32Rect(0, 0, _rasterizer.Buffer.Width, _rasterizer.Buffer.Height),
                _rasterizer.Buffer.Pixels, _wb.BackBufferStride, 0);
        }


        private readonly Stopwatch _stopwatch;
        private int _frameCounter;

        private void FpsCounting(object? sender, EventArgs e)
        {
            if (_frameCounter++ == 0)
            {
                _stopwatch.Start();
            }

            var frameRate = (long) (_frameCounter / _stopwatch.Elapsed.TotalSeconds);

            if (frameRate > 0)
            {
                _fpsLabel.Content = frameRate.ToString();
            }
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