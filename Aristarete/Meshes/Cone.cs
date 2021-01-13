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
            
            Float2[] uvs = new Float2[vertices.Length];
            subdivisions -= 1;
// Bottom cap
            int u = 0;
            uvs[u++] = new Float2(0.5f, 0.5f);
            while (u <= subdivisions)
            {
                float rad = (float)u / subdivisions * MathF.PI*2;
                uvs[u] = new Float2(MathF.Cos(rad) * .5f + .5f, MathF.Sin(rad) * .5f + .5f);
                u++;
            }
            
// Sides
            int uSides = 0;
            while (u <= uvs.Length - 4 )
            {
                float t = (float)uSides / subdivisions;
                uvs[u] = new Float2(t, 1f);
                uvs[u + 1] = new Float2(t, 0f);
                u += 2;
                uSides++;
            }
            uvs[u] = new Float2(1f, 1f);
            uvs[u + 1] = new Float2(1f, 0f);

            Vertices = vertices;
            UVs = uvs;
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