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
            var n = renderable.TransformNormals(vertex.Normal).NormalizeUnsafe();
            var v = renderable.ApplyView(-vertex.Position).NormalizeUnsafe();
            var r = Position.Reflect(n).NormalizeUnsafe();
            var diff = Saturate(Position.Dot(n));
            var spec = MathF.Pow(Saturate(Dot(r,v)), Shininess);

            return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne();
        }
    }
}