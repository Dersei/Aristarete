using Aristarete.Basic;
using Aristarete.Inputting;

namespace Aristarete.Rendering
{
    public class MatrixRendering : IRendering
    {
        private float _angleLeft;
        private float _angleUp;
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly CubeObject _cubeObject;

        public MatrixRendering()
        {
            _vertexProcessor.SetPerspective(60, 1, 1, 1000);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, -1), Float3.Up);
            _cubeObject = new CubeObject(_vertexProcessor);
        }

        public void Run(Rasterizer rasterizer)
        {
            if (Input.IsNewPress(Keys.A))
            {
                _angleLeft += 10f;
            }
            else if (Input.IsNewPress(Keys.D))
            {
                _angleLeft -= 10f;
            }
            else if (Input.IsNewPress(Keys.W))
            {
                _angleUp += 10f;
            }
            else if (Input.IsNewPress(Keys.S))
            {
                _angleUp -= 10f;
            }

            _cubeObject.Rotate(_angleLeft, Float3.Up);
            _cubeObject.Rotate(_angleUp, Float3.Left);

            _cubeObject.Update(rasterizer);
        }
    }
}