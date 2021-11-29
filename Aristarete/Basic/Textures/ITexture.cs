using Daeira;

namespace Aristarete.Basic.Textures
{
    public interface ITexture
    {
        public FloatColor GetColor(Float2 uv, Float2 scale, Float2 offset) => FloatColor.Error;
        public FloatColor GetColor(Float2 uv) => FloatColor.Error;
    }
}