using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Rendering;


namespace Aristarete.Meshes
{
    public interface IRenderable
    {
        void SetIdentity();
        IRenderable Rotate(float angle, Float3Sse v);
        IRenderable Translate(Float3Sse v);
        IRenderable Scale(Float3Sse v);
        IRenderable Scale(float v);
        void Transform();
        void Update(Rasterizer rasterizer, RenderMode renderMode = RenderMode.Color);
        public Float3Sse ApplyView(Float3Sse f);
        public Float3Sse Apply(Float3Sse f);
        public Float3Sse ApplyWorld(Float3Sse f) => Float3Sse.Zero;
        public Float3Sse TransformNormals(Float3Sse f) => Float3Sse.Zero;
        public PbrMaterial Material => PbrMaterial.Error;
    }
}