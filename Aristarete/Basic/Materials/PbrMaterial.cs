using Aristarete.Basic.Textures;

namespace Aristarete.Basic.Materials
{
    public class PbrMaterial
    {
        public FloatColor Color;
        public TextureInfo? DiffuseMap;
        public float DiffuseCoefficient;
        public float Specular;
        public float SpecularExponent;
        public float AmbientPower;
        public TextureInfo? EmissiveMap;
        public TextureInfo? SpecularMap;
        public TextureInfo? NormalMap;
        public float EmissionFactor = 1;

        public static readonly PbrMaterial Error = new(FloatColor.Error,
            emissiveMap: new TextureInfo(new ConstantColorTexture(FloatColor.Error)));


        public PbrMaterial(FloatColor color, TextureInfo? diffuseMap = null, TextureInfo? emissiveMap = null,
            TextureInfo? specularMap = null, TextureInfo? normalMap = null)
        {
            Color = color;
            DiffuseMap = diffuseMap;
            EmissiveMap = emissiveMap;
            SpecularMap = specularMap;
            NormalMap = normalMap;
            DiffuseCoefficient = 1;
            Specular = 0;
            SpecularExponent = 50;
            AmbientPower = 1f;
        }

        private FloatColor GetResultColor(FloatColor lightColor, float lightIntensity, FloatColor? texelColor,
            float diffuseFactor)
        {
            if (texelColor != null)
                return lightColor * lightIntensity * AmbientPower * texelColor.Value * Color * diffuseFactor *
                       DiffuseCoefficient;
            return lightColor * lightIntensity * AmbientPower * Color * diffuseFactor * DiffuseCoefficient;
        }

        public FloatColor GetDiffuse(Float2 uv)
        {
            var texelColor = DiffuseMap?.GetColor(uv);
            if (texelColor != null) return texelColor.Value;
            return FloatColor.Error;
        }
        
        public FloatColor GetSpecular(Float2 uv)
        {
            var texelColor = SpecularMap?.GetColor(uv);
            if (texelColor != null) return texelColor.Value;
            return FloatColor.Black;
        }

        public FloatColor GetEmissive(Float2 uv)
        {
            var texelColor = EmissiveMap?.GetColor(uv);
            if (texelColor != null) return texelColor.Value * EmissionFactor;
            return FloatColor.Black;
        }

        public Float3 GetNormals(Float2 uv)
        {
            var texelColor = NormalMap?.GetColor(uv);
            if (texelColor != null) return new Float3(texelColor.Value.R * 2 - 1, texelColor.Value.G * 2 - 1, texelColor.Value.B * 2 - 1);
            return Float3.Zero;
        }
    }
}