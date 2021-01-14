using Aristarete.Basic;

namespace Aristarete.Meshes
{
    public class Cube : Mesh
    {
        private void Create(float length, float width, float height)
        {
            var p0 = new Vertex(-length * .5f, -width * .5f, height * .5f);
            var p1 = new Vertex(length * .5f, -width * .5f, height * .5f);
            var p2 = new Vertex(length * .5f, -width * .5f, -height * .5f);
            var p3 = new Vertex(-length * .5f, -width * .5f, -height * .5f);

            var p4 = new Vertex(-length * .5f, width * .5f, height * .5f);
            var p5 = new Vertex(length * .5f, width * .5f, height * .5f);
            var p6 = new Vertex(length * .5f, width * .5f, -height * .5f);
            var p7 = new Vertex(-length * .5f, width * .5f, -height * .5f);

            Vertex[] vertices =
            {
                // bottom
                p0, p1, p2, p3,
                // left
                p7, p4, p0, p3,
                // front
                p4, p5, p1, p0,
                // back
                p6, p7, p3, p2,
                // right
                p5, p6, p2, p1,
                // top
                p7, p6, p5, p4
            };

            int[] triangles =
            {
                // bottom
                3, 1, 0,
                3, 2, 1,
                // left
                7, 5, 4,
                7, 6, 5,
                // front
                11, 9, 8,
                11, 10, 9,
                // back
                15, 13, 12,
                15, 14, 13,
                // right
                19, 17, 16,
                19, 18, 17,
                // top
                23, 21, 20,
                23, 22, 21,
            };

            Float2 _00 = new Float2( 0f, 0f );
            Float2 _10 = new Float2( 1f, 0f );
            Float2 _01 = new Float2( 0f, 1f );
            Float2 _11 = new Float2( 1f, 1f );
 
            Float2[] uvs = new Float2[]
            {
                // Bottom
                _11, _01, _00, _10,
 
                // Left
                _11, _01, _00, _10,
 
                // Front
                _11, _01, _00, _10,
 
                // Back
                _11, _01, _00, _10,
 
                // Right
                _11, _01, _00, _10,
 
                // Top
                _11, _01, _00, _10,
            };

            var size = triangles.Length / 3;
            Int3[] indices = new Int3[size];
            for (var j = 0; j < size; j++)
            {
                indices[j] = new Int3(triangles[j * 3], triangles[j * 3 + 1], triangles[j * 3 + 2]);
            }
            
            for (var n = 0; n < vertices.Length; n++)
            {
                vertices[n].UV = uvs[n];
            }
            
            Vertices = vertices;
            Indices = indices;
        }

        public Cube(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 1, 1);
        }

        public Cube(VertexProcessor vertexProcessor, float length, float width, float height) : base(vertexProcessor)
        {
            Create(length, width, height);
        }
    }
}