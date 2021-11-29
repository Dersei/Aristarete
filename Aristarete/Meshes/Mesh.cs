using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Rendering;
using Daeira;

namespace Aristarete.Meshes
{
    public abstract class Mesh
    {
        public Vertex[] Vertices = null!;
        public Int3[] Indices = null!;
        public List<Triangle> Triangles = null!;
        public readonly List<Float3[]> ScreenCoords = new();
        public readonly List<Float3[]> WorldCoords = new();
        private FloatColor _basicColor;
        public Matrix Object2World = Matrix.Identity;
        public Matrix Object2Projection = Matrix.Identity;
        public Matrix Object2View = Matrix.Identity;
        public bool IsDirty = true;
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

        protected Mesh(FloatColor basicColor = default)
        {
            Material = new PbrMaterial(basicColor);
        }

       public void SetIdentity()
        {
            Object2World = Matrix.Identity;
        }

        public Mesh Rotate(float angle, Float3 v)
        {
            Object2World = Matrix.Rotate(angle, v) * Object2World;
            IsDirty = true;
            return this;
        }

        public Mesh Translate(Float3 v)
        {
            Object2World = Matrix.Translate(v) * Object2World;
            IsDirty = true;
            return this;
        }

        public Mesh Scale(Float3 v)
        {
            Object2World = Matrix.Scale(v) * Object2World;
            IsDirty = true;
            return this;
        }

        public Mesh Scale(float v)
        {
            Object2World = Matrix.Scale(new Float3(v)) * Object2World;
            IsDirty = true;
            return this;
        }
        
        public Mesh SetMaterial(PbrMaterial material)
        {
            Material = material;
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

        public Mesh LoadHeightMap(string textureName, float scale = 1)
        {
            Material.HeightMap = new TextureInfo(Texture.LoadFrom(textureName), scale, Float2.Zero);
            return this;
        }

        public void Transform(Rasterizer rasterizer)
        {
            Object2View = rasterizer.Camera.World2View * Object2World;
            Object2Projection = rasterizer.Camera.View2Proj * Object2View;
            IsDirty = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Apply(Float3 f) => Object2Projection.MultiplyPoint(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 ApplyWorld(Float3 f) => Object2World.MultiplyPoint(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 ApplyView(Float3 f) => Object2View.MultiplyPoint(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 TransformNormals(Float3 f) => Object2View.MultiplyVector(f);

        // public Float3 TransformNormals(Float3 f) => Matrix.MultiplyVector(Object2View,f);

        public void UpdateCoords()
        {
            if (ScreenCoords.Count == 0)
            {
                for (var i = 0; i < Triangles.Count; i++)
                {
                    var triangle = Triangles[i];
                    var screenCoords = new Float3[3];
                    var worldCoords = new Float3[3];

                    for (var j = 0; j < 3; j++)
                    {
                        var v = triangle[j].Position;
                        v += triangle[j].Normal * Material.GetHeight(triangle[j].UV).R / 10f;
                        screenCoords[j] = Apply(v);
                        worldCoords[j] = Object2World.MultiplyPoint(v);
                    }
                    ScreenCoords.Add(screenCoords);
                    WorldCoords.Add(worldCoords);
                }
            }
            else
            {
                for (var i = 0; i < Triangles.Count; i++)
                {
                    var triangle = Triangles[i];

                    for (var j = 0; j < 3; j++)
                    {
                        var v = triangle[j].Position;
                        v += triangle[j].Normal * Material.GetHeight(triangle[j].UV).R / 10f;
                        ScreenCoords[i][j] = Apply(v);
                        WorldCoords[i][j] = Object2World.MultiplyPoint(v);
                    }
                }
            }
        }

        public virtual void Update(Rasterizer rasterizer)
        {
            if (IsDirty)
            {
                Transform(rasterizer);
                UpdateCoords();
            }
            //
            // for (var i = 0; i < Triangles.Count; i++)
            // {
            //     rasterizer.Triangle(Triangles[i], ScreenCoords[i], this, LightingMode, renderMode);
            // }

            if (LiveUpdate) SetIdentity();
        }

        protected virtual Mesh CreateTriangles()
        {
            Triangles = new List<Triangle>();

            for (var i = 0; i < Indices.Length; i++)
            {
                Triangles.Add(new Triangle(
                    Vertices[Indices[i].X],
                    Vertices[Indices[i].Y],
                    Vertices[Indices[i].Z]
                ));
            }

            return this;
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