using System;
using Aristarete.Basic;
using Aristarete.Meshes;
using Aristarete.Rendering;
using static Aristarete.Extensions.MathExtensions;
using static Aristarete.Basic.Float3Sse;

namespace Aristarete.Lighting
{
    public class PointLight : Light
    {
        public override FloatColor Calculate(Vertex vertex, Mesh renderable, Rasterizer rasterizer)
        {
            var n = renderable.TransformNormals(vertex.Normal).NormalizeExact() +
                    renderable.Material.GetNormals(vertex.UV);
            n = n.NormalizeExact();
            var v = vertex.Position;

            var distance = (Position - v).Length;
            var attenuation = 1.0f / (1.0f + 0.35f * distance + 0.44f * (distance * distance));   

            var l = (Position - v).NormalizeExact();
            v = v.NormalizeExact();
            var r = l.Reflect(n).NormalizeExact();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(
                Saturate(Dot((rasterizer.Camera.Position - vertex.Position).NormalizeExact(), -r)),
                Shininess * (1 - renderable.Material.GetSpecular(vertex.UV).R));

            return Saturate(Ambient * attenuation + Diffuse * diff * attenuation + Specular * spec * attenuation)
                .AlphaToOne();
        }

        public override FloatColor Calculate(in DeferredData data, Rasterizer rasterizer)
        {
            var n = data.Normal;
            n = n.NormalizeExact();
            var v = data.Position;
            var distance = (Position - v).Length;
            var attenuation = 1.0f / (1.0f + 0.35f * distance + 0.44f * (distance * distance)); 
            
            var l = (Position - v).NormalizeExact();
            v = v.NormalizeExact();
            var r = l.Reflect(n).NormalizeExact();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(
                Saturate(Dot((rasterizer.Camera.Position - data.Position).NormalizeExact(), -r)),
                Shininess * (1 - data.Specular.R));

            return Saturate(Ambient * attenuation + Diffuse * diff * attenuation + Specular * spec * attenuation).AlphaToOne();
        }
    }
}