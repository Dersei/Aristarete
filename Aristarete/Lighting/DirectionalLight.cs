using System;
using Aristarete.Basic;
using Aristarete.Meshes;
using static Aristarete.Extensions.MathExtensions;
using static Aristarete.Basic.Float3;

namespace Aristarete.Lighting
{
    public class DirectionalLight : Light
    {
        public override FloatColor Calculate(Vertex vertex, IRenderable renderable)
        {
            var n = renderable.TransformNormals(vertex.Normal + renderable.Material.GetNormals(vertex.UV)).NormalizeUnsafe();
            var v = renderable.ApplyView(-vertex.Position).NormalizeUnsafe();
            var r = Position.Reflect(n);
            var diff = Saturate(Position.Dot(n));

            
            var spec = diff > 0 ? MathF.Pow(MathF.Max(Dot(v, r), 0), 8 * (1 - renderable.Material.GetSpecular(vertex.UV).R)) : 0;

            return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne() + renderable.Material.GetEmissive(vertex.UV);
        }
    }
}