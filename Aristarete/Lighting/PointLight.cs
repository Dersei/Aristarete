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
            var n = renderable.TransformNormals(vertex.Normal).NormalizeUnsafe() + renderable.Material.GetNormals(vertex.UV);
            n = n.NormalizeUnsafe();
            var v = renderable.ApplyView(-vertex.Position);
            var l = (Position - v).NormalizeUnsafe();
            v = v.NormalizeUnsafe();
            var r = l.Reflect(n).NormalizeUnsafe();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(MathF.Max(Dot(v, r), 0),
                Shininess * (1 - renderable.Material.GetSpecular(vertex.UV).R));

            return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne();
        }
    }
}