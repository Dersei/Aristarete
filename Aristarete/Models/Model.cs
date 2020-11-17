using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Aristarete.Basic;

namespace Aristarete.Models
{
    public class Model
    {
        public List<Float3> Vertices { get; } = new List<Float3>();
        public List<Int3[]> Faces { get; } = new List<Int3[]>();
        public List<Float2> UV { get; } = new List<Float2>();
        public FloatColor ColorAngle = new FloatColor(1,1,1,1);
        private uint[]? _diffuseMap;
        private int _mapWidth;
        private int _mapHeight;

        public Model(string filename)
        {
            using var stream = new StreamReader(filename);

            while (!stream.EndOfStream)
            {
                var line = stream.ReadLine();
                if (line is null)
                {
                }
                else if (line.StartsWith("v "))
                {
                    var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                        .Select(v => float.Parse(v, CultureInfo.InvariantCulture))
                        .ToArray();
                    Vertices.Add(new Float3(array[0], array[1], array[2]));
                }
                else if (line.StartsWith("f "))
                {
                    var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                        .Select(fvn => fvn.Split("/").Select(int.Parse).ToArray())
                        .Select(a => new Int3(--a[0], --a[1], --a[2])).ToArray();
                    Faces.Add(array);
                }
                else if (line.StartsWith("vt "))
                {
                    var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                        .Select(v => float.Parse(v, CultureInfo.InvariantCulture)).ToArray();
                    UV.Add(new Float2(array[0], array[1]));
                }
            }

            LoadTexture(filename, "_diffuse.png");
        }

        private void LoadTexture(string filename, string suffix)
        {
            var dot = filename.LastIndexOf(".", StringComparison.Ordinal);
            if (dot != -1)
            {
                filename = filename.Substring(0, dot) + suffix;
                var img = new BitmapImage(new Uri(filename, UriKind.Relative));
                var wbm = new WriteableBitmap(img);
                var colors = new uint[wbm.PixelWidth * wbm.PixelHeight];
                wbm.CopyPixels(colors, wbm.BackBufferStride, 0);
                _diffuseMap = colors;
                _mapWidth = wbm.PixelWidth;
                _mapHeight = wbm.PixelHeight;
            }
        }

        public Int3 GetFace(int idx) =>
            new Int3
            (
                Faces[idx][0].X,
                Faces[idx][1].X,
                Faces[idx][2].X
            );

        public Float2 GetUV(int face, int vertex)
        {
            if (_diffuseMap is null) return Float2.Zero;
            var idx = Faces[face][vertex].Y;
            return new Float2(UV[idx].X * _mapWidth, UV[idx].Y * _mapHeight);
        }


        public FloatColor GetDiffuse(Float2 uv)
        {
            if (_diffuseMap is null) return FloatColor.Error;
            var x = (int) uv.X;
            var y = (int) uv.Y;
            return FloatColor.FromArgb(_diffuseMap[x + (_mapHeight - y - 1) * _mapWidth]) * ColorAngle;
        }
    }
}