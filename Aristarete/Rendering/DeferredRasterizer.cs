﻿using System;
using System.Threading.Tasks;
using Aristarete.Basic;
using Aristarete.Cameras;
using Aristarete.Meshes;


namespace Aristarete.Rendering
{
    public class DeferredRasterizer : Rasterizer
    {
        private readonly FloatColor[] _diffuseBuffer;
        private readonly FloatColor[] _colorBuffer;
        private readonly FloatColor[] _specularBuffer;
        private readonly FloatColor[] _emissionBuffer;
        private readonly Float3Sse[] _normalBuffer;
        private readonly Float3Sse[] _worldPositionBuffer;

        private readonly int Size;

        public DeferredRasterizer(Buffer buffer, Scene scene, Camera camera) : base(buffer, scene, camera)
        {
            Size = Width * Height;
            _diffuseBuffer = new FloatColor[Width * Height];
            _colorBuffer = new FloatColor[Width * Height];
            _specularBuffer = new FloatColor[Width * Height];
            _emissionBuffer = new FloatColor[Width * Height];
            _normalBuffer = new Float3Sse[Width * Height];
            _worldPositionBuffer = new Float3Sse[Width * Height];
            Array.Fill(_diffuseBuffer, FloatColor.Black);
            Array.Fill(_colorBuffer, FloatColor.Black);
            Array.Fill(_specularBuffer, FloatColor.Black);
            Array.Fill(_emissionBuffer, FloatColor.Black);
            Array.Fill(_normalBuffer, Float3Sse.One);
            Array.Fill(_worldPositionBuffer, Float3Sse.Zero);
        }

        public override void Render(RenderMode renderMode = RenderMode.Color)
        {
            Parallel.ForEach(Scene.Renderables, mesh =>
            {
                mesh.Update(this);
                for (int k = 0; k < mesh.Triangles.Count; k++)
                {
                    Triangle(mesh.Triangles[k], mesh.ScreenCoords[k], mesh.WorldCoords[k], mesh, mesh.LightingMode,
                        renderMode);
                }
            });

            RenderToScreen(BufferMode.Standard);
        }

        private FloatColor CalculateLight(in DeferredData deferredData)
        {
            var colorLight = FloatColor.Black;
            foreach (var light in Scene.Lights)
            {
                colorLight += light.Calculate(deferredData, this);
            }

            return colorLight;
        }

        public override void Triangle(in Triangle triangle, Float3Sse[] screenCords, Float3Sse[] worldCoords, Mesh mesh,
            LightingMode lightingMode,
            RenderMode renderMode = RenderMode.Color)
        {
            Float3Sse[] bufferCoords = new Float3Sse[3];
            bufferCoords[0] = ToBufferCoords(screenCords[0]);
            bufferCoords[1] = ToBufferCoords(screenCords[1]);
            bufferCoords[2] = ToBufferCoords(screenCords[2]);

            var bBoxMin = new Float2(float.MaxValue, float.MaxValue);
            var bBoxMax = new Float2(float.MinValue, float.MinValue);
            var clamp = new Float2(Width - 1, Height - 1);

            for (var i = 0; i < 3; i++)
            {
                bBoxMin = Float2.MinAbsolute(bBoxMin, bufferCoords[i].XY());
                bBoxMax = Float2.MaxClamped(bBoxMax, bufferCoords[i].XY(), clamp);
            }

            var barycentricHelper = new BarycentricHelper(bufferCoords[0], bufferCoords[1], bufferCoords[2]);

            for (var x = bBoxMin.X; x <= bBoxMax.X; x++)
            {
                for (var y = bBoxMin.Y; y <= bBoxMax.Y; y++)
                {
                    var barycentric = barycentricHelper.Calculate(x, y);
                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0) continue;
                    float z = 0;
                    z += screenCords[0].Z * barycentric.X;
                    z += screenCords[1].Z * barycentric.Y;
                    z += screenCords[2].Z * barycentric.Z;
                    //z = 1f / z;
                    var zCoord = x + y * Width;
                    if (z < ZBuffer[(int) zCoord])
                    {
                        if (IsOutsideFrustum(z)) continue;
                        var uv = triangle.First.UV * barycentric.X + triangle.Second.UV * barycentric.Y +
                                 triangle.Third.UV * barycentric.Z;

                        ZBuffer[(int) zCoord] = z;

                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                        {
                            if (renderMode == RenderMode.DepthOnly)
                            {
                                var ndc = z * 2.0f - 1.0f;
                                var linearDepth = 2.0f * 0.1f * 100 / (100 + 0.1f - ndc * (100 - 0.1f));
                                Buffer[(int) x, (int) y] = new FloatColor(linearDepth, linearDepth, linearDepth);
                                continue;
                            }

                            var normal = triangle.First.Normal * barycentric.X +
                                         triangle.Second.Normal * barycentric.Y +
                                         triangle.Third.Normal * barycentric.Z;
                            var index = (int) (x + y * Width);
                            var n = mesh.TransformNormals(normal)
                                .NormalizeExact() + mesh.Material.GetNormals(uv);
                            n = n.NormalizeExact();
                            _normalBuffer[index] = n;
                            _colorBuffer[index] = mesh.Material.Color;
                            _diffuseBuffer[index] = mesh.Material.GetDiffuse(uv);
                            _specularBuffer[index] = mesh.Material.GetSpecular(uv);
                            _emissionBuffer[index] = mesh.Material.GetEmissive(uv);
                            _worldPositionBuffer[index] = worldCoords[0] * barycentric.X +
                                                          worldCoords[1] * barycentric.Y +
                                                          worldCoords[2] * barycentric.Z;
                        }
                    }
                }
            }
        }

