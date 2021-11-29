using System;
using Aristarete.Basic;


namespace Aristarete.Meshes
{
    public class Cone : Mesh
    {
        private void Create(float radius, float height, int subdivisions = 18)
        {
            var bottomRadius = radius;
            const float topRadius = .00f;
            const int heightSeg = 1; // Not implemented yet

            var verticesCap = subdivisions + 1;

            #region Vertices

// bottom + top + sides
            Float3Sse[] vertices = new Float3Sse[verticesCap + verticesCap + subdivisions * heightSeg * 2 + 2];
            var vert = 0;
            const float _2pi = MathF.PI * 2f;

// Bottom cap
            vertices[vert++] = new Float3Sse(0f, 0f, 0f);
            while (vert <= subdivisions)
            {
                var rad = (float) vert / subdivisions * _2pi;
                vertices[vert] = new Float3Sse(MathF.Cos(rad) * bottomRadius, 0f, MathF.Sin(rad) * bottomRadius);
                vert++;
            }

// Top cap
            vertices[vert++] = new Float3Sse(0f, height, 0f);
            while (vert <= subdivisions * 2 + 1)
            {
                var rad = (float) (vert - subdivisions - 1) / subdivisions * _2pi;
                vertices[vert] = new Float3Sse(MathF.Cos(rad) * topRadius, height, MathF.Sin(rad) * topRadius);
                vert++;
            }

// Sides
            var v = 0;
            while (vert <= vertices.Length - 4)
            {
                var rad = (float) v / subdivisions * _2pi;
                vertices[vert] = new Float3Sse(MathF.Cos(rad) * topRadius, height, MathF.Sin(rad) * topRadius);
                vertices[vert + 1] = new Float3Sse(MathF.Cos(rad) * bottomRadius, 0, MathF.Sin(rad) * bottomRadius);
                vert += 2;
                v++;
            }

            vertices[vert] = vertices[subdivisions * 2 + 2];
            vertices[vert + 1] = vertices[subdivisions * 2 + 3];

            #endregion

            #region Normales

// bottom + top + sides
            Float3Sse[] normals = new Float3Sse[vertices.Length];
            vert = 0;

// Bottom cap
            while (vert <= subdivisions)
            {
                normals[vert++] = Float3Sse.Down;
            }

// Top cap
            while (vert <= subdivisions * 2 + 1)
            {
                normals[vert++] = Float3Sse.Up;
            }

// Sides
            v = 0;
            while (vert <= vertices.Length - 4)
            {
                var rad = (float) v / subdivisions * _2pi;
                var cos = MathF.Cos(rad);
                var sin = MathF.Sin(rad);

                normals[vert] = new Float3Sse(cos, 0f, sin);
                normals[vert + 1] = normals[vert];

                vert += 2;
                v++;
            }

            normals[vert] = normals[subdivisions * 2 + 2];
            normals[vert + 1] = normals[subdivisions * 2 + 3];

            #endregion

            #region UVs

            Float2[] uvs = new Float2[vertices.Length];

// Bottom cap
            var u = 0;
            uvs[u++] = new Float2(0.5f, 0.5f);
            while (u <= subdivisions)
            {
                var rad = (float) u / subdivisions * _2pi;
                uvs[u] = new Float2(MathF.Cos(rad) * .5f + .5f, MathF.Sin(rad) * .5f + .5f);
                u++;
            }

// Top cap
            uvs[u++] = new Float2(0.5f, 0.5f);
            while (u <= subdivisions * 2 + 1)
            {
                var rad = (float) u / subdivisions * _2pi;
                uvs[u] = new Float2(MathF.Cos(rad) * .5f + .5f, MathF.Sin(rad) * .5f + .5f);
                u++;
            }

// Sides
            var uSides = 0;
            while (u <= uvs.Length - 4)
            {
                var t = (float) uSides / subdivisions;
                uvs[u] = new Float2(t, 1f);
                uvs[u + 1] = new Float2(t, 0f);
                u += 2;
                uSides++;
            }

            uvs[u] = new Float2(1f, 1f);
            uvs[u + 1] = new Float2(1f, 0f);

            #endregion

            #region Triangles

            var trianglesNumber = subdivisions + subdivisions + subdivisions * 2;
            int[] triangles = new int[trianglesNumber * 3 + 3];

// Bottom cap
            var tri = 0;
            var i = 0;
            while (tri < subdivisions - 1)
            {
                triangles[i] = 0;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = tri + 2;
                tri++;
                i += 3;
            }

            triangles[i] = 0;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = 1;
            tri++;
            i += 3;

// Top cap
//tri++;
            while (tri < subdivisions * 2)
            {
                triangles[i] = tri + 2;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = verticesCap;
                tri++;
                i += 3;
            }

            triangles[i] = verticesCap + 1;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = verticesCap;
            tri++;
            i += 3;
            tri++;

// Sides
            while (tri <= trianglesNumber)
            {
                triangles[i] = tri + 2;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;

                triangles[i] = tri + 1;
                triangles[i + 1] = tri + 2;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;
            }

            #endregion

            Vertex[] finalVertices = new Vertex[vertices.Length];

            for (var j = 0; j < finalVertices.Length; j++)
            {
                finalVertices[j] = new Vertex(vertices[j], normals[j], uvs[j]);
            }

            Int3[] indices = new Int3[triangles.Length / 3];

            for (int i0 = 0, j = 0; i0 < indices.Length; i0++, j += 3)
            {
                indices[i0] = new Int3(triangles[j], triangles[j + 1], triangles[j + 2]);
            }

            Vertices = finalVertices;
            Indices = indices;

            CreateTriangles();
        }

        public Cone()
        {
            Create(1, 2, 10);
        }

        public Cone(float radius, float height, int subdivisions = 10)
        {
            Create(radius, height, subdivisions);
        }
    }
}