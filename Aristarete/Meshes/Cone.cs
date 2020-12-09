using System;
using Aristarete.Basic;

namespace Aristarete.Meshes
{
    public class Cone : Mesh
    {
        private void Create(float radius, float height, int subdivisions = 10)
        {
            Vertex[] vertices = new Vertex[subdivisions + 2];
            Int3[] triangles = new Int3[subdivisions * 2];

            vertices[0] = Float3.Zero;
            for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
            {
                var ratio = (float) i / n;
                var r = ratio * (MathF.PI * 2f);
                var x = MathF.Cos(r) * radius;
                var z = MathF.Sin(r) * radius;
                vertices[i + 1] = new Float3(x, 0f, z);
            }

            vertices[subdivisions + 1] = new Float3(0f, height, 0f);

            // construct bottom

            for (int i = 0, n = subdivisions - 1; i < n; i++)
            {
                triangles[i] = new Int3(0, i + 1, i + 2);
            }

            // construct sides

            var bottomOffset = subdivisions;
            for (int i = 0, n = subdivisions - 1; i < n; i++)
            {
                var offset = i + bottomOffset;
                triangles[offset] = new Int3(i + 1, subdivisions + 1, i + 2);
            }

            Vertices = vertices;
            Indices = triangles;
        }

        public Cone(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 2, 10);
        }

        public Cone(VertexProcessor vertexProcessor, float radius, float height, int subdivisions = 10) : base(
            vertexProcessor)
        {
            Create(radius, height, subdivisions);
        }
    }
}