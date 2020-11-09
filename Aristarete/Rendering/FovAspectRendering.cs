using System;
using Aristarete.Basic;
using Aristarete.Inputting;

namespace Aristarete.Rendering
{
    public class FovAspectRendering : IRendering
    {
        private float _fov = 60;
        private float _aspect = 1;
        private readonly VertexProcessor _vertexProcessor = new VertexProcessor();

        public FovAspectRendering()
        {
            _vertexProcessor.SetPerspective(_fov, _aspect, 1, 1000);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, -1), Float3.Up);
            _vertexProcessor.SetIdentity();
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

            if (Input.IsNewPress(Keys.W))
            {
                _aspect += 1f;
                Console.WriteLine("Aspect: " + (_aspect < 0 ? $"{-_aspect}:1" : $"1:{_aspect}"));
            }
            else if (Input.IsNewPress(Keys.S))
            {
                _aspect -= 1f;
                Console.WriteLine("Aspect: " + (_aspect < 0 ? $"{-_aspect}:1" : $"1:{_aspect}"));
            }

            _vertexProcessor.SetIdentity();
            _vertexProcessor.Rotate(20, Float3.Up);
            _vertexProcessor.Rotate(35, Float3.Left);
            _vertexProcessor.SetPerspective(_fov, _aspect, 1, 1000);
            _vertexProcessor.Transform();
            
             rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Red,
                    FloatColor.Red,
                    FloatColor.Red
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Cyan,
                    FloatColor.Cyan,
                    FloatColor.Cyan
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Cyan,
                    FloatColor.Cyan,
                    FloatColor.Cyan
                });

            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Magenta,
                    FloatColor.Magenta,
                    FloatColor.Magenta
                });
            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Magenta,
                    FloatColor.Magenta,
                    FloatColor.Magenta
                });
            rasterizer.Triangle(new[]
                {
                    _vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    _vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Red,
                    FloatColor.Red,
                    FloatColor.Red
                });
        }
    }
}