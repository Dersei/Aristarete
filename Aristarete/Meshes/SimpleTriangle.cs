using Aristarete.Basic;


namespace Aristarete.Meshes
{
    public class SimpleTriangle : Mesh
    {
        public SimpleTriangle()
        {
            Vertices = new Vertex[]
            {
                new()
                {
                    Position = new Float3Sse(-0.5f, 0f, 0f)
                },
                new()
                {
                    Position = new Float3Sse(0f, 0.5f, 0f)
                },
                new()
                {
                    Position = new Float3Sse(0.5f, 0f, 0f)
                }
            };

            Indices = new[] {new Int3(0, 1, 2)};
        }
    }
}