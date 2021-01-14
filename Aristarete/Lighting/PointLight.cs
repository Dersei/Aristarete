using System;
using Aristarete.Basic;
using Aristarete.Meshes;
using static Aristarete.Extensions.MathExtensions;
using static Aristarete.Basic.Float3;

namespace Aristarete.Lighting
{
    public class PointLight : Light
    {
        public override FloatColor Calculate(Vertex vertex, IRenderable renderable)
        {
            var n = renderable.TransformNormals(vertex.Normal* renderable.Material.GetNormals(vertex.UV)).NormalizeUnsafe();
          //  n = (n * (renderable.Material as PbrMaterial).GetNormals(vertex.UV)).Normalize();
            var v = renderable.ApplyView(-vertex.Position);
            var l = (Position - v).NormalizeUnsafe();
            v = v.NormalizeUnsafe();
            var r = l.Reflect(n).NormalizeUnsafe();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(Saturate(Dot(r, v)), Shininess);

            return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne() + renderable.Material.GetEmissive(vertex.UV);
        }
    }
}