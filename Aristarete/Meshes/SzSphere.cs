using Aristarete.Basic;
using static System.MathF;

namespace Aristarete.Meshes
{
    public class SzSphere : Mesh
    {
        public SzSphere(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 12, 24);
        }

        public SzSphere(VertexProcessor vertexProcessor, float radius, int length = 24, int latitude = 16) : base(
            vertexProcessor)
        {
            Create(radius, length, latitude);
        }

        private void Create(float radius, int vert = 24, int horiz = 12)
        {
            var vSize = vert * (horiz + 2);
            var tSize = 2 * vert * horiz;

            var vertices = new Vertex[vSize];
            var indices = new Int3[tSize];

            for (int yy = 0; yy <= horiz+1; yy++)
            {
                var y = Cos(yy * PI / (horiz + 1f));
                var r = Sqrt(1 - y * y);

                for (int rr = 0; rr < vert; rr++)
                {
                    var x = r * Cos(2 * PI * rr / vert);
                    var z = r * Sin(2 * PI * rr / vert);

                    vertices[rr + yy * vert].Position = new Float3(x, y, z);
                }
            }

            for (int yy = 0; yy < horiz; yy++)
            {
                for (int rr = 0; rr < vert; rr++)
                {
                    indices[rr + 2 * yy * vert] = new Int3(
                        (rr + 1) % vert + yy * vert,
                        rr + vert + yy * vert,
                        (rr + 1) % vert + vert + yy * vert);
                    
                    indices[rr + vert + 2 * yy * vert] = new Int3(
                        rr +  vert + yy * vert,
                        rr + 2*vert + yy * vert,
                        (rr + 1) % vert + vert + yy * vert);
                }
            }
            // for( int n = 0; n < vertices.Length; n++ )
            //     vertices[n].Normal = vertices[n].Position.Normalize();

            Vertices = vertices;
            Indices = indices;
        }
    }
}