using System;
using Aristarete.Basic;
using Aristarete.Meshes;
using Aristarete.Rendering;
using Daeira;
using static Aristarete.Extensions.MathExtensions;
using static Daeira.Float3;

namespace Aristarete.Lighting
{
    public class PointLight : Light
    {
        public override FloatColor Calculate(Vertex vertex, Mesh renderable, Rasterizer rasterizer)
        {
            var n = renderable.TransformNormals(vertex.Normal).NormalizeUnsafe() +
                    renderable.Material.GetNormals(vertex.UV);
            n = n.NormalizeUnsafe();
            var v = vertex.Position;

            float distance = (Position - v).Length;
            float attenuation = 1.0f / (1.0f + 0.35f * distance + 0.44f * (distance * distance));   

            var l = (Position - v).NormalizeUnsafe();
            v = v.NormalizeUnsafe();
            var r = l.Reflect(n).NormalizeUnsafe();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(
                Saturate(Dot((rasterizer.Camera.Position - vertex.Position).NormalizeUnsafe(), -r)),
                Shininess * (1 - renderable.Material.GetSpecular(vertex.UV).R));

            return Saturate(Ambient * attenuation + Diffuse * diff * attenuation + Specular * spec * attenuation)
                .AlphaToOne();
        }

        public override FloatColor Calculate(DeferredData data, Rasterizer rasterizer)
        {
            var n = data.Normal;
            n = n.NormalizeUnsafe();
            var v = data.Position;
            float distance = (Position - v).Length;
            float attenuation = 1.0f / (1.0f + 0.35f * distance + 0.44f * (distance * distance)); 
            
            var l = (Position - v).NormalizeUnsafe();
            v = v.NormalizeUnsafe();
            var r = l.Reflect(n).NormalizeUnsafe();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(MathF.Max(Dot((rasterizer.Camera.Position - data.Position).NormalizeUnsafe(), r), 0),
                Shininess * (1 - data.Specular.R));

            return Saturate(Ambient * attenuation + Diffuse * diff * attenuation + Specular * spec * attenuation).AlphaToOne();
        }
    }
}