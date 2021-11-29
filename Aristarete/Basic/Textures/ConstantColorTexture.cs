using Daeira;

namespace Aristarete.Basic.Textures
{
    public class ConstantColorTexture : ITexture
    {
        public readonly FloatColor Color;

        public ConstantColorTexture(FloatColor color) => Color = color;
        
        public FloatColor GetColor(Float2 uv) => Color;

        public FloatColor GetColor(Float2 uv, Float2 scale, Float2 offset) => GetColor(uv);
    }
}