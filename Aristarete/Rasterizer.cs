using System;
using Aristarete.Basic;

namespace Aristarete
{
    public class Rasterizer
    {
        public Buffer Buffer { get; }

        private readonly int _width;
        private readonly int _height;
        private readonly int[] _zBuffer;

        public Rasterizer(Buffer buffer)
        {
            Buffer = buffer;
            _width = buffer.Width;
            _height = buffer.Height;
            _zBuffer = new int[_width * _height];
            Array.Fill(_zBuffer, int.MaxValue);
        }

        private Vector3 ToBufferCoords(Vector3 originalCoord)
        {
            return new Vector3((int) ((originalCoord.X + 1.0f) * _width * 0.5f),
                (int) ((-originalCoord.Y + 1.0f) * _height * 0.5f), originalCoord.Z);
        }

        public void Triangle(Vector3[] vertices, FloatColor[] color)
        {
            vertices[0] = ToBufferCoords(vertices[0]);
            vertices[1] = ToBufferCoords(vertices[1]);
            vertices[2] = ToBufferCoords(vertices[2]);

            var bBoxMin = new Vector2(float.MaxValue, float.MaxValue);
            var bBoxMax = new Vector2(float.MinValue, float.MinValue);
            var clamp = new Vector2(_width - 1, _height - 1);

            for (var i = 0; i < 3; i++)
            {
                bBoxMin = Vector2.Min(bBoxMin, vertices[i].XY);
                bBoxMax = Vector2.Max(bBoxMax, vertices[i].XY, clamp);
            }

            var z = 0f;

            var barycentricHelper = new BarycentricHelper(vertices[0], vertices[1], vertices[2]);

            for (var x = bBoxMin.X; x <= bBoxMax.X; x++)
            {
                for (var y = bBoxMin.Y; y <= bBoxMax.Y; y++)
                {
                    var barycentric = barycentricHelper.Calculate(x, y, z);
                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0) continue;
                    z = 0;
                    z += vertices[0].Z * barycentric.X;
                    z += vertices[1].Z * barycentric.Y;
                    z += vertices[2].Z * barycentric.Z;
                    var zCoord = (int) (x + y * _width);
                    if (z < _zBuffer[zCoord])
                    {
                        _zBuffer[zCoord] = (int) z;
                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                            Buffer[(int) x, (int) y] = color[0] * barycentric.X + color[1] * barycentric.Y +
                                                       color[2] * barycentric.Z;
                    }
                }
            }
        }

        private readonly struct BarycentricHelper
        {
            public readonly float Dx12;
            public readonly float Dx13;
            public readonly float Dx23;
            public readonly float Dx31;
            public readonly float Dx32;
            public readonly float Dy12;
            public readonly float Dy13;
            public readonly float Dy23;
            public readonly float Dy31;
            public readonly float Dy32;
            public readonly float V3X;
            public readonly float V3Y;
            public readonly float MultiplierA;
            public readonly float MultiplierB;
            public readonly bool FirstEdge;
            public readonly bool SecondEdge;
            public readonly bool ThirdEdge;

            public BarycentricHelper(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                Dx12 = v1.X - v2.X;
                Dx13 = v1.X - v3.X;
                Dx23 = v2.X - v3.X;
                Dx31 = v3.X - v1.X;
                Dx32 = v3.X - v2.X;
                Dy12 = v1.Y - v2.Y;
                Dy13 = v1.Y - v3.Y;
                Dy23 = v2.Y - v3.Y;
                Dy31 = v3.Y - v1.Y;
                Dy32 = v3.Y - v2.Y;
                V3X = v3.X;
                V3Y = v3.Y;
                MultiplierA = 1f / (Dy23 * Dx13 + Dx32 * Dy13);
                MultiplierB = 1f / (Dy31 * Dx23 + Dx13 * Dy23);
                FirstEdge = Dy12 < 0 || Dy12 == 0 && Dx12 > 0;
                SecondEdge = Dy23 < 0 || Dy23 == 0 && Dx23 > 0;
                ThirdEdge = Dy31 < 0 || Dy31 == 0 && Dx31 > 0;
            }

            public Vector3 Calculate(float x, float y, float z)
            {
                var a = (Dy23 * (x - V3X) + Dx32 * (y - V3Y)) * MultiplierA;
                var b = (Dy31 * (x - V3X) + Dx13 * (y - V3Y)) * MultiplierB;
                var c = 1 - a - b;

                return new Vector3(a, b, c);
            }
        }
    }
}