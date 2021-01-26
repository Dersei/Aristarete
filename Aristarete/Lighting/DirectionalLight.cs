using System;
using Aristarete.Basic;
using Aristarete.Meshes;
using Aristarete.Rendering;
using static Aristarete.Extensions.MathExtensions;
using static Aristarete.Basic.Float3;

namespace Aristarete.Lighting
{
    public class DirectionalLight : Light
    {
        public override FloatColor Calculate(Vertex vertex, Mesh renderable, Rasterizer rasterizer)
        {
            var n = renderable.TransformNormals(vertex.Normal)
                .NormalizeUnsafe() + renderable.Material.GetNormals(vertex.UV);
            n = n.NormalizeUnsafe();
            var v = (rasterizer.Camera.Position - vertex.Position).NormalizeUnsafe();
            var r = (-Position).Reflect(n);
            var diff = Saturate(Position.Dot(n));
            var spec = diff > 0
                ? MathF.Pow(Saturate(Dot(v, r)), Shininess * (1 - renderable.Material.GetSpecular(vertex.UV).R))
                : 0;

            return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne();
        }
        
        public override FloatColor Calculate(DeferredData data, Rasterizer rasterizer)
        {
            var n = data.Normal;
            n = n.NormalizeUnsafe();
            var v = (rasterizer.Camera.Position - data.Position).NormalizeUnsafe();
            var r = Position.Reflect(n);
            var diff = Saturate(Position.Dot(n));
            var spec = diff > 0
                ? MathF.Pow(MathF.Max(Dot(v, r), 0), Shininess * (1 - data.Specular.R))
                : 0;

            return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne();
        }
    }
}