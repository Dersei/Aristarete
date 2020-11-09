﻿using Aristarete.Basic;
using Aristarete.Inputting;

namespace Aristarete.Rendering
{
    public class MatrixRendering : IRendering
    {
        private float _angleLeft;
        private float _angleUp;
        private readonly VertexProcessor _vertexProcessor = new VertexProcessor();
        private bool _isRotating;

        public MatrixRendering()
        {
            _vertexProcessor.SetPerspective(60, 1, 1, 1000);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, -1), Float3.Up);
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