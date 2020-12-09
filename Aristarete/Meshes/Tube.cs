﻿using System;
using Aristarete.Basic;

namespace Aristarete.Meshes
{
    public class Tube : Mesh
    {
        public Tube(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 24);
        }

        public Tube(VertexProcessor vertexProcessor, float height, int sides) : base(vertexProcessor)
        {
            Create(height, sides);
        }

        private void Create(float height, int sides)
        {
            // Outer shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2
            var bottomRadius1 = 0.5f;
            var bottomRadius2 = 0.15f;
            var topRadius1 = 0.5f;
            var topRadius2 = 0.15f;

            var verticesCap = sides * 2 + 2;
            var verticesSides = sides * 2 + 2;

            // bottom + top + sides
            Vertex[] vertices = new Vertex[verticesCap * 2 + verticesSides * 2];
            var vert = 0;
            const float _2pi = MathF.PI * 2f;

            // bottom cap
            var sideCounter = 0;
            while (vert < verticesCap)
            {
                sideCounter = sideCounter == sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / sides * _2pi;
                var cos = MathF.Cos(r1);
                var sin = MathF.Sin(r1);
                vertices[vert] = new Vertex(cos * (bottomRadius1 - bottomRadius2 * .5f), 0f,
                    sin * (bottomRadius1 - bottomRadius2 * .5f));
                vertices[vert + 1] = new Vertex(cos * (bottomRadius1 + bottomRadius2 * .5f), 0f,
                    sin * (bottomRadius1 + bottomRadius2 * .5f));
                vert += 2;
            }

            // top cap
            sideCounter = 0;
            while (vert < verticesCap * 2)
            {
                sideCounter = sideCounter == sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / sides * _2pi;
                var cos = MathF.Cos(r1);
                var sin = MathF.Sin(r1);
                vertices[vert] = new Vertex(cos * (topRadius1 - topRadius2 * .5f), height,
                    sin * (topRadius1 - topRadius2 * .5f));
                vertices[vert + 1] = new Vertex(cos * (topRadius1 + topRadius2 * .5f), height,
                    sin * (topRadius1 + topRadius2 * .5f));
                vert += 2;
            }

            // sides (out)
            sideCounter = 0;
            while (vert < verticesCap * 2 + verticesSides)
            {
                sideCounter = sideCounter == sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / sides * _2pi;
                var cos = MathF.Cos(r1);
                var sin = MathF.Sin(r1);

                vertices[vert] = new Vertex(cos * (topRadius1 + topRadius2 * .5f), height,
                    sin * (topRadius1 + topRadius2 * .5f));
                vertices[vert + 1] = new Vertex(cos * (bottomRadius1 + bottomRadius2 * .5f), 0,
                    sin * (bottomRadius1 + bottomRadius2 * .5f));
                vert += 2;
            }

            // sides (in)
            sideCounter = 0;
            while (vert < vertices.Length)
            {
                sideCounter = sideCounter == sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / sides * _2pi;
                var cos = MathF.Cos(r1);
                var sin = MathF.Sin(r1);

                vertices[vert] = new Vertex(cos * (topRadius1 - topRadius2 * .5f), height,
                    sin * (topRadius1 - topRadius2 * .5f));
                vertices[vert + 1] = new Vertex(cos * (bottomRadius1 - bottomRadius2 * .5f), 0,
                    sin * (bottomRadius1 - bottomRadius2 * .5f));
                vert += 2;
            }


            var facesNumber = sides * 4;
            var trianglesNumber = facesNumber * 2;
            var indexesNumber = trianglesNumber * 3;
            int[] triangles = new int[indexesNumber];

            // bottom cap
            var i = 0;
            sideCounter = 0;
            while (sideCounter < sides)
            {
                var current = sideCounter * 2;
                var next = sideCounter * 2 + 2;

                triangles[i++] = next + 1;
                triangles[i++] = next;
                triangles[i++] = current;

                triangles[i++] = current + 1;
                triangles[i++] = next + 1;
                triangles[i++] = current;

                sideCounter++;
            }

            // top cap
            while (sideCounter < sides * 2)
            {
                var current = sideCounter * 2 + 2;
                var next = sideCounter * 2 + 4;

                triangles[i++] = current;
                triangles[i++] = next;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = current + 1;

                sideCounter++;
            }

            // sides (out)
            while (sideCounter < sides * 3)
            {
                var current = sideCounter * 2 + 4;
                var next = sideCounter * 2 + 6;

                triangles[i++] = current;
                triangles[i++] = next;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = current + 1;

                sideCounter++;
            }


            // sides (in)
            while (sideCounter < sides * 4)
            {
                var current = sideCounter * 2 + 6;
                var next = sideCounter * 2 + 8;

                triangles[i++] = next + 1;
                triangles[i++] = next;
                triangles[i++] = current;

                triangles[i++] = current + 1;
                triangles[i++] = next + 1;
                triangles[i++] = current;

                sideCounter++;
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