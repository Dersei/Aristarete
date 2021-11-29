using Aristarete.Basic.Textures;

namespace Aristarete.Basic.Materials
{
    public class PbrMaterial
    {
        public FloatColor Color;
        public TextureInfo? DiffuseMap;
        public TextureInfo? EmissiveMap;
        public TextureInfo? SpecularMap;
        public TextureInfo? NormalMap;
        public TextureInfo? OpacityMap;
        public TextureInfo? HeightMap;
        public float EmissionFactor = 1;

        public static readonly PbrMaterial Error = new(FloatColor.Error,
            emissiveMap: new TextureInfo(new ConstantColorTexture(FloatColor.Black)));


        public PbrMaterial(FloatColor color, TextureInfo? diffuseMap = null, TextureInfo? emissiveMap = null,
            TextureInfo? specularMap = null, TextureInfo? normalMap = null, TextureInfo? opacityMap = null, TextureInfo? heightMap = null)
        {
            Color = color;
            DiffuseMap = diffuseMap;
            EmissiveMap = emissiveMap;
            SpecularMap = specularMap;
            NormalMap = normalMap;
            OpacityMap = opacityMap;
            HeightMap = heightMap;
        }
        
        public FloatColor GetDiffuse(Float2 uv)
        {
            var texelColor = DiffuseMap?.GetColor(uv);
            if (texelColor != null) return texelColor.Value * Color;
            return Color;
        }
        
        public FloatColor GetHeight(Float2 uv)
        {
            var texelColor = HeightMap?.GetColor(uv);
            if (texelColor != null) return texelColor.Value * Color;
            return FloatColor.Black;
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
        
        public FloatColor GetOpacity(Float2 uv)
        {
            var texelColor = OpacityMap?.GetColor(uv);
            if (texelColor != null) return texelColor.Value;
            return FloatColor.White;
        }
        
        public Float3Sse GetNormals(Float2 uv)
        {
            var texelColor = NormalMap?.GetColor(uv);
            if (texelColor != null) return new Float3Sse(texelColor.Value.R * 2 - 1, texelColor.Value.G * 2 - 1, texelColor.Value.B * 2 - 1);
            return Float3Sse.Zero;
        }
    }
}