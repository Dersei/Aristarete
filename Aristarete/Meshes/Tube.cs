using System;
using Daeira;


namespace Aristarete.Meshes
{
    public class Tube : Mesh
    {
        public Tube()
        {
            Create(1, 24);
        }

        public Tube(float height, int sides)
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
            
            // bottom + top + sides
            Float3[] normales = new Float3[vertices.Length];
            vert = 0;
 
// Bottom cap
            while( vert < verticesCap )
            {
                normales[vert++] = Float3.Down;
            }
 
// Top cap
            while( vert < verticesCap * 2 )
            {
                normales[vert++] = Float3.Up;
            }
 
// Sides (out)
            sideCounter = 0;
            while (vert < verticesCap * 2 + verticesSides )
            {
                sideCounter = sideCounter == sides ? 0 : sideCounter;
 
                float r1 = (float)sideCounter++ / sides * _2pi;
 
                normales[vert] = new Float3(MathF.Cos(r1), 0f, MathF.Sin(r1));
                normales[vert+1] = normales[vert];
                vert+=2;
            }
 
// Sides (in)
            sideCounter = 0;
            while (vert < vertices.Length )
            {
                sideCounter = sideCounter == sides ? 0 : sideCounter;
 
                float r1 = (float)sideCounter++ / sides * _2pi;
 
                normales[vert] = -new Float3(MathF.Cos(r1), 0f, MathF.Sin(r1));
                normales[vert+1] = normales[vert];
                vert+=2;
            }
            
            Float2[] uvs = new Float2[vertices.Length];
 
            vert = 0;
// Bottom cap
            sideCounter = 0;
            while( vert < verticesCap )
            {
                float t = (float)sideCounter++ / sides;
                uvs[ vert++ ] = new Float2( 0f, t );
                uvs[ vert++ ] = new Float2( 1f, t );
            }
 
// Top cap
            sideCounter = 0;
            while( vert < verticesCap * 2 )
            {
                float t = (float)sideCounter++ / sides;
                uvs[ vert++ ] = new Float2( 0f, t );
                uvs[ vert++ ] = new Float2( 1f, t );
            }
 
// Sides (out)
            sideCounter = 0;
            while (vert < verticesCap * 2 + verticesSides )
            {
                float t = (float)sideCounter++ / sides;
                uvs[ vert++ ] = new Float2( t, 0f );
                uvs[ vert++ ] = new Float2( t, 1f );
            }
 
// Sides (in)
            sideCounter = 0;
            while (vert < vertices.Length )
            {
                float t = (float)sideCounter++ / sides;
                uvs[ vert++ ] = new Float2( t, 0f );
                uvs[ vert++ ] = new Float2( t, 1f );
            }


            var size = triangles.Length / 3;
            Int3[] indices = new Int3[size];
            for (var j = 0; j < size; j++)
            {
                indices[j] = new Int3(triangles[j * 3], triangles[j * 3 + 1], triangles[j * 3 + 2]);
            }
            
            for (var n = 0; n < vertices.Length; n++)
            {
                vertices[n].UV = uvs[n];
                vertices[n].Normal = normales[n];
            }

            Vertices = vertices;
            Indices = indices;
            
            CreateTriangles();
        }
    }
}