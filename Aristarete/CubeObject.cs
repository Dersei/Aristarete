using Aristarete.Basic;

namespace Aristarete
{
    public class CubeObject : IRenderable
    {
        public VertexProcessor VertexProcessor { get; }
        public Matrix Object2World = Matrix.Identity;
        public Matrix Object2Projection = Matrix.Identity;
        public Matrix Object2View = Matrix.Identity;
        private bool _isDirty = true;

        public CubeObject(VertexProcessor vertexProcessor)
        {
            VertexProcessor = vertexProcessor;
        }

        public void SetIdentity()
        {
            Object2World = Matrix.Identity;
        }

        public IRenderable Rotate(float angle, Float3 v)
        {
            Object2World = Matrix.Rotate(angle, v) * Object2World;
            _isDirty = true;
            return this;
        }

        public IRenderable Translate(Float3 v)
        {
            Object2World = Matrix.Translate(v) * Object2World;
            _isDirty = true;
            return this;
        }

        public IRenderable Scale(Float3 v)
        {
            Object2World = Matrix.Scale(v) * Object2World;
            _isDirty = true;
            return this;
        }

        public void Transform()
        {
            Object2View = VertexProcessor.World2View * Object2World;
            Object2Projection = VertexProcessor.View2Proj * Object2View;
            _isDirty = false;
        }

        public Float3 Apply(Float3 f) => Object2Projection.MultiplyPoint(f);

        public void Update(Rasterizer rasterizer)
        {
            if (_isDirty)
            {
                Transform();
                SetIdentity();
            }

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, -1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    Apply(new Float3(1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    Apply(new Float3(-1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(-1.0f, -1.0f, -1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, -1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, -1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, -1.0f, 1.0f)),
                    Apply(new Float3(1.0f, -1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Red,
                    FloatColor.Red,
                    FloatColor.Red
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    Apply(new Float3(1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Cyan,
                    FloatColor.Cyan,
                    FloatColor.Cyan
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, -1.0f, -1.0f)),
                    Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(1.0f, -1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Cyan,
                    FloatColor.Cyan,
                    FloatColor.Cyan
                });

            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(1.0f, 1.0f, -1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, -1.0f))
                },
                new[]
                {
                    FloatColor.Magenta,
                    FloatColor.Magenta,
                    FloatColor.Magenta
                });
            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, -1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, 1.0f))
                },
                new[]
                {
                    FloatColor.Magenta,
                    FloatColor.Magenta,
                    FloatColor.Magenta
                });
            rasterizer.Triangle(new[]
                {
                    Apply(new Float3(1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(-1.0f, 1.0f, 1.0f)),
                    Apply(new Float3(1.0f, -1.0f, 1.0f))
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