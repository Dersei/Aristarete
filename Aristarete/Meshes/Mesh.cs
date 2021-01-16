using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;

namespace Aristarete.Meshes
{
    public abstract class Mesh : IRenderable
    {
        public Vertex[] Vertices = null!;
        public Int3[] Indices = null!;
        public readonly VertexProcessor VertexProcessor = null!;
        private FloatColor _basicColor;
        public Matrix Object2World = Matrix.Identity;
        public Matrix Object2Projection = Matrix.Identity;
        public Matrix Object2View = Matrix.Identity;
        protected bool IsDirty = true;
        public LightingMode LightingMode = LightingMode.Pixel;
        public bool LiveUpdate = false;

        public PbrMaterial Material { get; set; }

        public FloatColor BasicColor
        {
            get => _basicColor;
            set
            {
                Material.Color = value;
                _basicColor = value;
            }
        }

        protected Mesh(VertexProcessor vertexProcessor, FloatColor basicColor = default)
        {
            VertexProcessor = vertexProcessor;
            Material = new PbrMaterial(basicColor);
        }


        public void SetIdentity()
        {
            Object2World = Matrix.Identity;
        }

        public IRenderable Rotate(float angle, Float3 v)
        {
            Object2World = Matrix.Rotate(angle, v) * Object2World;
            IsDirty = true;
            return this;
        }

        public IRenderable Translate(Float3 v)
        {
            Object2World = Matrix.Translate(v) * Object2World;
            IsDirty = true;
            return this;
        }

        public IRenderable Scale(Float3 v)
        {
            Object2World = Matrix.Scale(v) * Object2World;
            IsDirty = true;
            return this;
        }

        public IRenderable Scale(float v)
        {
            Object2World = Matrix.Scale(new Float3(v)) * Object2World;
            IsDirty = true;
            return this;
        }

        public Mesh LoadDiffuseMap(string textureName, float scale = 1)
        {
            Material.DiffuseMap = new TextureInfo(Texture.LoadFrom(textureName), scale, Float2.Zero);
            return this;
        }

        public Mesh LoadSpecularMap(string textureName, float scale = 1)
        {
            Material.SpecularMap = new TextureInfo(Texture.LoadFrom(textureName), scale, Float2.Zero);
            return this;
        }

        public Mesh LoadEmissiveMap(string textureName, float emissionFactor = 1, float scale = 1)
        {
            Material.EmissiveMap = new TextureInfo(Texture.LoadFrom(textureName), scale, Float2.Zero);
            Material.EmissionFactor = emissionFactor;
            return this;
        }

        public Mesh LoadNormalMap(string textureName, float scale = 1)
        {
            Material.NormalMap = new TextureInfo(Texture.LoadFrom(textureName), scale, Float2.Zero);
            return this;
        }
        
        public Mesh LoadOpacityMap(string textureName, float scale = 1)
        {
            Material.OpacityMap = new TextureInfo(Texture.LoadFrom(textureName), scale, Float2.Zero);
            return this;
        }

        public void Transform()
        {
            Object2View = VertexProcessor.World2View * Object2World;
            Object2Projection = VertexProcessor.View2Proj * Object2View;
            IsDirty = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Apply(Float3 f) => Object2Projection.MultiplyPoint(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 ApplyView(Float3 f) => Object2View.MultiplyPoint(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 TransformNormals(Float3 f) => Object2View.MultiplyVector(f);

        // public Float3 TransformNormals(Float3 f) => Matrix.MultiplyVector(Object2View,f);

        public virtual void Update(Rasterizer rasterizer)
        {
            if (IsDirty)
            {
                Transform();
                if (LiveUpdate) SetIdentity();
            }

            for (var i = 0; i < Indices.Length; i++)
            {
                var face = Indices[i];
                var screenCoords = new Float3[3];

                for (var j = 0; j < 3; j++)
                {
                    var v = Vertices[face[j]].Position;
                    screenCoords[j] = Apply(v);
                }

                rasterizer.Triangle(new Triangle(
                        Vertices[Indices[i].X],
                        Vertices[Indices[i].Y],
                        Vertices[Indices[i].Z]
                    ),
                    screenCoords,
                    this, LightingMode);
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