using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Daeira;
using static Daeira.Extensions.MathExtensions;


namespace Aristarete.Rendering
{
    public class Raymarcher : Scene
    {
        private Stopwatch _timer = new();
        private float _time;
        private float _cosTime;
    
        public Raymarcher()
        {
            _timer.Start();
        }
    
        private static float SmoothMinLerp(float a, float b, float k)
        {
            var h = Clamp(0.5f + 0.5f * (b - a) / k, 0.0f, 1.0f);
            return Lerp(b, a, h) - k * h * (1.0f - h);
        }
    
        private Float3 DistanceFieldNormal(in Float3 pos)
        {
            const float eps = 0.001f;
            var d = SignedDistance(pos);
            var nx = SignedDistance(pos + new Float3(eps, 0, 0)) - d;
            var ny = SignedDistance(pos + new Float3(0, eps, 0)) - d;
            var nz = SignedDistance(pos + new Float3(0, 0, eps)) - d;
            return new Float3(nx, ny, nz).Normalize();
        }
    
        private float SignedDistance(in Float3 p)
        {
            var s1 = p.Length - 0.5f;
            var s2 = (p + new Float3(_cosTime, _cosTime, 0)).Length - 0.2f;
            var s3 = (p + new Float3(_cosTime, -_cosTime, 0)).Length - 0.2f;
            return SmoothMinLerp(SmoothMinLerp(s1, s2, 1), s3, 1);
        }
    
        private bool SphereTrace(in Float3 orig, in Float3 dir, out Float3 pos)
        {
            var dot = Float3.Dot(orig, dir);
            if (orig.LengthSquared - dot * dot > 2.5f)
            {
                pos = default;
                return false;
            }
    
            pos = orig;
    
            for (var i = 0; i < 16; i++)
            {
                var d = SignedDistance(pos);
                if (d < 0.001f) return true;
                pos += dir * d;
            }
    
            return false;
        }
    
        private Float3[]? _lookup;
    
        [MemberNotNull(nameof(_lookup))]
        private void GenerateLookup(int width, int height, float fov)
        {
            if (_lookup is not null) return;
    
            _lookup = new Float3[width * height];
    
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var dirX = i + 0.5f - width / 2.0f;
                    var dirY = -(j + 0.5f) + height / 2.0f; // this flips the image at the same time
                    var dirZ = -height / (2.0f * MathF.Tan(fov / 2.0f));
    
                    _lookup[i + j * width] = new Float3(dirX, dirY, dirZ).Normalize();
                }
            }
        }
    
        private FloatColor PostEffect(Float3 endPos)
        {
            var s1 = endPos.Length - 0.5f;
            var s2 = (endPos + new Float3(_cosTime, _cosTime, 0)).Length - 0.2f;
            var s3 = (endPos + new Float3(_cosTime, -_cosTime, 0)).Length - 0.2f;
            var a = Float4.Normalize(new Float4(1.0f / s1, 1.0f / s2, 1.0f / s3, 1.0f));
            var albedo =
                FloatColor.Green * a.X +
                FloatColor.Blue * a.Y +
                FloatColor.Red * a.Z +
                FloatColor.UnityYellow * a.W;
            return albedo;
        }
    
      public override void Run()
        {
            var width = Rasterizer.Buffer.Width;
            var height = Rasterizer.Buffer.Height;
            const float fov = MathF.PI / 3.0f;
            var cameraOrigin = Float3.Forward * 5;
            var lightDirection = (Float3.Up + Float3.Forward * 5).Normalize();
            _time = (float) _timer.Elapsed.TotalSeconds;
            _cosTime = MathF.Cos(_time);
            GenerateLookup(width, height, fov);
    
            Parallel.For(0, height, j =>
            {
                for (var i = 0; i < width; i++)
                {
                    if (SphereTrace(cameraOrigin, _lookup[i + j * width], out var hit))
                    {
                        var lightIntensity = Saturate(Float3.Dot(lightDirection, DistanceFieldNormal(hit)));
                        var resultColor = PostEffect(hit) * lightIntensity;
                        Rasterizer.Buffer[i, j] = new FloatColor(resultColor.R, resultColor.G, resultColor.B);
                    }
                }
            });
    
            // for (int i = 0; i < width; i++)
            // {
            //     for (int j = 0; j < height; j++)
            //     {
            //         var dirX = i + 0.5f - width / 2.0f;
            //         var dirY = -(j + 0.5f) + height / 2.0f; // this flips the image at the same time
            //         var dirZ = -height / (2.0f * MathF.Tan(fov / 2.0f));
            //         if (SphereTrace(Float3.Forward * 5, new Float3(dirX, dirY, dirZ).Normalize(), out var hit))
            //         {
            //             var lightDir = (Float3.Up + Float3.Forward * 5).Normalize();
            //             var lightIntensity = Saturate(Float3.Dot(lightDir, DistanceFieldNormal(hit))) * 5;
            //             rasterizer.Buffer[i, j] = (hit * lightIntensity).ToFloatColor();
            //         }
            //     }
            // }
        }
    }
}