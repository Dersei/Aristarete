using Aristarete.Basic;
using Aristarete.Rendering;

namespace Aristarete.Meshes.Models
{
    public sealed class RenderObject : Mesh
    {
        private readonly Model _model;

        public RenderObject(Model model, FloatColor basicColor = default) : base(basicColor)
        {
            _model = model;
            Material = model.Material;
            CreateTriangles();
        }
        
        protected override Mesh CreateTriangles()
        {
            Triangles = _model.Triangles;
            return this;
        }

        public override void Update(Rasterizer rasterizer)
        {
            if (IsDirty)
            {
                Transform(rasterizer);
                UpdateCoords();
            }
            //
            // for (var i = 0; i < Triangles.Count; i++)
            // {
            //     rasterizer.Triangle(Triangles[i], ScreenCoords[i], this, LightingMode, renderMode);
            // }

            if (LiveUpdate) SetIdentity();
        }
    }
}