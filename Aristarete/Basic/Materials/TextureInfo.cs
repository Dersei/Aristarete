using Aristarete.Basic.Textures;

namespace Aristarete.Basic.Materials
{
    public class TextureInfo
    {
        public readonly ITexture Texture;
        public readonly Float2 Scale;
        public readonly Float2 Offset;
        
        public FloatColor GetColor(Float2 uv) => Texture.GetColor(uv, Scale, Offset);

        public TextureInfo(ITexture texture, Float2 scale, Float2 offset)
        {
            Texture = texture;
            Scale = scale;
            Offset = offset;
        }
        
        public TextureInfo(ITexture texture, float scale, Float2 offset)
        {
            Texture = texture;
            Scale = new Float2(scale, scale);
            Offset = offset;
        }
        
        public TextureInfo(ITexture texture)
        {
            Texture = texture;
            Scale = Float2.One;
            Offset = Float2.Zero;
        }
    }
}