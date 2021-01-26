using Aristarete.Basic;
using Daeira;

namespace Aristarete.Meshes
{
    public class Plane : Mesh
    {
        public Plane()
        {
            Create();
        }

        public Plane(float length = 1, float width = 1, int resX = 2, int resZ = 2)
        {
            Create(length, width, resX, resZ);
        }

        private void Create(float length = 1, float width = 1, int resX = 2, int resZ = 2)
        {
            Vertex[] vertices = new Vertex[resX * resZ];
            for (var z = 0; z < resZ; z++)
            {
                // [ -length / 2, length / 2 ]
                var zPos = ((float) z / (resZ - 1) - .5f) * length;
                for (var x = 0; x < resX; x++)
                {
                    // [ -width / 2, width / 2 ]
                    var xPos = ((float) x / (resX - 1) - .5f) * width;
                    vertices[x + z * resX] = new Vertex(xPos, 0f, zPos);
                }
            }

            var nbFaces = (resX - 1) * (resZ - 1);
            int[] triangles = new int[nbFaces * 6];
            var t = 0;
            for (var face = 0; face < nbFaces; face++)
            {
                // Retrieve lower left corner from face ind
                var i = face % (resX - 1) + face / (resZ - 1) * resX;

                triangles[t++] = i + resX;
                triangles[t++] = i + 1;
                triangles[t++] = i;

                triangles[t++] = i + resX;
                triangles[t++] = i + resX + 1;
                triangles[t++] = i + 1;
            }
            
            Float2[] uvs = new Float2[ vertices.Length ];
            for(int v = 0; v < resZ; v++)
            {
                for(int u = 0; u < resX; u++)
                {
                    uvs[ u + v * resX ] = new Float2( (float)u / (resX - 1), (float)v / (resZ - 1) );
                }
            }

            var size = triangles.Length / 3;
            Int3[] indices = new Int3[size];
            for (var j = 0; j < size; j++)
            {
                indices[j] = new Int3(triangles[j * 3], triangles[j * 3 + 1], triangles[j * 3 + 2]);
            }

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal = Float3.Up;
                vertices[i].UV = uvs[i];
            }

            Vertices = vertices;
            Indices = indices;
            CreateTriangles();
        }
    }
}