        private void RenderToScreen(BufferMode bufferMode = BufferMode.Standard)
        {
            Parallel.For(0, Size, i =>
            {
                var column = i % Width;
                var row = (i - column) / Width;
                var depth = ZBuffer[i];
                if (depth < Camera.Far && depth > Camera.Near)
                {
                    var deferredData = new DeferredData(_worldPositionBuffer[i], _normalBuffer[i], _colorBuffer[i],
                        _diffuseBuffer[i], _specularBuffer[i], _emissionBuffer[i]);

                    switch (bufferMode)
                    {
                        case BufferMode.Standard:
                            Buffer[column, row] =
                                CalculateLight(deferredData) * deferredData.Diffuse *
                                deferredData.Color + deferredData.Emission
                                -
                                ShadowFactor(deferredData.Position);
                            break;
                        case BufferMode.Normal:
                            Buffer[column, row] = new FloatColor(deferredData.Normal);
                            break;
                        case BufferMode.Color:
                            Buffer[column, row] = deferredData.Color;
                            break;
                        case BufferMode.Diffuse:
                            Buffer[column, row] = deferredData.Diffuse;
                            break;
                        case BufferMode.Specular:
                            Buffer[column, row] = deferredData.Specular;
                            break;
                        case BufferMode.Emission:
                            Buffer[column, row] = deferredData.Emission;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(bufferMode), bufferMode, null);
                    }
                }
                else
                {
                    Buffer[column, row] = FloatColor.Black;
                }
            });
            // Parallel.For(0, Width, x =>
            // {
            //     for (var y = 0; y < Height; ++y)
            //     {
            //         var i = y * Width + x;
            //         var depth = ZBuffer[i];
            //         if (depth < Camera.Far && depth > Camera.Near)
            //         {
            //             var deferredData = new DeferredData(_worldPositionBuffer[i], _normalBuffer[i], _colorBuffer[i],
            //                 _diffuseBuffer[i], _specularBuffer[i], _emissionBuffer[i]);
            //
            //             switch (bufferMode)
            //             {
            //                 case BufferMode.Standard:
            //                     Buffer[x, y] =
            //                         CalculateLight(deferredData) * deferredData.Diffuse *
            //                         deferredData.Color + deferredData.Emission
            //                         -
            //                         ShadowFactor(deferredData.Position);
            //                     break;
            //                 case BufferMode.Normal:
            //                     Buffer[x, y] = new FloatColor(deferredData.Normal);
            //                     break;
            //                 case BufferMode.Color:
            //                     Buffer[x, y] = deferredData.Color;
            //                     break;
            //                 case BufferMode.Diffuse:
            //                     Buffer[x, y] = deferredData.Diffuse;
            //                     break;
            //                 case BufferMode.Specular:
            //                     Buffer[x, y] = deferredData.Specular;
            //                     break;
            //                 case BufferMode.Emission:
            //                     Buffer[x, y] = deferredData.Emission;
            //                     break;
            //                 default:
            //                     throw new ArgumentOutOfRangeException(nameof(bufferMode), bufferMode, null);
            //             }
            //         }
            //         else
            //         {
            //             Buffer[x, y] = FloatColor.Black;
            //         }
            //     }
            // });
            // for (var x = 0; x < Width; ++x)
            // {
            //     for (var y = 0; y < Height; ++y)
            //     {
            //         var i = y * Width + x;
            //         var depth = ZBuffer[i];
            //         if (depth < Camera.Far && depth > Camera.Near)
            //         {
            //             var deferredData = new DeferredData(_worldPositionBuffer[i], _normalBuffer[i], _colorBuffer[i],
            //                 _diffuseBuffer[i], _specularBuffer[i], _emissionBuffer[i]);
            //
            //             switch (bufferMode)
            //             {
            //                 case BufferMode.Standard:
            //                     Buffer[x, y] = 
            //                         CalculateLight(deferredData) * deferredData.Diffuse *
            //                         deferredData.Color + deferredData.Emission 
            //                         - 
            //                         ShadowFactor(deferredData.Position);
            //                     break;
            //                 case BufferMode.Normal:
            //                     Buffer[x, y] = new FloatColor(deferredData.Normal);
            //                     break;
            //                 case BufferMode.Color:
            //                     Buffer[x, y] = deferredData.Color;
            //                     break;
            //                 case BufferMode.Diffuse:
            //                     Buffer[x, y] = deferredData.Diffuse;
            //                     break;
            //                 case BufferMode.Specular:
            //                     Buffer[x, y] = deferredData.Specular;
            //                     break;
            //                 case BufferMode.Emission:
            //                     Buffer[x, y] = deferredData.Emission;
            //                     break;
            //                 default:
            //                     throw new ArgumentOutOfRangeException(nameof(bufferMode), bufferMode, null);
            //             }
            //         }
            //         else
            //         {
            //             Buffer[x, y] = FloatColor.Black;
            //         }
            //     }
            // }
        }

        public enum BufferMode
        {
            Standard,
            Normal,
            Color,
            Diffuse,
            Specular,
            Emission
        }
    }
}