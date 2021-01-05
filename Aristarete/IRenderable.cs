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
        public Float3 ApplyView(Float3 f);
        public Float3 Apply(Float3 f);
        public Float3 TransformNormals(Float3 f) => Float3.Zero;
    }
}