using System.Collections.Generic;
using Aristarete.Basic;
using Aristarete.Inputting;
using Aristarete.Meshes;

namespace Aristarete.Rendering
{
    public class GeneratedRendering : IRendering
    {
        private float _angleLeft;
        private float _angleUp;
        private float _oldAngleLeft;
        private float _oldAngleUp;
        private bool _isStopped;
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly List<IRenderable> _meshes = new();

        public GeneratedRendering()
        {
            _vertexProcessor.SetPerspective(45, 2, 1, 1000);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, -1), Float3.Up);
            _meshes.Add(new Pyramid(_vertexProcessor) {BasicColor = FloatColor.Red}
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Left + Float3.Left / 2));
            _meshes.Add(new Cone(_vertexProcessor) {BasicColor = FloatColor.UnityYellow}
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Left / 2));
            _meshes.Add(new Sphere(_vertexProcessor) {BasicColor = FloatColor.Green}
                .Scale(0.3f).Rotate(45, Float3.Left).Translate(Float3.Right / 2));
            _meshes.Add(new Cylinder(_vertexProcessor) {BasicColor = FloatColor.Blue}
                .Scale(0.3f).Rotate(45, Float3.Left).Translate(2 * Float3.Left + Float3.Left / 2));
            _meshes.Add(new Torus(_vertexProcessor) {BasicColor = FloatColor.Magenta}
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Right + Float3.Right / 2));
            _meshes.Add(new Tube(_vertexProcessor) {BasicColor = FloatColor.White}
                .Scale(0.3f).Rotate(70, Float3.Left).Translate(2 * Float3.Right + Float3.Right / 2));
            _meshes.Add(new Cube(_vertexProcessor) {BasicColor = FloatColor.Grey}
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(3 * Float3.Right + Float3.Right / 2));
            _meshes.Add(new Plane(_vertexProcessor) {BasicColor = FloatColor.Cyan}
                .Scale(0.3f).Rotate(70, Float3.Left).Translate(3 * Float3.Left + Float3.Left / 2));
        }

        public void Run(Rasterizer rasterizer)
        {
            if (Input.IsNewPress(Keys.A))
            {
                _angleLeft += 5f;
            }
            else if (Input.IsNewPress(Keys.D))
            {
                _angleLeft -= 5f;
            }
            else if (Input.IsNewPress(Keys.W))
            {
                _angleUp += 5f;
            }
            else if (Input.IsNewPress(Keys.S))
            {
                _angleUp -= 5f;
            }
            else if (Input.IsNewPress(Keys.Space))
            {
                if (_isStopped)
                {
                    _angleLeft = _oldAngleLeft;
                    _angleUp = _oldAngleUp;
                    _isStopped = false;
                }
                else
                {
                    _oldAngleLeft = _angleLeft;
                    _oldAngleUp = _angleUp;
                    _angleLeft = 0;
                    _angleUp = 0;
                    _isStopped = true;
                }
            }

            foreach (var mesh in _meshes)
            {
                mesh.Rotate(_angleLeft, Float3.Up);
                mesh.Rotate(_angleUp, Float3.Left);
                mesh.Update(rasterizer);
            }
        }
    }
}