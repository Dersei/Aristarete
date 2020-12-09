using Aristarete.Basic;

namespace Aristarete
{
    public interface IRenderable
    {
        void SetIdentity();
        IRenderable Rotate(float angle, Float3 v);
        IRenderable Translate(Float3 v);
        IRenderable Scale(Float3 v);
        IRenderable Scale(float v);
        void Transform();
        void Update(Rasterizer rasterizer);
    }
}