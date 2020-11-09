using Aristarete.Basic;
using Aristarete.Inputting;
using Aristarete.Models;

namespace Aristarete.Rendering
{
    public class ModelRendering : IRendering
    {
        private float _angleLeft;
        private float _angleUp;
        private bool _isRotating;
        private readonly Model _model = new Model("_Resources/crystal.obj");
        private readonly VertexProcessor _vertexProcessor = new VertexProcessor();

        public ModelRendering()
        {
            _vertexProcessor.SetPerspective(30, 1, 1, 1000);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 2), new Float3(0, 0, -1), Float3.Up);
            _vertexProcessor.SetIdentity();
        }

        public void Run(Rasterizer rasterizer)
        {
            if (Input.IsNewPress(Keys.A))
            {
                _angleLeft += 1f;
            }
            else if (Input.IsNewPress(Keys.D))
            {
                _angleLeft -= 1f;
            }

            if (Input.IsNewPress(Keys.W))
            {
                _angleUp += 1f;
            }
            else if (Input.IsNewPress(Keys.S))
            {
                _angleUp -= 1f;
            }
            else if (Input.IsNewPress(Keys.R))
            {
                _isRotating = !_isRotating;
            }

            if (!_isRotating) _vertexProcessor.SetIdentity();
            
            _vertexProcessor.Rotate(_angleLeft, Float3.Up);
            _vertexProcessor.Rotate(_angleUp, Float3.Left);
            _vertexProcessor.Transform();
            
            for (var i = 0; i < _model.Faces.Count; i++)
            {
                var face = _model.GetFace(i);
                var screenCoords = new Float3[3];
                var worldCoords = new Float3[3];

                for (var j = 0; j < 3; j++)
                {
                    var v = _model.Vertices[face[j]];
                    screenCoords[j] = _vertexProcessor.Apply(v);
                    worldCoords[j] = v;
                }
                var uv = new Float2[3];
                for (var k = 0; k < 3; k++)
                {
                    uv[k] = _model.GetUV(i, k);
                }
                rasterizer.Triangle(screenCoords, uv, _model);
            }
        }
    }
}