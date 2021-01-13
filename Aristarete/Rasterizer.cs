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
            return new Float3((int) ((originalCoord.X + 1.0f) * _width * 0.5f),
                (int) ((-originalCoord.Y + 1.0f) * _height * 0.5f), originalCoord.Z);
        }

        public void Triangle(Vertex[] vertices, Float3[] screenCords, FloatColor[] color, IRenderable mesh)
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
                        ZBuffer[zCoord] = z;
                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                        {
                            var vertex = new Vertex
                            {
                                Position = vertices[0].Position * barycentric.X + vertices[1].Position * barycentric.Y +
                                           vertices[2].Position * barycentric.Z,
                                Normal = vertices[0].Normal * barycentric.X + vertices[1].Normal * barycentric.Y +
                                         vertices[2].Normal * barycentric.Z,
                            };

                            var colorLight = FloatColor.Black;
                            foreach (var light in Statics.Lights)
                            {
                                colorLight += light.Calculate(vertex, mesh);
                            }

                            Buffer[(int) x, (int) y] =
                                color[0] * colorLight * barycentric.X + color[1] * colorLight * barycentric.Y +
                                color[2] * colorLight * barycentric.Z;
                        }
                    }
                }
            }
        }

        public void Triangle(Vertex[] vertices, Float3[] screenCords, FloatColor[] color, Float2[] uvs,
            IRenderable mesh)
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
                        ZBuffer[zCoord] = z;
                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                        {
                            var vertex = new Vertex
                            {
                                Position = vertices[0].Position * barycentric.X + vertices[1].Position * barycentric.Y +
                                           vertices[2].Position * barycentric.Z,
                                Normal = vertices[0].Normal * barycentric.X + vertices[1].Normal * barycentric.Y +
                                         vertices[2].Normal * barycentric.Z,
                            };

                            var colorLight = FloatColor.Black;
                            foreach (var light in Statics.Lights)
                            {
                                colorLight += light.Calculate(vertex, mesh);
                            }

                            var uv = uvs[0] * barycentric.X + uvs[1] * barycentric.Y + uvs[2] * barycentric.Z;
                            var textureColor = mesh.GetDiffuse(uv);
                            Buffer[(int) x, (int) y] =
                                color[0] * colorLight * textureColor * barycentric.X +
                                color[1] * colorLight * textureColor * barycentric.Y +
                                color[2] * colorLight * textureColor * barycentric.Z;
                        }
                    }
                }
            }
        }


        public void TriangleVertices(Float3[] vertices, FloatColor[] color)
        {
            vertices[0] = ToBufferCoords(vertices[0]);
            vertices[1] = ToBufferCoords(vertices[1]);
            vertices[2] = ToBufferCoords(vertices[2]);

            var bBoxMin = new Float2(float.MaxValue, float.MaxValue);
            var bBoxMax = new Float2(float.MinValue, float.MinValue);
            var clamp = new Float2(_width - 1, _height - 1);

            for (var i = 0; i < 3; i++)
            {
                bBoxMin = Float2.MinAbsolute(bBoxMin, vertices[i].XY);
                bBoxMax = Float2.MaxClamped(bBoxMax, vertices[i].XY, clamp);
            }

            var barycentricHelper = new BarycentricHelper(vertices[0], vertices[1], vertices[2]);

            for (var x = bBoxMin.X; x <= bBoxMax.X; x++)
            {
                for (var y = bBoxMin.Y; y <= bBoxMax.Y; y++)
                {
                    var barycentric = barycentricHelper.Calculate(x, y);
                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0) continue;
                    float z = 0;
                    z += 1f / vertices[0].Z * barycentric.X;
                    z += 1f / vertices[1].Z * barycentric.Y;
                    z += 1f / vertices[2].Z * barycentric.Z;
                    z = 1f / z;
                    var zCoord = (int) (x + y * _width);
                    if (z < ZBuffer[zCoord])
                    {
                        ZBuffer[zCoord] = z;
                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                            Buffer[(int) x, (int) y] = color[0] * barycentric.X + color[1] * barycentric.Y +
                                                       color[2] * barycentric.Z;
                    }
                }
            }
        }

        public void TriangleVertices(Float3[] vertices, FloatColor[] color, Float2[] uvs,
            IRenderable mesh)
        {
            vertices[0] = ToBufferCoords(vertices[0]);
            vertices[1] = ToBufferCoords(vertices[1]);
            vertices[2] = ToBufferCoords(vertices[2]);

            var bBoxMin = new Float2(float.MaxValue, float.MaxValue);
            var bBoxMax = new Float2(float.MinValue, float.MinValue);
            var clamp = new Float2(_width - 1, _height - 1);

            for (var i = 0; i < 3; i++)
            {
                bBoxMin = Float2.MinAbsolute(bBoxMin, vertices[i].XY);
                bBoxMax = Float2.MaxClamped(bBoxMax, vertices[i].XY, clamp);
            }

            var barycentricHelper = new BarycentricHelper(vertices[0], vertices[1], vertices[2]);

            for (var x = bBoxMin.X; x <= bBoxMax.X; x++)
            {
                for (var y = bBoxMin.Y; y <= bBoxMax.Y; y++)
                {
                    var barycentric = barycentricHelper.Calculate(x, y);
                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0) continue;
                    float z = 0;
                    z += 1f / vertices[0].Z * barycentric.X;
                    z += 1f / vertices[1].Z * barycentric.Y;
                    z += 1f / vertices[2].Z * barycentric.Z;
                    z = 1f / z;
                    var zCoord = (int) (x + y * _width);
                    if (z < ZBuffer[zCoord])
                    {
                        ZBuffer[zCoord] = z;
                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                        {
                            var uv = uvs[0] * barycentric.X + uvs[1] * barycentric.Y + uvs[2] * barycentric.Z;
                            var textureColor = mesh.GetDiffuse(uv);
                            Buffer[(int) x, (int) y] =
                                color[0] * textureColor * barycentric.X +
                                color[1] * textureColor * barycentric.Y +
                                color[2] * textureColor * barycentric.Z;
                        }
                    }
                }
            }
        }
        
        public void Triangle(Float3[] vertices, Float2[] uvs, Model model)
        {
            vertices[0] = ToBufferCoords(vertices[0]);
            vertices[1] = ToBufferCoords(vertices[1]);
            vertices[2] = ToBufferCoords(vertices[2]);

            var bBoxMin = new Float2(float.MaxValue, float.MaxValue);
            var bBoxMax = new Float2(float.MinValue, float.MinValue);
            var clamp = new Float2(_width - 1, _height - 1);

            for (var i = 0; i < 3; i++)
            {
                bBoxMin = Float2.MinAbsolute(bBoxMin, vertices[i].XY);
                bBoxMax = Float2.MaxClamped(bBoxMax, vertices[i].XY, clamp);
            }

            var barycentricHelper = new BarycentricHelper(vertices[0], vertices[1], vertices[2]);

            for (var x = bBoxMin.X; x <= bBoxMax.X; x++)
            {
                for (var y = bBoxMin.Y; y <= bBoxMax.Y; y++)
                {
                    var barycentric = barycentricHelper.Calculate(x, y);
                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0) continue;
                    float z = 0;
                    z += 1f / vertices[0].Z * barycentric.X;
                    z += 1f / vertices[1].Z * barycentric.Y;
                    z += 1f / vertices[2].Z * barycentric.Z;
                    z = 1f / z;
                    var zCoord = (int) (x + y * _width);
                    if (z < ZBuffer[zCoord])
                    {
                        ZBuffer[zCoord] = z;

                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                        {
                            var uv = uvs[0] * barycentric.X + uvs[1] * barycentric.Y + uvs[2] * barycentric.Z;
                            var color = model.GetDiffuse(uv);
                            Buffer[(int) x, (int) y] = color;
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

            public BarycentricHelper(Float3 v1, Float3 v2, Float3 v3)
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