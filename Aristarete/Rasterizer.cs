using System;
using System.Runtime.CompilerServices;
using Aristarete.Basic;
using Aristarete.Meshes;
using Aristarete.Models;

namespace Aristarete
{
    public class Rasterizer
    {
        public Buffer Buffer { get; }

        private readonly int _width;
        private readonly int _height;
        public readonly float[] ZBuffer;

        public Rasterizer(Buffer buffer)
        {
            Buffer = buffer;
            _width = buffer.Width;
            _height = buffer.Height;
            ZBuffer = new float[_width * _height];
            Array.Fill(ZBuffer, float.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Float3 ToBufferCoords(Float3 originalCoord)
        {
            return new((int) ((originalCoord.X + 1.0f) * _width * 0.5f),
                (int) ((-originalCoord.Y + 1.0f) * _height * 0.5f), originalCoord.Z);
        }

        private static readonly (FloatColor first, FloatColor second, FloatColor third) LightingNoneColors = (
            FloatColor.White, FloatColor.White, FloatColor.White);

        private static (FloatColor first, FloatColor second, FloatColor third) CalculateLight(in Triangle triangle,
            IRenderable mesh, Float3 barycentric, LightingMode mode)
        {
            switch (mode)
            {
                case LightingMode.None: return LightingNoneColors;
                case LightingMode.Vertex:
                {
                    var colorA = FloatColor.Black;
                    var colorB = FloatColor.Black;
                    var colorC = FloatColor.Black;

                    foreach (var light in Statics.Lights)
                    {
                        colorA += light.Calculate(triangle.First, mesh);
                        colorB += light.Calculate(triangle.Second, mesh);
                        colorC += light.Calculate(triangle.Third, mesh);
                    }

                    return (
                        colorA, 
                        colorB, 
                        colorC);
                }
                case LightingMode.Pixel:
                {
                    var vertex = new Vertex
                    {
                        Position = triangle.First.Position * barycentric.X + triangle.Second.Position * barycentric.Y +
                                   triangle.Third.Position * barycentric.Z,
                        Normal = triangle.First.Normal * barycentric.X + triangle.Second.Normal * barycentric.Y +
                                 triangle.Third.Normal * barycentric.Z,
                        UV = triangle.First.UV * barycentric.X + triangle.Second.UV * barycentric.Y +
                             triangle.Third.UV * barycentric.Z
                    };

                    var colorLight = FloatColor.Black;
                    foreach (var light in Statics.Lights)
                    {
                        colorLight += light.Calculate(vertex, mesh);
                    }

                    return (
                        colorLight, 
                        colorLight, 
                        colorLight);
                }
            }

            throw new IndexOutOfRangeException();
        }

        public void Triangle(in Triangle triangle, Float3[] screenCords, IRenderable mesh, LightingMode mode)
        {
            screenCords[0] = ToBufferCoords(screenCords[0]);
            screenCords[1] = ToBufferCoords(screenCords[1]);
            screenCords[2] = ToBufferCoords(screenCords[2]);

            var bBoxMin = new Float2(float.MaxValue, float.MaxValue);
            var bBoxMax = new Float2(float.MinValue, float.MinValue);
            var clamp = new Float2(_width - 1, _height - 1);

            for (var i = 0; i < 3; i++)
            {
                bBoxMin = Float2.MinAbsolute(bBoxMin, screenCords[i].XY);
                bBoxMax = Float2.MaxClamped(bBoxMax, screenCords[i].XY, clamp);
            }

            var barycentricHelper = new BarycentricHelper(screenCords[0], screenCords[1], screenCords[2]);

            for (var x = bBoxMin.X; x <= bBoxMax.X; x++)
            {
                for (var y = bBoxMin.Y; y <= bBoxMax.Y; y++)
                {
                    var barycentric = barycentricHelper.Calculate(x, y);
                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0) continue;
                    float z = 0;
                    z += 1f / screenCords[0].Z * barycentric.X;
                    z += 1f / screenCords[1].Z * barycentric.Y;
                    z += 1f / screenCords[2].Z * barycentric.Z;
                    z = 1f / z;
                    var zCoord = (int) (x + y * _width);
                    if (z < ZBuffer[zCoord])
                    {
                        var uv = triangle.First.UV * barycentric.X + triangle.Second.UV * barycentric.Y +
                                 triangle.Third.UV * barycentric.Z;
                        if(mesh.Material.GetOpacity(uv).R == 0) continue;
                        ZBuffer[zCoord] = z;
                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                        {
                            var lightColors = CalculateLight(triangle, mesh, barycentric, mode);

                           
                            var textureColor = mesh.Material.GetDiffuse(uv);
                            var emissiveColor = mesh.Material.GetEmissive(uv);
                            Buffer[(int) x, (int) y] =
                                lightColors.first * textureColor * barycentric.X  +
                                lightColors.second * textureColor * barycentric.Y +
                                lightColors.third * textureColor * barycentric.Z + emissiveColor;
                        }
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

            public BarycentricHelper(in Float3 v1, in Float3 v2, in Float3 v3)
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

            public Float3 Calculate(float x, float y)
            {
                var a = (Dy23 * (x - V3X) + Dx32 * (y - V3Y)) * MultiplierA;
                var b = (Dy31 * (x - V3X) + Dx13 * (y - V3Y)) * MultiplierB;
                var c = 1 - a - b;

                return new Float3(a, b, c);
            }
        }

        public void Clear()
        {
            Buffer.Clear(FloatColor.Black);
            Array.Fill(ZBuffer, float.MaxValue);
        }
    }
}