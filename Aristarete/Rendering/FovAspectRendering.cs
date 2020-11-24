using System;
using Aristarete.Basic;
using Aristarete.Inputting;

namespace Aristarete.Rendering
{
    public class FovAspectRendering : IRendering
    {
        private float _fov = 60;
        private float _aspect = 1;
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly CubeObject _cubeObject;

        public FovAspectRendering()
        {
            _vertexProcessor.SetPerspective(_fov, _aspect, 1, 1000);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, -1), Float3.Up);
            _cubeObject = new CubeObject(_vertexProcessor);
        }

        public void Run(Rasterizer rasterizer)
        {
            if (Input.IsNewPress(Keys.A))
            {
                _fov += 10f;
                Console.WriteLine($"FOV: {_fov}");
            }
            else if (Input.IsNewPress(Keys.D))
            {
                _fov -= 10f;
                Console.WriteLine($"FOV: {_fov}");
            }
            else if (Input.IsNewPress(Keys.W))
            {
                _aspect += 1f;
                Console.WriteLine("Aspect: " + (_aspect < 0 ? $"{-_aspect}:1" : $"1:{_aspect}"));
            }
            else if (Input.IsNewPress(Keys.S))
            {
                _aspect -= 1f;
                Console.WriteLine("Aspect: " + (_aspect < 0 ? $"{-_aspect}:1" : $"1:{_aspect}"));
            }

            _cubeObject.SetIdentity();
            _cubeObject.Rotate(20, Float3.Up);
            _cubeObject.Rotate(35, Float3.Left);
            _vertexProcessor.SetPerspective(_fov, _aspect, 1, 1000);
            
            _cubeObject.Update(rasterizer);
        }
    }
}