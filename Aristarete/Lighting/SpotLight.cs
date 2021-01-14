using System;
using Aristarete.Basic;
using Aristarete.Meshes;
using static Aristarete.Extensions.MathExtensions;
using static Aristarete.Basic.Float3;

namespace Aristarete.Lighting
{
    public class SpotLight : Light
    {

        public Float3 Direction;
        public float Angle = 10;
        public override FloatColor Calculate(Vertex vertex, IRenderable renderable)
        {
            var n = renderable.TransformNormals(vertex.Normal + renderable.Material.GetNormals(vertex.UV)).NormalizeUnsafe();
            var v = renderable.ApplyView(vertex.Position);
            var l = (Position - v).NormalizeUnsafe();

            v = v.NormalizeUnsafe();
            var r = l.Reflect(n).NormalizeUnsafe();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(MathF.Max(Dot(v, r), 0), Shininess * (1 - renderable.Material.GetSpecular(vertex.UV).R));

            var spotFactor = Dot(l, Direction.NormalizeUnsafe());
            if (spotFactor > MathF.Cos(Angle * Deg2Rad))
                return Saturate(Ambient + Diffuse * diff + Specular * spec).AlphaToOne() + renderable.Material.GetEmissive(vertex.UV);
            return Ambient;
        }
    }
}