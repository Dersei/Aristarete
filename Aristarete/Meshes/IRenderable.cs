using Aristarete.Basic.Materials;
using Aristarete.Rendering;
using Daeira;


namespace Aristarete.Meshes
{
    public interface IRenderable
    {
        void SetIdentity();
        IRenderable Rotate(float angle, Float3 v);
        IRenderable Translate(Float3 v);
        IRenderable Scale(Float3 v);
        IRenderable Scale(float v);
        void Transform();
        void Update(Rasterizer rasterizer, RenderMode renderMode = RenderMode.Color);
        public Float3 ApplyView(Float3 f);
        public Float3 Apply(Float3 f);
        public Float3 ApplyWorld(Float3 f) => Float3.Zero;
        public Float3 TransformNormals(Float3 f) => Float3.Zero;
        public PbrMaterial Material => PbrMaterial.Error;
    }
}