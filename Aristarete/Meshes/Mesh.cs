using System;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Aristarete.Basic;
using Aristarete.Extensions;

namespace Aristarete.Meshes
{
    public abstract class Mesh : IRenderable
    {
        public Vertex[] Vertices = null!;
        public Int3[] Indices = null!;
        public Float2[] UVs = null!;
        public readonly VertexProcessor VertexProcessor;
        public FloatColor BasicColor = FloatColor.Error;
        public Matrix Object2World = Matrix.Identity;
        public Matrix Object2Projection = Matrix.Identity;
        public Matrix Object2View = Matrix.Identity;
        private bool _isDirty = true;
        public LightingMode LightingMode = LightingMode.Pixel;
        public bool LiveUpdate = false;

        private uint[]? _diffuseMap;
        private int _mapWidth;
        private int _mapHeight;

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

        public IRenderable LoadTexture(string textureName)
        {
            var img = new BitmapImage(new Uri(textureName, UriKind.Relative));
            var wbm = new WriteableBitmap(img);
            var colors = new uint[wbm.PixelWidth * wbm.PixelHeight];
            wbm.CopyPixels(colors, wbm.BackBufferStride, 0);
            _diffuseMap = colors;
            _mapWidth = wbm.PixelWidth;
            _mapHeight = wbm.PixelHeight;
            return this;
        }

        public void Transform()
        {
            Object2View = VertexProcessor.World2View * Object2World;
            Object2Projection = VertexProcessor.View2Proj * Object2View;
            _isDirty = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Apply(Float3 f) => Object2Projection.MultiplyPoint(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 ApplyView(Float3 f) => Object2View.MultiplyPoint(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 TransformNormals(Float3 f) => Object2View.MultiplyVector(f);

        // public Float3 TransformNormals(Float3 f) => Matrix.MultiplyVector(Object2View,f);
        public FloatColor GetDiffuse(Float2 uv)
        {
            if (_diffuseMap is null) return FloatColor.Error;
            var x = MathExtensions.Clamp((int) (uv.X),0,_mapWidth - 1);
            var y = MathExtensions.Clamp((int) (uv.Y),0,_mapHeight - 1);
            return FloatColor.FromArgb(_diffuseMap[y * _mapHeight + x]);
        }

        public void Update(Rasterizer rasterizer)
        {
            if (_isDirty)
            {
                Transform();
                if (LiveUpdate) SetIdentity();
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

                var uvs = new Float2[3];
                if (_diffuseMap is not null)
                {
                    for (var k = 0; k < 3; k++)
                    {
                        uvs[k] = new Float2(UVs[face[k]].X * _mapWidth, UVs[face[k]].Y * _mapHeight);
                    }
                }

                switch (LightingMode)
                {
                    case LightingMode.Vertex:
                    {
                        var colorA = FloatColor.Black;
                        var colorB = FloatColor.Black;
                        var colorC = FloatColor.Black;

                        foreach (var light in Statics.Lights)
                        {
                            colorA += light.Calculate(Vertices[Indices[i].X], this);
                            colorB += light.Calculate(Vertices[Indices[i].Y], this);
                            colorC += light.Calculate(Vertices[Indices[i].Z], this);
                        }

                        rasterizer.TriangleVertices(
                            screenCoords,
                            new[]
                            {
                                colorA * BasicColor, colorB * BasicColor, colorC * BasicColor
                            });
                        break;
                    }

                    case LightingMode.Pixel:
                    {
                        if (_diffuseMap is null)
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
                                }, this);
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
                                }, uvs, this);
                        }
                        break;
                    }
                    case LightingMode.None:
                    {
                        if (_diffuseMap is null)
                        {
                            rasterizer.TriangleVertices(
                                screenCoords,
                                new[]
                                {
                                    BasicColor, BasicColor, BasicColor
                                });
                        }
                        else
                        {
                            rasterizer.TriangleVertices(
                                screenCoords,
                                new[]
                                {
                                    BasicColor, BasicColor, BasicColor
                                }, uvs, this);
                        }
                        break;
                    }
                }
                

  
            }
        }

        public Mesh CreateNormals()
        {
            for (var i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = Float3.Zero;
            }

            for (var i = 0; i < Indices.Length; i++)
            {
                var n = (Vertices[Indices[i].Y].Position - Vertices[Indices[i].X].Position).Cross(
                    Vertices[Indices[i].Z].Position - Vertices[Indices[i].X].Position);

                Vertices[Indices[i].X].Normal += n;
                Vertices[Indices[i].Y].Normal += n;
                Vertices[Indices[i].Z].Normal += n;
            }

            for (var i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = Float3.Normalize(Vertices[i].Normal);
            }

            return this;
        }
    }
}