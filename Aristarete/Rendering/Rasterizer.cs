using System;
using System.Runtime.CompilerServices;
using Aristarete.Basic;
using Aristarete.Cameras;
using Aristarete.Meshes;


namespace Aristarete.Rendering
{
    public abstract class Rasterizer
    {
        public Buffer Buffer { get; }

        protected readonly int Width;
        protected readonly int Height;
        public readonly float[] ZBuffer;
        public Scene Scene;
        public Camera Camera;

        protected Rasterizer(Buffer buffer, Scene scene, Camera camera)
        {
            Buffer = buffer;
            Scene = scene;
            Camera = camera;
            Width = buffer.Width;
            Height = buffer.Height;
            ZBuffer = new float[Width * Height];
            Array.Fill(ZBuffer, float.MaxValue);
        }

        public void CreateShadowMaps(bool update = false)
        {
            if (!update) return;
            foreach (var light in Scene.Lights)
            {
                if (light.ShadowMap is null)
                {
                    light.CreateShadowMap();
                }

                light.ShadowMap?.Render();
            }
        }
        
        protected FloatColor ShadowFactor(Float3Sse pointWorld)
        {
            var shadow = 0.0f; // No shadow     

            foreach (var light in Scene.Lights)
            {
                if (light.ShadowMap is not null)
                {
                    shadow += light.ShadowMap.PointInShadow(pointWorld);
                }
            }

            return new FloatColor(shadow, shadow, shadow);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Float3Sse ToBufferCoords(Float3Sse originalCoord)
        {
            return new((int) ((originalCoord.X + 1.0f) * Width * 0.5f),
                (int) ((-originalCoord.Y + 1.0f) * Height * 0.5f), originalCoord.Z);
        }

        protected bool IsOutsideFrustum(float depth) => depth < Camera.Near || depth > Camera.Far;


        public abstract void Triangle(in Triangle triangle, Float3Sse[] screenCords, Float3Sse[] worldCoords, Mesh mesh, LightingMode lightingMode,
            RenderMode renderMode = RenderMode.Color);

        public abstract void Render(RenderMode renderMode = RenderMode.Color);

        protected void DrawTriangle(Triangle triangle)
        {
            for (var i = 0; i < 3; ++i)
            {
                var p0 = new Int2((int) triangle[i % 3].Position.X, (int) triangle[i % 3].Position.Y);
                var p1 = new Int2((int) triangle[(i + 1) % 3].Position.X, (int) triangle[(i + 1) % 3].Position.Y);
                DrawLine(p0, p1);
            }
        }

        private Float2 ViewportTransformInv(Float2 pointRaster)
        {
            var slopeX = 2.0f / Width;
            var slopeY = 2.0f / Height;

            var pointNdc = new Float2(
                -1 + slopeX * pointRaster.X,
                -1 + slopeY * pointRaster.Y
            );

            return pointNdc;
        }

        protected Float3Sse Unrasterize(Float2 pointRaster, float depthFromCamera)
        {
            var pointNdc = ViewportTransformInv(pointRaster);
            var pointCamera = Camera.ProjectTransformInv(pointNdc, depthFromCamera);
            var pointWorld = Camera.ViewTransformInv(pointCamera);
            return pointWorld;
        }

        public Float3Sse Rasterize(Float3Sse pointWorld)
        {
            var pointCamera = Camera.ViewTransform(pointWorld);
            var pointNdc = Camera.ProjectTransform(pointCamera);
            var pointRaster = ToBufferCoords(pointNdc);
            return pointRaster;
        }

// World to raster/screen space
        public Triangle Rasterize(Triangle triangleWorld)
        {
            return new Triangle(
                Rasterize(triangleWorld.First.Position),
                Rasterize(triangleWorld.Second.Position),
                Rasterize(triangleWorld.Third.Position)
            );
        }

        private void DrawLine(Int2 p0, Int2 p1)
        {
            var color = new FloatColor(1.0f, 0.0f, 0);

            var steep = false;
            if (Math.Abs(p0.X - p1.X) < Math.Abs(p0.Y - p1.Y))
            {
                p0 = new Int2(p0.Y, p0.X);
                p1 = new Int2(p1.Y, p1.X);
                steep = true;
            }

            if (p0.X > p1.X)
                (p0, p1) = (p1, p0);

            //Always map to one octant, either (I or VIII) with derivative/slope [-1, 1]

            var dx = p1.X - p0.X;
            var dy = p1.Y - p0.Y;
            var dydx = (float) dy / dx;

            var totalError = 0;
            //This deals with whether octant I or VIII
            var dError = dydx > 0 ? +1 : -1;
            var error = 2 * dy;

            int x;
            var y = p0.Y;
            for (x = p0.X; x < p1.X; ++x)
            {
                if (!steep)
                {
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                    {
                        Buffer[x, y] = color;
                    }
                }
                else
                {
                    if (x >= 0 && x < Height && y >= 0 && y < Width)
                    {
                        Buffer[y, x] = color;
                    }
                }

                totalError += error;
                if (Math.Abs(totalError) > dx)
                {
                    y += dError;
                    totalError -= 2 * dx * dError;
                }
            }
        }


        protected void DrawEllipseCentered(int xc, int yc, int xr, int yr, FloatColor color)
        {
            var w = Width;
            var h = Height;

            // Avoid endless loop
            if (xr < 1 || yr < 1)
            {
                return;
            }

            // Init vars
            int uy, ly, lx, rx;
            var x = xr;
            var y = 0;
            var xrSqTwo = (xr * xr) << 1;
            var yrSqTwo = (yr * yr) << 1;
            var xChg = yr * yr * (1 - (xr << 1));
            var yChg = xr * xr;
            var err = 0;
            var xStopping = yrSqTwo * xr;
            var yStopping = 0;

            // Draw first set of points counter clockwise where tangent line slope > -1.
            while (xStopping >= yStopping)
            {
                // Draw 4 quadrant points at once
                uy = yc + y; // Upper half
                ly = yc - y; // Lower half

                rx = xc + x;
                lx = xc - x;

                if (0 <= uy && uy < h)
                {
                    if (0 <= rx && rx < w) Buffer[rx, uy] = color; // Quadrant I (Actually an octant)
                    if (0 <= lx && lx < w) Buffer[lx, uy] = color; // Quadrant II
                }

                if (0 <= ly && ly < h)
                {
                    if (0 <= lx && lx < w) Buffer[lx, ly] = color; // Quadrant III
                    if (0 <= rx && rx < w) Buffer[rx, ly] = color; // Quadrant IV
                }

                y++;
                yStopping += xrSqTwo;
                err += yChg;
                yChg += xrSqTwo;
                if (xChg + (err << 1) > 0)
                {
                    x--;
                    xStopping -= yrSqTwo;
                    err += xChg;
                    xChg += yrSqTwo;
                }
            }

            // ReInit vars
            x = 0;
            y = yr;
            uy = yc + y; // Upper half
            ly = yc - y; // Lower half
            xChg = yr * yr;
            yChg = xr * xr * (1 - (yr << 1));
            err = 0;
            xStopping = 0;
            yStopping = xrSqTwo * yr;

            // Draw second set of points clockwise where tangent line slope < -1.
            while (xStopping <= yStopping)
            {
                // Draw 4 quadrant points at once
                rx = xc + x;
                if (0 <= rx && rx < w)
                {
                    if (0 <= uy && uy < h) Buffer[rx, uy] = color; // Quadrant I (Actually an octant)
                    if (0 <= ly && ly < h) Buffer[rx, ly] = color; // Quadrant IV
                }

                lx = xc - x;
                if (0 <= lx && lx < w)
                {
                    if (0 <= uy && uy < h) Buffer[lx, uy] = color; // Quadrant II
                    if (0 <= ly && ly < h) Buffer[lx, ly] = color; // Quadrant III
                }

                x++;
                xStopping += yrSqTwo;
                err += xChg;
                xChg += yrSqTwo;
                if (yChg + (err << 1) > 0)
                {
                    y--;
                    uy = yc + y; // Upper half
                    ly = yc - y; // Lower half
                    yStopping -= xrSqTwo;
                    err += yChg;
                    yChg += xrSqTwo;
                }
            }
        }

        protected readonly struct BarycentricHelper
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

            public BarycentricHelper(in Float3Sse v1, in Float3Sse v2, in Float3Sse v3)
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
                // FirstEdge = Dy12 < 0 || Dx12 < 0;
                // SecondEdge = Dy23 < 0 || Dx23 < 0;
                // ThirdEdge = Dy31 < 0 || Dx31 < 0;
            }

            public Float3Sse Calculate(float x, float y)
            {
                var a = (Dy23 * (x - V3X) + Dx32 * (y - V3Y)) * MultiplierA;
                var b = (Dy31 * (x - V3X) + Dx13 * (y - V3Y)) * MultiplierB;
                var c = 1 - a - b;

                return new Float3Sse(a, b, c);
            }
        }

        public void Clear()
        {
            Buffer.Clear(FloatColor.Black);
            Array.Fill(ZBuffer, float.MaxValue);
        }
    }
}