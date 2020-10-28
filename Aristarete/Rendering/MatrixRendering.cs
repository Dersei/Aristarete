using System;
using System.Windows.Input;
using Aristarete.Basic;

namespace Aristarete.Rendering
{
    public class MatrixRendering : IRendering
    {
        private float _angle;

        public void Run(Rasterizer rasterizer)
        {
            if (Keyboard.IsKeyDown(Key.A))
            {
                _angle += 1f;
            }
            else if (Keyboard.IsKeyDown(Key.D))
            {
                _angle -= 1f;
            }

            var vertexProcessor = new VertexProcessor();
            vertexProcessor.SetPerspective(75, 1, 1, 1000);
            vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, -1), Float3.Up);
            vertexProcessor.SetIdentity();
            vertexProcessor.Rotate(_angle, Float3.Left);
            vertexProcessor.Transform();

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Red,
                    FloatColor.Red,
                    FloatColor.Red
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Cyan,
                    FloatColor.Cyan,
                    FloatColor.Cyan
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Cyan,
                    FloatColor.Cyan,
                    FloatColor.Cyan
                });

            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Magenta,
                    FloatColor.Magenta,
                    FloatColor.Magenta
                });
            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, -1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Magenta,
                    FloatColor.Magenta,
                    FloatColor.Magenta
                });
            rasterizer.Triangle(new[]
                {
                    vertexProcessor.Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    vertexProcessor.Apply(new Float3(1.0f, -1.0f, 1.0f))
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