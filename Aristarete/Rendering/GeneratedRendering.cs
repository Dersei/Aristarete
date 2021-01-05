using System.Collections.Generic;
using Aristarete.Basic;
using Aristarete.Inputting;
using Aristarete.Lighting;
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
            _vertexProcessor.SetPerspective(45, 2, 0.1f, 100);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, 0), Float3.Up);
            _meshes.Add(new Pyramid(_vertexProcessor) {BasicColor = FloatColor.Red}.CreateNormals()
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Right / 2 + Float3.Up));

            _meshes.Add(new Cone(_vertexProcessor) {BasicColor = FloatColor.UnityYellow}.CreateNormals()
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(1.5f * Float3.Right + Float3.Up));

            _meshes.Add(new Sphere(_vertexProcessor) {BasicColor = FloatColor.Green}.CreateNormals()
                .Scale(0.3f).Translate(Float3.Left / 2 + Float3.Up));

            _meshes.Add(new Sphere(_vertexProcessor, 1, 8, 6) {BasicColor = FloatColor.White}.CreateNormals()
                .Scale(0.6f).Translate(Float3.Left));
            _meshes.Add(new Sphere(_vertexProcessor, 1, 8, 6) {BasicColor = FloatColor.White, VertexLight = true}.CreateNormals()
                .Scale(0.6f).Translate(Float3.Right));
            _meshes.Add(new Sphere(_vertexProcessor, 1, 8, 6) {BasicColor = FloatColor.White}.CreateNormals()
                .Scale(0.6f).Translate(Float3.Right * 3));
            _meshes.Add(new Sphere(_vertexProcessor, 1, 8, 6) {BasicColor = FloatColor.White, VertexLight = true}.CreateNormals()
                .Scale(0.6f).Translate(Float3.Left * 3));

            _meshes.Add(new Cylinder(_vertexProcessor) {BasicColor = FloatColor.Blue}.CreateNormals()
                .Scale(0.3f).Rotate(45, Float3.Left).Translate(1.5f * Float3.Left + Float3.Up));

            _meshes.Add(new Torus(_vertexProcessor) {BasicColor = FloatColor.Magenta}.CreateNormals()
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Right / 2 + Float3.Down));

            _meshes.Add(new Tube(_vertexProcessor) {BasicColor = FloatColor.White}.CreateNormals()
                .Scale(0.3f).Rotate(90, Float3.Right).Translate(1.5f * Float3.Right + Float3.Down));

            _meshes.Add(new Cube(_vertexProcessor) {BasicColor = FloatColor.Grey}.CreateNormals()
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Left / 2 + Float3.Down));

            _meshes.Add(new Plane(_vertexProcessor) {BasicColor = FloatColor.Cyan}.CreateNormals()
                .Scale(0.3f).Rotate(90, Float3.Forward).Translate(1.5f * Float3.Left + Float3.Down));

            // _meshes.Add(new Cube(_vertexProcessor) {BasicColor = FloatColor.Grey}.CreateNormals()
            //     .Scale(0.3f).Rotate(30, Float3.Up + Float3.Left + Float3.Down));

            Statics.Lights.Add(new DirectionalLight
            {
                Position = Float3.Left.Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Red,
                Specular = FloatColor.White,
                Shininess = 32
            });
            
            Statics.Lights.Add(new PointLight
            {
                Position = Float3.Back * 5 + Float3.Right * 2,
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Blue,
                Specular = FloatColor.UnityYellow,
                Shininess = 32
            });
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