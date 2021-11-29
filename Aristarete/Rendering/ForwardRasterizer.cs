using System;
using System.Threading.Tasks;
using Aristarete.Cameras;
using Aristarete.Meshes;
using Daeira;


namespace Aristarete.Rendering
{
    public class ForwardRasterizer : Rasterizer
    {
        public ForwardRasterizer(Buffer buffer, Scene scene,Camera camera) : base(buffer,scene,camera)
        {
        }

        private static readonly (FloatColor first, FloatColor second, FloatColor third) LightingNoneColors = (
            FloatColor.White, FloatColor.White, FloatColor.White);

        private (FloatColor first, FloatColor second, FloatColor third) CalculateLight(in Triangle triangle, Float3[] worldCoords,
            Mesh mesh, Float3 barycentric, LightingMode mode)
        {
            switch (mode)
            {
                case LightingMode.None: return LightingNoneColors;
                case LightingMode.Vertex:
                {
                    var colorA = FloatColor.Black;
                    var colorB = FloatColor.Black;
                    var colorC = FloatColor.Black;

                    foreach (var light in Scene.Lights)
                    {
                        colorA += light.Calculate(triangle.First, mesh,this);
                        colorB += light.Calculate(triangle.Second, mesh,this);
                        colorC += light.Calculate(triangle.Third, mesh,this);
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
                        Position = worldCoords[0] * barycentric.X + worldCoords[1] * barycentric.Y +
                                   worldCoords[2] * barycentric.Z,
                        Normal = triangle.First.Normal * barycentric.X + triangle.Second.Normal * barycentric.Y +
                                 triangle.Third.Normal * barycentric.Z,
                        UV = triangle.First.UV * barycentric.X + triangle.Second.UV * barycentric.Y +
                             triangle.Third.UV * barycentric.Z
                    };

                    var colorLight = FloatColor.Black;
                    foreach (var light in Scene.Lights)
                    {
                        colorLight += light.Calculate(vertex, mesh,this);
                    }

                    return (
                        colorLight,
                        colorLight,
                        colorLight);
                }
            }

            throw new IndexOutOfRangeException();
        }

       

        public override void Render(RenderMode renderMode = RenderMode.Color)
        {
            Parallel.ForEach(Scene.Renderables, mesh =>
            {
                mesh.Update(this);
                for (int k = 0; k < mesh.Triangles.Count; k++)
                {
                    Triangle(mesh.Triangles[k], mesh.ScreenCoords[k], mesh.WorldCoords[k],mesh, mesh.LightingMode, renderMode);
                }
            });
        }

        public override void Triangle(in Triangle triangle, Float3[] screenCords, Float3[] worldCoords, Mesh mesh,
            LightingMode lightingMode,
            RenderMode renderMode = RenderMode.Color)
        {
            Float3[] bufferCoords = new Float3[3];
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

            if (renderMode == RenderMode.Vertices)
            {
                DrawEllipseCentered((int) bufferCoords[0].X, (int) bufferCoords[0].Y, 1, 1, FloatColor.White);
                DrawEllipseCentered((int) bufferCoords[1].X, (int) bufferCoords[1].Y, 1, 1, FloatColor.White);
                DrawEllipseCentered((int) bufferCoords[2].X, (int) bufferCoords[2].Y, 1, 1, FloatColor.White);
            }
            else if (renderMode == RenderMode.Wireframe)
            {
                DrawTriangle(new Triangle(bufferCoords[0], bufferCoords[1], bufferCoords[2]));
            }
            else if (renderMode == RenderMode.WireframeAndVertices)
            {
                DrawEllipseCentered((int) bufferCoords[0].X, (int) bufferCoords[0].Y, 1, 1, FloatColor.White);
                DrawEllipseCentered((int) bufferCoords[1].X, (int) bufferCoords[1].Y, 1, 1, FloatColor.White);
                DrawEllipseCentered((int) bufferCoords[2].X, (int) bufferCoords[2].Y, 1, 1, FloatColor.White);
                DrawTriangle(new Triangle(bufferCoords[0], bufferCoords[1], bufferCoords[2]));
            }

            var barycentricHelper = new BarycentricHelper(bufferCoords[0], bufferCoords[1], bufferCoords[2]);

            for (var x = bBoxMin.X; x <= bBoxMax.X; x++)
            {
                for (var y = bBoxMin.Y; y <= bBoxMax.Y; y++)
                {
                    var barycentric = barycentricHelper.Calculate(x, y);
                    if (barycentric.X < 0 || barycentric.Y < 0 || barycentric.Z < 0) continue;
                    float z = 0;
                    //if(screenCords[0].X< 0)Console.WriteLine(screenCords[0].X);
                    z += screenCords[0].Z * barycentric.X;
                    z += screenCords[1].Z * barycentric.Y;
                    z += screenCords[2].Z * barycentric.Z;

                   // z = 1f / z;
                    //z = (1/z - 1/0.1f) / (1/100f - 1/0.1f);
                    var zCoord = x + y * Width;
                    //Console.WriteLine(z);
                    if (z < ZBuffer[(int) zCoord])
                    {
                        if (IsOutsideFrustum(z)) continue;
                        var uv = triangle.First.UV * barycentric.X + triangle.Second.UV * barycentric.Y +
                                 triangle.Third.UV * barycentric.Z;
                        //if (mesh.Material.GetOpacity(uv).R == 0) continue;

                        ZBuffer[(int) zCoord] = z;

                        if (barycentricHelper.FirstEdge || barycentricHelper.SecondEdge || barycentricHelper.ThirdEdge)
                        {
                            if (renderMode == RenderMode.DepthOnly)
                            {
                                var ndc = z * 2.0f - 1.0f;
                                var linearDepth = 2.0f * Camera.Near * Camera.Far /
                                                  (Camera.Far +
                                                   Camera.Near -
                                                   ndc * (Camera.Far - Camera.Near));
                                // Console.WriteLine(z);
                                Buffer[(int) x, (int) y] = new FloatColor(linearDepth, linearDepth, linearDepth);
                                // Console.WriteLine(val);
                                
                                continue;
                            }


                            var lightColors = CalculateLight(triangle, worldCoords, mesh, barycentric, lightingMode);

                            var textureColor = mesh.Material.GetDiffuse(uv);
                            var emissiveColor = mesh.Material.GetEmissive(uv);
                            var color =
                                    lightColors.first * textureColor * barycentric.X +
                                    lightColors.second * textureColor * barycentric.Y +
                                    lightColors.third * textureColor * barycentric.Z + emissiveColor
                                    -
                                    ShadowFactor(
                                        worldCoords[0] * barycentric.X +
                                        worldCoords[1] * barycentric.Y +
                                        worldCoords[2] * barycentric.Z)
                                ;

                            Buffer[(int) x, (int) y] = color;//FloatColor.ToHdr(color, 0.1f,1f);
                        }
                    }
                }
            }
        }
    }
}