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
            var n = renderable.TransformNormals(vertex.Normal).Normalize();
            var v = renderable.ApplyView(-vertex.Position);
            var l = (Position - v).Normalize();
            v = Normalize(v);
            var r = Normalize(l.Reflect(n));
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(Saturate(Dot(r, v)), Shininess);

            return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne();
        }
    }
}