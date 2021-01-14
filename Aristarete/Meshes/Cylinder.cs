using System;
using Aristarete.Basic;

namespace Aristarete.Meshes
{
    public class Cylinder : Mesh
    {
        public Cylinder(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 1, 2, 28, 1);
        }

        public Cylinder(VertexProcessor vertexProcessor, float bottomRadius, float topRadius, float length, int slices,
            int stacks) : base(vertexProcessor)
        {
            Create(bottomRadius, topRadius, length, slices, stacks);
        }

        private void Create(float bottomRadius, float topRadius, float length, int slices, int stacks)
        {
            var sliceStep = MathF.PI * 2.0f / slices;
            var heightStep = length / stacks;
            var radiusStep = (topRadius - bottomRadius) / stacks;
            var currentHeight = -length / 2;
            var vertexCount = (stacks + 1) * slices + 2; //cone = stacks * slices + 1
            var triangleCount = (stacks + 1) * slices * 2; //cone = stacks * slices * 2 + slices
            var indexCount = triangleCount * 3;
            var currentRadius = bottomRadius;

            Vertex[] cylinderVertices = new Vertex[vertexCount];
            Float3[] cylinderNormals = new Float3[vertexCount];
            Float2[] cylinderUVs = new Float2[vertexCount];

            // Start at the bottom of the cylinder            
            var currentVertex = 0;
            cylinderVertices[currentVertex] = new Float3(0, currentHeight, 0);
            cylinderNormals[currentVertex] = Float3.Down;
            currentVertex++;
            for (var i = 0; i <= stacks; i++)
            {
                float sliceAngle = 0;
                for (var j = 0; j < slices; j++)
                {
                    var x = currentRadius * MathF.Cos(sliceAngle);
                    var y = currentHeight;
                    var z = currentRadius * MathF.Sin(sliceAngle);

                    var position = new Float3(x, y, z);
                    cylinderVertices[currentVertex] = position;
                    cylinderNormals[currentVertex] = Float3.Normalize(position);
                    cylinderUVs[currentVertex] =
                        new Float2((float) (Math.Sin(cylinderNormals[currentVertex].X)/MathF.PI + 0.5f),
                            (float) (Math.Sin(cylinderNormals[currentVertex].Y)/MathF.PI + 0.5f));
                    currentVertex++;

                    sliceAngle += sliceStep;
                }

                currentHeight += heightStep;
                currentRadius += radiusStep;
            }

            cylinderVertices[currentVertex] = new Float3(0, length / 2, 0);
            cylinderNormals[currentVertex] = Float3.Up;
            
            var triangles = CreateIndexBuffer(vertexCount, indexCount, slices);

            var size = triangles.Length / 3;
            Int3[] indices = new Int3[size];
            for (var j = 0; j < size; j++)
            {
                indices[j] = new Int3(triangles[j * 3], triangles[j * 3 + 1], triangles[j * 3 + 2]);
            }

            for (int i = 0; i < cylinderVertices.Length; i++)
            {
                cylinderVertices[i].Normal = cylinderNormals[i];
            }
            
            for (var n = 0; n < cylinderVertices.Length; n++)
            {
                cylinderVertices[n].UV = cylinderUVs[n];
            }

            Vertices = cylinderVertices;
            Indices = indices;
        }

        private static int[] CreateIndexBuffer(int vertexCount, int indexCount, int slices)
        {
            int[] indices = new int[indexCount];
            var currentIndex = 0;

            // Bottom circle/cone of shape
            for (var i = 1; i <= slices; i++)
            {
                indices[currentIndex++] = i;
                indices[currentIndex++] = 0;
                if (i - 1 == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
            }

            // Middle sides of shape
            for (var i = 1; i < vertexCount - slices - 1; i++)
            {
                indices[currentIndex++] = i + slices;
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;

                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;
            }

            // Top circle/cone of shape
            for (var i = vertexCount - slices - 1; i < vertexCount - 1; i++)
            {
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                indices[currentIndex++] = vertexCount - 1;
            }

            return indices;
        }
    }
}