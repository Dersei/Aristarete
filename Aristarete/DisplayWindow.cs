using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aristarete.Inputting;
using Aristarete.Rendering;
using Aristarete.Utils;

namespace Aristarete
{
    public class DisplayWindow
    {
        private readonly Window _window;
        private readonly Rasterizer _rasterizer;
        private readonly WriteableBitmap _wb;
        private readonly TextBlock _fpsLabel;
        private readonly Scene _rendering;

        public DisplayWindow(Scene rendering)
        {
            _rendering = rendering;
            _rasterizer = rendering.Rasterizer;
            _wb = new WriteableBitmap(_rasterizer.Buffer.Width, _rasterizer.Buffer.Height, 96, 96, PixelFormats.Bgra32,
                null);
            var image = new Image {Source = _wb, Width = _rasterizer.Buffer.Width, Height = _rasterizer.Buffer.Height};
            var canvasPanel = new Canvas {Width = image.Width, Height = image.Height};
            _fpsLabel = new TextBlock {Text = "FPS", Foreground = Brushes.White, FontSize = 24};
            canvasPanel.Children.Add(image);
            canvasPanel.Children.Add(_fpsLabel);
            _window = new Window
            {
                Content = canvasPanel, SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            CompositionTarget.Rendering += CompositionTargetOnRendering;
            CompositionTarget.Rendering += FpsCounting;
            CompositionTarget.Rendering += Input.Update;
            _window.MouseRightButtonDown += WindowOnMouseRightButtonDown;
            _stopwatch = new Stopwatch();
        }

        private void CompositionTargetOnRendering(object? sender, EventArgs e)
        {
            Time.Update(_stopwatch.Elapsed);
            _rasterizer.Clear();
            _rendering.Run();
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
                _fpsLabel.Text = frameRate.ToString();
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