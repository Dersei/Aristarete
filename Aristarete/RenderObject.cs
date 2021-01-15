using System;
using Aristarete.Basic;
using Aristarete.Models;

namespace Aristarete
{
    public class RenderObject : IRenderable
    {
        public VertexProcessor VertexProcessor { get; }
        public Model2 Model { get; }
        public Matrix Object2World = Matrix.Identity;
        public Matrix Object2Projection = Matrix.Identity;
        public Matrix Object2View = Matrix.Identity;
        private bool _isDirty= true;

        public RenderObject(VertexProcessor vertexProcessor, Model2 model)
        {
            VertexProcessor = vertexProcessor;
            Model = model;
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

        public void Update(Rasterizer rasterizer)
        {
            if (_isDirty)
            {
                Transform();
                SetIdentity();
            }
           
            for (var i = 0; i < Model.Triangles.Count; i++)
            {
                var face = Model.Triangles[i];
                var screenCoords = new Float3[3];

                for (var j = 0; j < 3; j++)
                {
                    var v = face[j];
                    screenCoords[j] = Apply(v.Position);
                }

                var uv = new Float2[3];
                for (var k = 0; k < 3; k++)
                {
                    uv[k] = face[k].UV;
                   uv[k] = new Float2(1 - MathF.Min(uv[k].Y, 1.0f),
                       1 - MathF.Min(uv[k].X, 1.0f));
                  // Console.WriteLine(uv[k]);
                }

                rasterizer.Triangle(screenCoords, uv, Model);
            }
        }
    }
}