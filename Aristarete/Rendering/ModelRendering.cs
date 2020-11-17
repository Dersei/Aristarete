using Aristarete.Basic;
using Aristarete.Inputting;
using Aristarete.Models;

namespace Aristarete.Rendering
{
    public class ModelRendering : IRendering
    {
        private float _angleLeft;
        private float _angleUp;
        private bool _switchCamera = true;
        private Float3 _scale = Float3.One;
        private Float3 _translate = Float3.Zero;
        private Float3 _eye = new Float3(0, 0, 2);
        private readonly Model _model = new Model("_Resources/crystal.obj");
        private readonly VertexProcessor _vertexProcessor = new VertexProcessor();

        public ModelRendering()
        {
            _vertexProcessor.SetPerspective(60, 1, 1, 1000);
            _vertexProcessor.SetLookAt(_eye, new Float3(0, 0, 0), Float3.Up);
            _vertexProcessor.SetIdentity();
        }

        public void Run(Rasterizer rasterizer)
        {
            if (Input.IsCurrentPress(Keys.Q))
            {
                _angleLeft += 15f;
            }
            else if (Input.IsCurrentPress(Keys.E))
            {
                _angleLeft -= 15f;
            }
            else if (Input.IsCurrentPress(Keys.W))
            {
                _angleUp += 15f;
            }
            else if (Input.IsCurrentPress(Keys.S))
            {
                _angleUp -= 15f;
            }
            else if (Input.IsNewPress(Keys.Z))
            {
                _scale /= 2;
            }
            else if (Input.IsNewPress(Keys.X))
            {
                _scale *= 2;
            }
            else if (Input.IsCurrentPress(Keys.A))
            {
                _translate += Float3.Left / 10f;
            }
            else if (Input.IsCurrentPress(Keys.D))
            {
                _translate += Float3.Right / 10f;
            }
            else if (Input.IsNewPress(Keys.Space))
            {
                _switchCamera = !_switchCamera;
            }

            _vertexProcessor.SetIdentity();
            _vertexProcessor.SetIdentityToView();
            _vertexProcessor.SetLookAt(_switchCamera ? new Float3(0, 0, 2) : new Float3(0, -3, 0), new Float3(0, 0, 0),
                _switchCamera ? Float3.Up : Float3.Forward);
            
            _vertexProcessor.Scale(_scale);
            _vertexProcessor.Translate(_translate);
            _vertexProcessor.Rotate(_angleLeft, Float3.Up);
            _vertexProcessor.Rotate(_angleUp, Float3.Left);
            _vertexProcessor.Translate(_translate);
            _vertexProcessor.Transform();
            _vertexProcessor.SetIdentity();

            _model.ColorAngle = FloatColor.White;
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

            _vertexProcessor.Scale(Float3.One / 2f);
            _vertexProcessor.Translate(Float3.Left);
            _vertexProcessor.Transform();
            _vertexProcessor.SetIdentity();

            _model.ColorAngle = FloatColor.Blue;
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

            _vertexProcessor.Scale(Float3.One);
            _vertexProcessor.Rotate(45, Float3.Forward);
            _vertexProcessor.Translate(Float3.Right + Float3.Back);
            _vertexProcessor.Transform();
            _vertexProcessor.SetIdentity();

            _model.ColorAngle = FloatColor.Red;
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
            
            _vertexProcessor.Scale(Float3.One/4);
            _vertexProcessor.Rotate(180, Float3.Forward);
            _vertexProcessor.Translate(Float3.Forward/5);
            _vertexProcessor.Transform();

            _model.ColorAngle = FloatColor.Blue;
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