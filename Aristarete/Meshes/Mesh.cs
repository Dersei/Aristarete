using System;
using System.Diagnostics.CodeAnalysis;
using Aristarete.Basic;
using Aristarete.Lighting;

namespace Aristarete.Meshes
{
    public abstract class Mesh : IRenderable
    {
        public Vertex[] Vertices = null!;
        public Int3[] Indices = null!;
        public VertexProcessor VertexProcessor { get; }
        public FloatColor BasicColor = FloatColor.Error;
        public Matrix Object2World = Matrix.Identity;
        public Matrix Object2Projection = Matrix.Identity;
        public Matrix Object2View = Matrix.Identity;
        private bool _isDirty = true;
        public bool VertexLight = false;
        public bool LifeUpdate = false;

        protected Mesh(VertexProcessor vertexProcessor)
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

        public IRenderable Scale(float v)
        {
            Object2World = Matrix.Scale(new Float3(v)) * Object2World;
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

        public Float3 ApplyView(Float3 f) => Object2View.MultiplyPoint(f);

        public Float3 TransformNormals(Float3 f) => Object2View.MultiplyVector(f);

        public void Update(Rasterizer rasterizer)
        {
            if (_isDirty)
            {
                Transform();
                if(LifeUpdate) SetIdentity();
            }

            for (var i = 0; i < Indices.Length; i++)
            {
                var face = Indices[i];
                var screenCoords = new Float3[3];
                var worldCoords = new Float3[3];

                for (var j = 0; j < 3; j++)
                {
                    var v = Vertices[face[j]].Position;
                    screenCoords[j] = Apply(v);
                    worldCoords[j] = v;
                }

                var uv = new Float2[3];
                for (var k = 0; k < 3; k++)
                {
                    uv[k] = Float2.Zero;
                }

                var colorA = FloatColor.Black;
                var colorB = FloatColor.Black;
                var colorC = FloatColor.Black;

                foreach (var light in Statics.Lights)
                {
                    colorA += light.Calculate(Vertices[Indices[i].X], this);
                    colorB += light.Calculate(Vertices[Indices[i].Y], this);
                    colorC += light.Calculate(Vertices[Indices[i].Z], this);
                }

                if (VertexLight)
                {
                    rasterizer.TriangleVertices(
                        screenCoords,
                        new[]
                        {
                            colorA * BasicColor, colorB * BasicColor, colorC * BasicColor
                        });
                }
                else
                {
                    rasterizer.Triangle(new[]
                        {
                            Vertices[Indices[i].X],
                            Vertices[Indices[i].Y],
                            Vertices[Indices[i].Z]
                        },
                        screenCoords,
                        new[]
                        {
                            BasicColor, BasicColor, BasicColor
                            // FloatColor.FromNormal(Vertices[Indices[i].X].Normal),
                            // FloatColor.FromNormal(Vertices[Indices[i].Y].Normal),
                            // FloatColor.FromNormal(Vertices[Indices[i].Z].Normal)
                        }, this);
                }
            }
        }

        public Mesh CreateNormals()
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = Float3.Zero;
            }

            for (int i = 0; i < Indices.Length; i++)
            {
                var n = (Vertices[Indices[i].Y].Position - Vertices[Indices[i].X].Position).Cross(
                    Vertices[Indices[i].Z].Position - Vertices[Indices[i].X].Position);

                Vertices[Indices[i].X].Normal += n;
                Vertices[Indices[i].Y].Normal += n;
                Vertices[Indices[i].Z].Normal += n;
            }

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = Float3.Normalize(Vertices[i].Normal);
            }

            return this;
        }
    }
}