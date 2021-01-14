using System;
using Aristarete.Basic;

namespace Aristarete.Meshes
{
    public class Sphere : Mesh
    {
        public Sphere(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 24, 16);
        }

        public Sphere(VertexProcessor vertexProcessor, float radius, int length = 24, int latitude = 16) : base(
            vertexProcessor)
        {
            Create(radius, length, latitude);
        }

        private void Create(float radius, int length = 24, int latitude = 16)
        {
            Vertex[] vertices = new Vertex[(length + 1) * latitude + 2];
            const float pi = MathF.PI;
            const float _2pi = pi * 2f;

            vertices[0] = Float3.Up * radius;
            for (var lat = 0; lat < latitude; lat++)
            {
                var a1 = pi * (lat + 1) / (latitude + 1);
                var sin1 = MathF.Sin(a1);
                var cos1 = MathF.Cos(a1);

                for (var lon = 0; lon <= length; lon++)
                {
                    var a2 = _2pi * (lon == length ? 0 : lon) / length;
                    var sin2 = MathF.Sin(a2);
                    var cos2 = MathF.Cos(a2);

                    vertices[lon + lat * (length + 1) + 1] = new Float3(sin1 * cos2, cos1, sin1 * sin2) * radius;
                }
            }

            vertices[^1] = Float3.Up * -radius;

            for (var n = 0; n < vertices.Length; n++)
            {
                vertices[n].Normal = vertices[n].Position.Normalize();
            }

            Float2[] uvs = new Float2[vertices.Length];
            uvs[0] = Float2.Up;
            uvs[^1] = Float2.Zero;
            for (var lat = 0; lat < latitude; lat++)
            {
                for (var lon = 0; lon <= length; lon++)
                {
                    uvs[lon + lat * (length + 1) + 1] =
                        new Float2((float) lon / length, 1f - (float) (lat + 1) / (latitude + 1));
                }
            }

            var nbFaces = vertices.Length;
            var nbTriangles = nbFaces * 2;
            var nbIndexes = nbTriangles * 3;
            int[] triangles = new int[nbIndexes];

            //top cap
            var i = 0;
            for (var lon = 0; lon < length; lon++)
            {
                triangles[i++] = lon + 2;
                triangles[i++] = lon + 1;
                triangles[i++] = 0;
            }

            //middle
            for (var lat = 0; lat < latitude - 1; lat++)
            {
                for (var lon = 0; lon < length; lon++)
                {
                    var current = lon + lat * (length + 1) + 1;
                    var next = current + length + 1;

                    triangles[i++] = current;
                    triangles[i++] = current + 1;
                    triangles[i++] = next + 1;

                    triangles[i++] = current;
                    triangles[i++] = next + 1;
                    triangles[i++] = next;
                }
            }

            //bottom cap
            for (var lon = 0; lon < length; lon++)
            {
                triangles[i++] = vertices.Length - 1;
                triangles[i++] = vertices.Length - (lon + 2) - 1;
                triangles[i++] = vertices.Length - (lon + 1) - 1;
            }


            for (var n = 0; n < vertices.Length; n++)
            {
                vertices[n].UV = uvs[n];
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