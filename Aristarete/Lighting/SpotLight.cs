using System;
using Aristarete.Basic;
using Aristarete.Meshes;
using Aristarete.Rendering;
using static Aristarete.Extensions.MathExtensions;
using static Aristarete.Basic.Float3Sse;

namespace Aristarete.Lighting
{
    public class SpotLight : Light
    {
        public Float3Sse Direction;

        public float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                SetCos(value, _outerConeAngle);
            }
        }

        public float OuterConeAngle
        {
            get => _outerConeAngle;
            set
            {
                _outerConeAngle = value;
                SetCos(_angle, value);
            }
        }

        private float _cosAngle;
        private float _cosOuterConeAngle;
        private float _angle = 5;
        private float _outerConeAngle = 6;

        private void SetCos(float angle, float outerAngle)
        {
            _cosAngle = MathF.Cos(angle * Deg2Rad);
            _cosOuterConeAngle = MathF.Cos(outerAngle * Deg2Rad);
        }

        public override FloatColor Calculate(Vertex vertex, Mesh renderable, Rasterizer rasterizer)
        {
            var n = renderable.TransformNormals(vertex.Normal).NormalizeExact() + renderable.Material.GetNormals(vertex.UV);
            n = n.NormalizeExact();
            var v = vertex.Position;
            var l = (Position - v).NormalizeExact();

            //v = v.NormalizeUnsafe();
            var r = l.Reflect(n).NormalizeExact();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(Saturate(Dot((rasterizer.Camera.Position - vertex.Position).NormalizeExact(), -r)),
                Shininess * (1 - renderable.Material.GetSpecular(vertex.UV).R));

            var innerMinusOuter = _cosAngle - _cosOuterConeAngle;

            var spotFactor = Dot(l, Direction.Normalize());

            var spot = Saturate((spotFactor - _cosOuterConeAngle) / innerMinusOuter);

            return Saturate(Ambient + Diffuse * diff * spot + Specular * spec * spot).AlphaToOne();
        }
        
        public override FloatColor Calculate(in DeferredData data, Rasterizer rasterizer)
        {
            var n = data.Normal;
            n = n.NormalizeExact();
            var v = data.Position;
            var l = (Position - v).NormalizeExact();

            //v = v.NormalizeUnsafe();
            var r = l.Reflect(n).NormalizeExact();
            var diff = Saturate(l.Dot(n));
            var spec = MathF.Pow(Saturate(Dot((rasterizer.Camera.Position - data.Position).NormalizeExact(), -r)),
                Shininess * (1 - data.Specular.R));

            var innerMinusOuter = _cosAngle - _cosOuterConeAngle;

            var spotFactor = Dot(l, Direction.Normalize());

            var spot = Saturate((spotFactor - _cosOuterConeAngle) / innerMinusOuter);

            return Saturate(Ambient + Diffuse * diff * spot + Specular * spec * spot).AlphaToOne();
        }
    }
}