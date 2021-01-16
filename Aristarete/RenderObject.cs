using Aristarete.Basic;
using Aristarete.Meshes;
using Aristarete.Models;

namespace Aristarete
{
    public class RenderObject : Mesh
    {
        private readonly Model _model;

        public RenderObject(VertexProcessor vertexProcessor, Model model, FloatColor basicColor = default) : base(vertexProcessor, basicColor)
        {
            _model = model;
            Material = model.Material;
        }
        
        public override void Update(Rasterizer rasterizer)
        {
            if (IsDirty)
            {
                Transform();
                SetIdentity();
            }


            for (var i = 0; i < _model.Triangles.Count; i++)
            {
                var triangle = _model.Triangles[i];
                var screenCoords = new Float3[3];
                
                for (var j = 0; j < 3; j++)
                {
                    var v = triangle[j];
                    screenCoords[j] = Apply(v.Position);
                }
                
                rasterizer.Triangle(new Triangle(
                        triangle[0],
                        triangle[1],
                        triangle[2]
                    ),
                    screenCoords,
                    this, LightingMode);
            }
        }
    }
}