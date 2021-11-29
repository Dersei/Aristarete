using System.Numerics;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;

namespace Aristarete.Extensions
{
    public static class TextureExtensions
    {
        public static TextureInfo ToInfo(this ITexture texture) => new TextureInfo(texture);
        public static TextureInfo ToInfo(this ITexture texture, float scale) => new TextureInfo(texture, scale, Float2.Zero);
        public static TextureInfo ToInfo(this ITexture texture, float scale, Float2 offset) => new TextureInfo(texture, scale, offset);
        public static TextureInfo ToInfo(this ITexture texture, Float2 scale, Float2 offset) => new TextureInfo(texture, scale, offset);
    }
}