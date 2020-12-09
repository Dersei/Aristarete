using System;
using Aristarete.Basic;
using Aristarete.Extensions;

namespace Aristarete.Meshes
{
    public class Torus : Mesh
    {
        public Torus(VertexProcessor vertexProcessor, float radius1, float radius2, int radiusSegments = 24,
            int sides = 18) : base(vertexProcessor)
        {
            Create(radius1, radius2, radiusSegments, sides);
        }

        public Torus(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 0.3f, 24, 18);
        }

        private void Create(float radius1, float radius2, int radiusSegments = 24, int sides = 18)
        {
            Vertex[] vertices = new Vertex[(radiusSegments + 1) * (sides + 1)];
            const float _2pi = MathF.PI * 2f;
            for (var seg = 0; seg <= radiusSegments; seg++)
            {
                var currSeg = seg == radiusSegments ? 0 : seg;

                var t1 = (float) currSeg / radiusSegments * _2pi;
                var r1 = new Float3(MathF.Cos(t1) * radius1, 0f, MathF.Sin(t1) * radius1);

                for (var side = 0; side <= sides; side++)
                {
                    var currSide = side == sides ? 0 : side;

                    var t2 = (float) currSide / sides * _2pi;
                    var r2 = Float3.FromBuiltIn(MathExtensions.Rotate(
                        MathExtensions.AngleAxis(-t1 * MathExtensions.Rad2Deg, Float3.Up),
                        new Float3(MathF.Sin(t2) * radius2, MathF.Cos(t2) * radius2, 0).ToBuiltIn()));

                    vertices[side + seg * (sides + 1)] = r1 + r2;
                }
            }


            var facesNumber = vertices.Length;
            var trianglesNumber = facesNumber * 2;
            var indicesNumber = trianglesNumber * 3;
            int[] triangles = new int[indicesNumber];

            var i = 0;
            for (var seg = 0; seg <= radiusSegments; seg++)
            {
                for (var side = 0; side <= sides - 1; side++)
                {
                    var current = side + seg * (sides + 1);
                    var next = side + (seg < radiusSegments ? (seg + 1) * (sides + 1) : 0);

                    if (i < triangles.Length - 6)
                    {
                        triangles[i++] = current;
                        triangles[i++] = next;
                        triangles[i++] = next + 1;

                        triangles[i++] = current;
                        triangles[i++] = next + 1;
                        triangles[i++] = current + 1;
                    }
                }
            }

            var size = triangles.Length / 3;
            Int3[] indices = new Int3[size];
            for (var j = 0; j < size; j++)
            {
                indices[j] = new Int3(triangles[j * 3], triangles[j * 3 + 1], triangles[j * 3 + 2]);
            }

            Vertices = vertices;
            Indices = indices;
        }
    }
}