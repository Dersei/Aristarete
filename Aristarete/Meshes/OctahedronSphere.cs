using System;
using Aristarete.Basic;
using Daeira;

namespace Aristarete.Meshes
{
    public class OctahedronSphere : Mesh
    {
        public OctahedronSphere()
        {
            Create(6, 1);
        }

        public OctahedronSphere(int subdivisions, float radius)
        {
            Create(subdivisions, radius);
        }

        private static Float3[] directions =
        {
            Float3.Left,
            Float3.Back,
            Float3.Right,
            Float3.Forward
        };


        private static int CreateVertexLine(Float3 from, Float3 to, int steps, int v, Float3[] vertices)
        {
            for (int i = 1; i <= steps; i++)
            {
                vertices[v++] = Float3.Lerp(from, to, (float) i / steps);
            }

            return v;
        }

        private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
        {
            for (int i = 1; i < steps; i++)
            {
                triangles[t++] = vBottom;
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;

                triangles[t++] = vBottom++;
                triangles[t++] = vTop++;
                triangles[t++] = vBottom;
            }

            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;
            return t;
        }

        private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
        {
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = ++vBottom;
            for (int i = 1; i <= steps; i++)
            {
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;
                triangles[t++] = vBottom;

                triangles[t++] = vBottom;
                triangles[t++] = vTop++;
                triangles[t++] = ++vBottom;
            }

            return t;
        }
        
        
        private static void CreateOctahedron (Float3[] vertices, int[] triangles, int resolution) {
            int v = 0, vBottom = 0, t = 0;
			
            for (int i = 0; i < 4; i++) {
                vertices[v++] = Float3.Down;
            }

            for (int i = 1; i <= resolution; i++) {
                float progress = (float)i / resolution;
                Float3 from, to;
                vertices[v++] = to = Float3.Lerp(Float3.Down, Float3.Forward, progress);
                for (int d = 0; d < 4; d++) {
                    from = to;
                    to = Float3.Lerp(Float3.Down, directions[d], progress);
                    t = CreateLowerStrip(i, v, vBottom, t, triangles);
                    v = CreateVertexLine(from, to, i, v, vertices);
                    vBottom += i > 1 ? i - 1 : 1;
                }
                vBottom = v - 1 - i * 4;
            }

            for (int i = resolution - 1; i >= 1; i--) {
                float progress = (float)i / resolution;
                Float3 from, to;
                vertices[v++] = to = Float3.Lerp(Float3.Up, Float3.Forward, progress);
                for (int d = 0; d < 4; d++) {
                    from = to;
                    to = Float3.Lerp(Float3.Up, directions[d], progress);
                    t = CreateUpperStrip(i, v, vBottom, t, triangles);
                    v = CreateVertexLine(from, to, i, v, vertices);
                    vBottom += i + 1;
                }
                vBottom = v - 1 - i * 4;
            }

            for (int i = 0; i < 4; i++) {
                triangles[t++] = vBottom;
                triangles[t++] = v;
                triangles[t++] = ++vBottom;
                vertices[v++] = Float3.Up;
            }
        }

        private static void Normalize(Float3[] vertices, Float3[] normals)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                normals[i] = vertices[i] = vertices[i].Normalize();
            }
        }

        private static void CreateUV(Float3[] vertices, Float2[] uv)
        {
            float previousX = 1f;
            for (int i = 0; i < vertices.Length; i++)
            {
                Float3 v = vertices[i];
                if (v.X == previousX)
                {
                    uv[i - 1] = new Float2(1, uv[i - 1].Y);
                }

                previousX = v.X;

                var textureCoordinatesX = MathF.Atan2(v.X, v.Z) / (-2f * MathF.PI);
                if (textureCoordinatesX < 0f)
                {
                    textureCoordinatesX += 1f;
                }

                var textureCoordinatesY = MathF.Asin(v.Y) / MathF.PI + 0.5f;
                uv[i] = new Float2(textureCoordinatesX, textureCoordinatesY);
            }

            uv[vertices.Length - 4] = new Float2(0.125f, uv[vertices.Length - 4].Y);
            uv[vertices.Length - 3] = new Float2(0.375f, uv[vertices.Length - 3].Y);
            uv[vertices.Length - 2] = new Float2(0.625f, uv[vertices.Length - 2].Y);
            uv[vertices.Length - 1] = new Float2(0.875f, uv[vertices.Length - 1].Y);
            uv[0] = new Float2(0.125f, uv[0].Y);
            uv[1] = new Float2(0.375f, uv[1].Y);
            uv[2] = new Float2(0.625f, uv[2].Y);
            uv[3] = new Float2(0.875f, uv[3].Y);
        }

        private static void CreateTangents(Float3[] vertices, Float4[] tangents)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Float3 v = vertices[i];
                v = new Float3(v.X, 0, v.Z).Normalize();
                Float4 tangent = new Float4(-v.Z, 0, v.X, -1);
                tangents[i] = tangent;
            }

            tangents[vertices.Length - 4] = tangents[0] = new Float4(new Float3(-1f, 0, -1f).Normalize(), 0);
            tangents[vertices.Length - 3] = tangents[1] = new Float4(new Float3(1f, 0f, -1f).Normalize(), 0);
            tangents[vertices.Length - 2] = tangents[2] = new Float4(new Float3(1f, 0f, 1f).Normalize(), 0);
            tangents[vertices.Length - 1] = tangents[3] = new Float4(new Float3(-1f, 0f, 1f).Normalize(), 0);
            for (int i = 0; i < 4; i++)
            {
                tangents[vertices.Length - 1 - i] = new Float4(tangents[vertices.Length - 1 - i].XYZ(), -1);
                tangents[i] = new Float4(tangents[i].XYZ(), -1);
            }
        }


        private void Create(int subdivisions, float radius)
        {
            if (subdivisions < 0) {
                subdivisions = 0;
                Console.WriteLine("Octahedron Sphere subdivisions increased to minimum, which is 0.");
            }
            else if (subdivisions > 6) {
                subdivisions = 6;
                Console.WriteLine("Octahedron Sphere subdivisions decreased to maximum, which is 6.");
            }

            int resolution = 1 << subdivisions;
            Float3[] vertices = new Float3[(resolution + 1) * (resolution + 1) * 4 - (resolution * 2 - 1) * 3];
            Console.WriteLine(vertices.Length);
            int[] triangles = new int[(1 << (subdivisions * 2 + 3)) * 3];
            Console.WriteLine(triangles.Length);
            CreateOctahedron(vertices, triangles, resolution);
			
            Float3[] normals = new Float3[vertices.Length];
            Normalize(vertices, normals);

            Float2[] uv = new Float2[vertices.Length];
            CreateUV(vertices, uv);

            Float4[] tangents = new Float4[vertices.Length];
            CreateTangents(vertices, tangents);
			
            if (radius != 1f) {
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i] *= radius;
                }
            }

            Vertex[] vertexes = new Vertex[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertexes[i].Position = vertices[i];
                vertexes[i].Normal = normals[i];
                vertexes[i].UV = uv[i];
            }

            Int3[] indices = new Int3[triangles.Length/3];
            
            for (int i = 0, j = 0; i < indices.Length; i++, j+=3)
            {
                indices[i] = new Int3(triangles[j], triangles[j + 1], triangles[j + 2]);
            }
            
            Vertices = vertexes;
            Indices = indices;
            CreateTriangles();
        }
    }
}